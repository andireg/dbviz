using DatabaseVisualizer;
using DatabaseVisualizer.DataAccess;
using DatabaseVisualizer.Renderer;
using DatabaseVisualizer.Renderer.DependencyInjection;
using DatabaseVisualizer.Renderer.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ConfigurationManager configuration = new();
if (args != null)
{
    configuration.AddCommandLine(args);
}

IServiceProvider serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddRenderer<ClassDiagramRenderer>()
    .AddRenderer<ErDiagramRenderer>()
    .AddSingleton<IDbFactory, DbFactory>()
    .AddTransient<IDatabaseParser, DatabaseParser>()
    .AddTransient<Launcher>()
    .AddSingleton<IRendererFactory, RendererFactory>()
    .AddLogging(loggingBuilder => loggingBuilder
        .AddConsole()
        .AddDebug())
    .Configure<AppSettings>(configuration)
    .BuildServiceProvider();

await serviceProvider
    .GetRequiredService<Launcher>()
    .RunAsync(default);
