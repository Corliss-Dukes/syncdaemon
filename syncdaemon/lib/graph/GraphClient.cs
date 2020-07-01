using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

namespace syncdaemon
{
    public class GraphClient
    {
        public GraphServiceClient getClient(IConfigurationRoot config)
        {
            return GetAuthenticatedGraphClient(config);
        }
        public static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            return new GraphServiceClient(authenticationProvider);
        }
        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var tenantId = config["tenantId"];
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithClientSecret(clientSecret)
                                                    .Build();
            return MsalAuthenticationProvider.GetInstance(cca, scopes.ToArray());
        }
    }
}
