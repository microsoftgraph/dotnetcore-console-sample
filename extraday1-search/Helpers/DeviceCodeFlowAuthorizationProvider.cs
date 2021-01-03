using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace ConsoleGraphTest {
    public class DeviceCodeFlowAuthorizationProvider : IAuthenticationProvider
    {
        private readonly IPublicClientApplication _application;
        private readonly List<string> _scopes;
        private string _authToken;
        public DeviceCodeFlowAuthorizationProvider(IPublicClientApplication application, List<string> scopes) {
            _application = application;
            _scopes = scopes;
        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            if(string.IsNullOrEmpty(_authToken))
            {
                var result = await _application.AcquireTokenWithDeviceCode(_scopes, callback => {
                    Console.WriteLine(callback.Message);
                    return Task.FromResult(0);
                }).ExecuteAsync();
                _authToken = result.AccessToken;
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _authToken);
        }
    }
}