using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;

namespace lettermint_dotnet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLettermint(this IServiceCollection services, Action<LettermintOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        //services.AddHttpClient<ILettermintClient, LettermintClient>();

        services.AddHttpClient<ILettermintClient, LettermintClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<LettermintOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                throw new InvalidOperationException("Lettermint API key is required.");
            }

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new InvalidOperationException("Lettermint BaseUrl is required.");
            }

            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("x-lettermint-token", options.ApiKey);
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        return services;
    }
}
