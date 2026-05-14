namespace EnterpriseCMS.Plugins.Core;

public interface ICmsPlugin
{
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    void Configure(IServiceProvider serviceProvider);
}
