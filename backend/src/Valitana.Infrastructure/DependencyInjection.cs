using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valitana.Application.Abstractions;
using Valitana.Domain.Time;
using Valitana.Infrastructure.Messaging;
using Valitana.Infrastructure.Persistence;
using Valitana.Infrastructure.Time;

namespace Valitana.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((_, cfg) =>
            {
                var options = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
                    ?? new RabbitMqOptions();
                cfg.Host(options.HostName, (ushort)options.Port, "/", h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });
            });
        });

        services.AddDbContext<ValitanaDbContext>(options =>
            options.UseInMemoryDatabase("Valitana"));
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

        return services;
    }
}
