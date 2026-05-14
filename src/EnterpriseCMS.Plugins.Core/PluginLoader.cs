namespace EnterpriseCMS.Plugins.Core;

public class PluginLoader
{
    private readonly List<ICmsPlugin> _plugins = new();

    public IReadOnlyList<ICmsPlugin> LoadedPlugins => _plugins.AsReadOnly();

    public void LoadFromDirectory(string path)
    {
        // Plugin discovery: load assemblies from path
        // For Phase 1, this is intentionally empty (zero plugins)
    }
}
