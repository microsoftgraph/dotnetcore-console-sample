using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Threading;

namespace GraphWebhooks
{
    // This class allows an implementation of IAuthenticationProvider to be inserted into the DelegatingHandler
    // pipeline of an HttpClient instance.  In future versions of GraphSDK, many cross-cutting concerns will
    // be implemented as DelegatingHandlers.  This AuthHandler will come in the box.
    public class AuthHandler : DelegatingHandler
    {
        private IAuthenticationProvider _authenticationProvider;

        public AuthHandler(IAuthenticationProvider authenticationProvider, HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
            _authenticationProvider = authenticationProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _authenticationProvider.AuthenticateRequestAsync(request);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
