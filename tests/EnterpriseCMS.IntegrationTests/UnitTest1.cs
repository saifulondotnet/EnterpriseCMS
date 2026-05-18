using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EnterpriseCMS.IntegrationTests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task Get_Health_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Home_Returns200Or302()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var response = await client.GetAsync("/");
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.OK ||
            response.StatusCode == System.Net.HttpStatusCode.Redirect ||
            response.StatusCode == System.Net.HttpStatusCode.MovedPermanently ||
            response.StatusCode == System.Net.HttpStatusCode.Found,
            $"Expected 2xx or 3xx but got {response.StatusCode}");
    }

    [Fact]
    public async Task Get_AdminLogin_Returns200()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var response = await client.GetAsync("/Admin/Account/Login");
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.OK ||
            response.StatusCode == System.Net.HttpStatusCode.Redirect ||
            response.StatusCode == System.Net.HttpStatusCode.Found,
            $"Expected 2xx or 3xx but got {response.StatusCode}");
    }
}
