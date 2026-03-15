using System;
using System.Net.Http;

namespace Layla.Desktop.Services
{
    public static class ConfigurationService
    {
        /// <summary>
        /// Base URL for the .NET Server Core API (Identity, Projects, Users).
        /// </summary>
        public static string ServerCoreUrl { get; } = "https://localhost:7165";

        /// <summary>
        /// Base URL for the Node.js Worldbuilding API (Manuscripts, Wiki, Graph).
        /// </summary>
        public static string WorldbuildingApiUrl { get; } = "http://localhost:3000";

        /// <summary>
        /// Creates an HttpClient configured for local development (bypasses SSL validation for localhost).
        /// </summary>
        public static HttpClient CreateHttpClient(string baseUrl)
        {
            var handler = new HttpClientHandler();

            // SSL certificate bypass for local development. TODO: Remove this in production.
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert?.Subject.Contains("localhost") == true || baseUrl.Contains("localhost"))
                {
                    return true;
                }
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
    }
}
