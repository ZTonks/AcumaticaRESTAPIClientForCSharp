using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Acumatica.RESTClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAcumaticaHttpClientHandler(
            this IServiceCollection serviceCollection,
            string acumaticaHttpClientName) =>
                serviceCollection.AddSingleton<RESTClient.Client.IHttpClientHandler>(
                    sp => new Client.HttpClientHandler(
                        sp.GetRequiredService<CookieContainer>(),
                        sp.GetRequiredService<IHttpClientFactory>(),
                        acumaticaHttpClientName));

        public static IServiceCollection ConfigureDefaultAcumaticaApiClientDependencies(
            this IServiceCollection serviceCollection,
            int timeout = 100000,
            bool ignoreSslErrors = false)
        {
            const string httpClientName = "AcumaticaHttpClient";
            var cookies = new CookieContainer();

            serviceCollection
                .AddSingleton(_ => cookies)
                .AddHttpClient(
                    httpClientName,
                    c =>
                    {
                        c.Timeout = TimeSpan.FromMilliseconds(timeout);
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

            return serviceCollection
                .ConfigureAcumaticaHttpClientHandler(httpClientName);
        }
    }
}
