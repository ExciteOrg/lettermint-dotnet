using lettermint_dotnet.SendingApi;
using lettermint_dotnet.TeamApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lettermint;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLettermint(this IServiceCollection services, Action<LettermintOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        // Peek at the configured options so we can decide which clients to register.
        // Each API uses its own key; a consumer may configure only one of them.
        var options = new LettermintOptions();
        configureOptions(options);

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new InvalidOperationException("Lettermint BaseUrl is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey) && string.IsNullOrWhiteSpace(options.TeamApiKey))
        {
            throw new InvalidOperationException("At least one of Lettermint ApiKey (sending) or TeamApiKey (team) is required.");
        }

        // Sending API — only registered when a sending key is configured.
        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            // Register the email whitelist validator as a singleton (stateless after initialization)
            services.AddSingleton<IEmailWhitelistValidator, EmailWhitelistValidator>();

            services.AddHttpClient<ILettermintSendingClient, LettermintSendingClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<LettermintOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("x-lettermint-token", options.ApiKey);
                client.Timeout = TimeSpan.FromSeconds(15);
            });
        }

        // Team API — uses a separate key, only registered when that key is configured.
        if (!string.IsNullOrWhiteSpace(options.TeamApiKey))
        {
            services.AddHttpClient<ILettermintTeamClient, LettermintTeamClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<LettermintOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.TeamApiKey);
                client.Timeout = TimeSpan.FromSeconds(15);
            });
        }

        return services;
    }
}
