namespace EnterpriseCMS.Plugins.Core;

public class PluginManifest
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntryPoint { get; set; } = string.Empty;
    public List<string> Dependencies { get; set; } = new();
}
