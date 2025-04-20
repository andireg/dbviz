using DatabaseVisualizer.Renderer.DependencyInjection;
using DatabaseVisualizer.Renderer.Metadata;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DatabaseVisualizer.Renderer.Factory;

public partial class RendererFactory(
    IOptions<AppSettings> appSettingsOptions,
    IEnumerable<RendererRegistration> rendererRegistrations,
    IServiceProvider serviceProvider) : IRendererFactory
{
    public IEnumerable<IRenderer> GetRenderers()
    {
        List<IRenderer> renderers = [];
        AppSettings appSettings = appSettingsOptions.Value;
        MatchCollection matches = RendererRegex().Matches(appSettings.Render);
        foreach (Match match in matches)
        {
            string parserKey = match.Groups[1].Value;

            Type? rendererType = rendererRegistrations.FirstOrDefault(x =>
                string.Equals(x.Name, parserKey, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.ShortName, parserKey, StringComparison.OrdinalIgnoreCase))?.RendererType;

            if (rendererType == null)
            {
                throw new InvalidOperationException($"Renderer '{parserKey}' not found.");
            }

            IRenderer renderer = (IRenderer)serviceProvider.GetService(rendererType);

            string? parserValue = match.Groups.Count > 2 ? match.Groups[2].Value : null;
            if (!string.IsNullOrWhiteSpace(parserValue))
            {
                PropertyInfo[] propertyInfos = rendererType.GetProperties();
                Dictionary<string, string?> options = ParseOptions(parserValue);
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    RenderParameterAttribute? renderParameterAttribute = propertyInfo.GetCustomAttribute<RenderParameterAttribute>();
                    string name = renderParameterAttribute?.Name ?? propertyInfo.Name;
                    string shortName = renderParameterAttribute?.ShortName ?? propertyInfo.Name;

                    if (options.TryGetValue(name, out string? value) || options.TryGetValue(shortName, out value))
                    {
                        if (propertyInfo.PropertyType == typeof(bool))
                        {
                            propertyInfo.SetValue(renderer, bool.Parse(value ?? "false"));
                        }
                        else if (propertyInfo.PropertyType == typeof(string))
                        {
                            propertyInfo.SetValue(renderer, value);
                        }
                        else if (propertyInfo.PropertyType == typeof(int))
                        {
                            propertyInfo.SetValue(renderer, int.Parse(value ?? "0"));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported property type '{propertyInfo.PropertyType.Name}' for '{name}'.");
                        }
                    }
                }
            }

            renderers.Add(renderer);
        }

        return renderers;
    }

    private static Dictionary<string, string?> ParseOptions(string options)
    {
        Dictionary<string, string?> result = [];
        string innerValue = options.Trim('[', ']');
        MatchCollection matches = Regex.Matches(innerValue, "([^,:]*)(:)?([^,]*)?(,)?");
        foreach (Match match in matches)
        {
            result.Add(match.Groups[1].Value, match.Groups.Count > 3 ? match.Groups[3].Value : "true");
        }

        return result;
    }


    [GeneratedRegex("([^;\\[]{2,})(\\[[^\\]]*\\])?")]
    private static partial Regex RendererRegex();
}
