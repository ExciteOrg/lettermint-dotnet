using Microsoft.Extensions.DependencyInjection;

namespace lettermint_dotnet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLettermint(this IServiceCollection services, Action<LettermintOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        services.AddHttpClient<ILettermintClient, LettermintClient>();

        return services;
    }
}
