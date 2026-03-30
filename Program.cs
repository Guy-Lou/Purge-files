using Gelf.Extensions.Logging;
using GestionPurge.Main;
using GestionPurge.Main.Plannification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Services
    .AddOptionsWithValidateOnStart<GestionPlannificationHorairesOptions>().BindConfiguration(nameof(GestionPlannificationHorairesOptions)).Services
    .AddOptionsWithValidateOnStart<GestionPlannificationCyclesOptions>().BindConfiguration(nameof(GestionPlannificationCyclesOptions)).Services
    .AddOptionsWithValidateOnStart<GestionPlannificationOptions>().BindConfiguration(nameof(GestionPlannificationOptions)).Services
    .AddOptionsWithValidateOnStart<GestionPurgeOptions>().BindConfiguration(nameof(GestionPurgeOptions)).Services
    .AddBREBDDServiceProvider()
    //.AddSingleton<JeuTestGED>()
    //.AddSingleton<JeuTestEDI>()
    .AddSingleton<PurgeGed>()
    .AddSingleton<PurgeEDI>()
    .AddSingleton<Horaires>()
    .AddSingleton<Cycles>()
    .AddHostedService<Worker>();

builder.Logging.AddGelf();

var host = builder.Build();
await host.RunAsync();