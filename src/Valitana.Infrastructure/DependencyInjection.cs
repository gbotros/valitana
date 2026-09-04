using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valitana.Application.Abstractions;
using Valitana.Infrastructure.Messaging;
using Valitana.Infrastructure.Persistence;

namespace Valitana.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PriceHistoryOptions>()
            .Bind(configuration.GetSection(PriceHistoryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IStockRepository, InMemoryStockRepository>();
        services.AddSingleton<IPriceHistoryStore, InMemoryPriceHistoryStore>();
        services.AddSingleton<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();

        return services;
    }
}
