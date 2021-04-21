using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace day30Sample.helpers
{
    // This class encapsulates the details of getting a token from MSAL and exposes it via the 
    // IAuthenticationProvider interface so that GraphServiceClient or AuthHandler can use it.
    // A significantly enhanced version of this class will in the future be available from
    // the GraphSDK team.  It will supports all the types of Client Application as defined by MSAL.
    public class MsalAuthenticationProvider : IAuthenticationProvider
    {
        private IConfidentialClientApplication _clientApplication;
        private string[] _scopes;

        // Please generate the token from https://developer.microsoft.com/en-us/graph/graph-explorer for testing usage.
        private string _token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IlJCREE4OEpyZm5Lc1QzcmRTQ0I3b09ESjhoU3RqZU1COFM1aGo4dVlsdW8iLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNjE4ODMwOTMxLCJuYmYiOjE2MTg4MzA5MzEsImV4cCI6MTYxODgzNDgzMSwiYWNjdCI6MCwiYWNyIjoiMSIsImFjcnMiOlsidXJuOnVzZXI6cmVnaXN0ZXJzZWN1cml0eWluZm8iLCJ1cm46bWljcm9zb2Z0OnJlcTIiLCJ1cm46bWljcm9zb2Z0OnJlcTMiLCJjMSIsImMyIiwiYzMiLCJjNCIsImM1IiwiYzYiLCJjNyIsImM4IiwiYzkiLCJjMTAiLCJjMTEiLCJjMTIiLCJjMTMiLCJjMTQiLCJjMTUiLCJjMTYiLCJjMTciLCJjMTgiLCJjMTkiLCJjMjAiLCJjMjEiLCJjMjIiLCJjMjMiLCJjMjQiLCJjMjUiXSwiYWlvIjoiQVVRQXUvOFRBQUFBMmxmNlBOMG5XU01DTzFYMXN1NVptaEEwamE2TkZXWUJBN0xkVUdhWVAvdkw4NFFQRUhlWDJUckNEU29NcExuZnVDVWFFaVFJcE9VeVl4YnpSQ0V5dEE9PSIsImFtciI6WyJyc2EiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiR3JhcGggZXhwbG9yZXIgKG9mZmljaWFsIHNpdGUpIiwiYXBwaWQiOiJkZThiYzhiNS1kOWY5LTQ4YjEtYThhZC1iNzQ4ZGE3MjUwNjQiLCJhcHBpZGFjciI6IjAiLCJjb250cm9scyI6WyJhcHBfcmVzIl0sImNvbnRyb2xzX2F1ZHMiOlsiZGU4YmM4YjUtZDlmOS00OGIxLWE4YWQtYjc0OGRhNzI1MDY0IiwiMDAwMDAwMDMtMDAwMC0wMDAwLWMwMDAtMDAwMDAwMDAwMDAwIiwiMDAwMDAwMDMtMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwIl0sImRldmljZWlkIjoiY2M2ZjQ1OWUtNWQ5OS00N2YzLWJlYTEtMDQzMTIxNWQ3NTI3IiwiZmFtaWx5X25hbWUiOiJXYW5nIChNU0FJKSIsImdpdmVuX25hbWUiOiJZaXdlbiIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjE2Ny4yMjAuMjMzLjQyIiwibmFtZSI6Illpd2VuIFdhbmcgKE1TQUkpIiwib2lkIjoiNDk3YjdhMmEtOWUxYS00OGQ3LTgwZTgtMjk2NWQyZmMzYTgxIiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTIxNDY3NzMwODUtOTAzMzYzMjg1LTcxOTM0NDcwNy0yNjkxMjgwIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAxMDYxQTY3NzIiLCJyaCI6IjAuQVFFQXY0ajVjdkdHcjBHUnF5MTgwQkhiUjdYSWk5NzUyYkZJcUsyM1NOcHlVR1FhQUQ0LiIsInNjcCI6IkNhbGVuZGFycy5SZWFkV3JpdGUgQ29udGFjdHMuUmVhZFdyaXRlIERldmljZU1hbmFnZW1lbnRBcHBzLlJlYWRXcml0ZS5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZC5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUHJpdmlsZWdlZE9wZXJhdGlvbnMuQWxsIERldmljZU1hbmFnZW1lbnRNYW5hZ2VkRGV2aWNlcy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkV3JpdGUuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWQuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWRXcml0ZS5BbGwgRGlyZWN0b3J5LkFjY2Vzc0FzVXNlci5BbGwgRGlyZWN0b3J5LlJlYWRXcml0ZS5BbGwgRmlsZXMuUmVhZFdyaXRlLkFsbCBHcm91cC5SZWFkV3JpdGUuQWxsIElkZW50aXR5Umlza0V2ZW50LlJlYWQuQWxsIE1haWwuUmVhZFdyaXRlIE1haWxib3hTZXR0aW5ncy5SZWFkV3JpdGUgTm90ZXMuUmVhZFdyaXRlLkFsbCBvcGVuaWQgUGVvcGxlLlJlYWQgUHJlc2VuY2UuUmVhZCBQcmVzZW5jZS5SZWFkLkFsbCBwcm9maWxlIFJlcG9ydHMuUmVhZC5BbGwgU2l0ZXMuUmVhZFdyaXRlLkFsbCBUYXNrcy5SZWFkV3JpdGUgVXNlci5SZWFkIFVzZXIuUmVhZEJhc2ljLkFsbCBVc2VyLlJlYWRXcml0ZSBVc2VyLlJlYWRXcml0ZS5BbGwgZW1haWwiLCJzaWduaW5fc3RhdGUiOlsiZHZjX21uZ2QiLCJkdmNfY21wIiwiZHZjX2RtamQiLCJpbmtub3dubnR3ayIsImttc2kiXSwic3ViIjoiRldqb014STdJcm9uUlVsX0hqNXVSbk9wWWs5ZGJvYWpJeGpwYVNpN3hzNCIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJXVyIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoieWl3ZW53YW5nQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJ5aXdlbndhbmdAbWljcm9zb2Z0LmNvbSIsInV0aSI6IlJTOXhoT2pvZTA2TWxPUWV0dVFVQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoieDNzS1FBOWtCRkZBVXdsYy1sbXpxNExCWTJibzFuSGk3ZzAydTR3bzU1WSJ9LCJ4bXNfdGNkdCI6MTI4OTI0MTU0N30.nhA-HUQfcQFWEtk0BNSfYsDXtLjLn2JL7QMK-H8o67n8t4NTQfQB_HqfJj78Tmn0wRwpkKebntl0wmLn5cGvbDIV8sa3wilqjnlUMy8mFnbptpvdq2UH0pwQaAwzEf1m_A5HDV-qpyZv1ssTPqom-rhgj5znyRThM682r3VGQLYK3J6M7Yq7xbhaMIenZ6p3Hx-V6HL3fBMXRs0JFPbL7M928--1MB36JQbhlY0EGNfs-h2hEXdG_PUG8okvX16_2gf0G9XM9F_FVNMnsFnNNWXO1NGYTPSS0iy9x2J2MG2oDfalgJHSEW0Y_RqXNzIBXH5JmEpJb2fWUsgRhzL06Q";


        public MsalAuthenticationProvider(IConfidentialClientApplication clientApplication, string[] scopes) {
            _clientApplication = clientApplication;
            _scopes = scopes;
        }

        /// <summary>
        /// Update HttpRequestMessage with credentials
        /// </summary>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        /// <summary>
        /// Acquire Token 
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            AuthenticationResult authResult = null;
            authResult = await _clientApplication.AcquireTokenForClient(_scopes)
                                .ExecuteAsync();
            return authResult.AccessToken;
        }

        public string getMockToken()
        {
            return this._token;
        }
    }
}