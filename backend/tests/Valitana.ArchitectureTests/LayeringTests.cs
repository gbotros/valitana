using System.Reflection;
using NetArchTest.Rules;
using Xunit;
using Valitana.Api.Controllers;
using Valitana.Application.Commands;
using Valitana.Contracts;
using Valitana.Domain.Aggregates;
using Valitana.Infrastructure;
using Valitana.SignalR;

namespace Valitana.ArchitectureTests;

public sealed class LayeringTests
{
    private static readonly Assembly Domain = typeof(Stock).Assembly;
    private static readonly Assembly Application = typeof(SetStockPriceCommand).Assembly;
    private static readonly Assembly Contracts = typeof(StockPriceUpdated).Assembly;
    private static readonly Assembly Infrastructure = typeof(DependencyInjection).Assembly;
    private static readonly Assembly Api = typeof(PricesController).Assembly;
    private static readonly Assembly SignalR = typeof(PriceHub).Assembly;
    private static readonly Assembly Publisher = Assembly.LoadFrom(
        Path.Combine(AppContext.BaseDirectory, "Valitana.Publisher.dll"));

    [Fact]
    public void Domain_must_not_depend_on_other_layers_or_infrastructure_packages()
    {
        ShouldNotDependOn(
            Domain,
            "Valitana.Application",
            "Valitana.Infrastructure",
            "Valitana.Api",
            "Valitana.SignalR",
            "Valitana.Publisher",
            "Valitana.Contracts",
            "MediatR",
            "MassTransit",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore");
    }

    [Fact]
    public void Contracts_must_not_depend_on_other_valitana_assemblies_or_infrastructure_packages()
    {
        ShouldNotDependOn(
            Contracts,
            "Valitana.Domain",
            "Valitana.Application",
            "Valitana.Infrastructure",
            "Valitana.Api",
            "Valitana.SignalR",
            "Valitana.Publisher",
            "MediatR",
            "MassTransit",
            "Microsoft.EntityFrameworkCore");
    }

    [Fact]
    public void Application_must_not_depend_on_outer_layers_or_infrastructure_packages()
    {
        ShouldNotDependOn(
            Application,
            "Valitana.Infrastructure",
            "Valitana.Api",
            "Valitana.SignalR",
            "Valitana.Publisher",
            "MassTransit",
            "Microsoft.EntityFrameworkCore");
    }

    [Fact]
    public void Infrastructure_must_not_depend_on_hosts()
    {
        ShouldNotDependOn(
            Infrastructure,
            "Valitana.Api",
            "Valitana.SignalR",
            "Valitana.Publisher");
    }

    [Fact]
    public void SignalR_must_not_depend_on_domain_application_or_other_hosts()
    {
        ShouldNotDependOn(
            SignalR,
            "Valitana.Domain",
            "Valitana.Application",
            "Valitana.Infrastructure",
            "Valitana.Api",
            "Valitana.Publisher");
    }

    [Fact]
    public void Publisher_must_not_depend_on_other_valitana_assemblies()
    {
        ShouldNotDependOn(
            Publisher,
            "Valitana.Domain",
            "Valitana.Application",
            "Valitana.Contracts",
            "Valitana.Infrastructure",
            "Valitana.Api",
            "Valitana.SignalR");
    }

    private static void ShouldNotDependOn(Assembly assembly, params string[] names)
    {
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(names)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            string.Join(", ", result.FailingTypes?.Select(type => type.FullName) ?? []));
    }
}
