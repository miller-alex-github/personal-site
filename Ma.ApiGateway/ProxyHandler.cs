using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ma.ApiGateway
{
    public class ProxyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {             
            if (((HttpClientHandler)InnerHandler).Proxy == null)
            {
                // Fix: Despite all my efforts, I can't figure out how can I set the proxy to the ocelot api.
                // When I detect that no proxy is defined then I set up it here once.
                ((HttpClientHandler)InnerHandler).Proxy = new WebProxy("http://winproxy.server.lan:3128", true)
                {
                    UseDefaultCredentials = true                    
                };
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
