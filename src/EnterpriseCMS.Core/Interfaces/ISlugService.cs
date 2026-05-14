namespace EnterpriseCMS.Core.Interfaces;

public interface ISlugService
{
    string Generate(string text);
    Task<string> GenerateUniqueAsync(string text, Func<string, Task<bool>> existsAsync);
}
