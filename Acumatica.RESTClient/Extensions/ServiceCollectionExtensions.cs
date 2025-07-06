using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Acumatica.RESTClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAcumaticaApiClientDependencies(
            this IServiceCollection serviceCollection,
            int timeout = 100000,
            bool ignoreSslErrors = false)
        {
            const string httpClientName = "AcumaticaHttpClient";
            var cookies = new CookieContainer();

            serviceCollection
                .AddHttpClient(
                    httpClientName,
                    c =>
                    {
                        c.Timeout = new TimeSpan(0, 0, 0, 0, timeout);
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => ignoreSslErrors
                        ? new HttpClientHandler
                        {
                            UseCookies = true,
                            CookieContainer = cookies,
                            ServerCertificateCustomValidationCallback =
                                (_, _, _, _) => true,
                        }
                        : new HttpClientHandler
                        {
                            UseCookies = true,
                            CookieContainer = cookies,
                        });

            return serviceCollection.AddSingleton(
                sp => new Client.HttpClientHandler(
                    cookies,
                    sp.GetRequiredService<IHttpClientFactory>(),
                    httpClientName));
        }
    }
}
