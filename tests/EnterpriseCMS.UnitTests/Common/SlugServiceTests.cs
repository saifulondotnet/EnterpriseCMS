using EnterpriseCMS.Infrastructure.Services;
using FluentAssertions;

namespace EnterpriseCMS.UnitTests.Common;

public class SlugServiceTests
{
    private readonly SlugService _slugService = new();

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("My Blog Post Title", "my-blog-post-title")]
    [InlineData("  Leading & Trailing  ", "leading-trailing")]
    [InlineData("Special @#$% Characters", "special-characters")]
    [InlineData("Multiple   Spaces", "multiple-spaces")]
    public void Generate_ShouldProduceExpectedSlug(string input, string expected)
    {
        var result = _slugService.Generate(input);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GenerateUniqueAsync_ShouldReturnSlug_WhenNotExists()
    {
        var result = await _slugService.GenerateUniqueAsync("My Page", _ => Task.FromResult(false));
        result.Should().Be("my-page");
    }

    [Fact]
    public async Task GenerateUniqueAsync_ShouldAppendNumber_WhenSlugExists()
    {
        var calls = 0;
        var result = await _slugService.GenerateUniqueAsync("My Page",
            _ => Task.FromResult(calls++ < 2));
        result.Should().Be("my-page-2");
    }
}
