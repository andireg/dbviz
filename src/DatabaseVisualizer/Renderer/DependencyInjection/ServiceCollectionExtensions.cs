using Microsoft.Extensions.DependencyInjection;

namespace DatabaseVisualizer.Renderer.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRenderer<T>(this IServiceCollection services)
        where T : class, IRenderer
    {
        services.AddTransient<IRenderer, T>();
        services.AddTransient<T>();
        services.AddSingleton(new RendererRegistration(typeof(T)));
        return services;
    }
}
