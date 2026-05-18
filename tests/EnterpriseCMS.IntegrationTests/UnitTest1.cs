using EnterpriseCMS.Infrastructure.Data;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EnterpriseCMS.IntegrationTests;

/// <summary>
/// Custom factory that replaces SQL Server with in-memory EF Core and Hangfire with in-memory storage.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server DbContext with in-memory
            services.RemoveAll<DbContextOptions<CmsDbContext>>();
            services.RemoveAll<CmsDbContext>();
            services.AddDbContext<CmsDbContext>(opts =>
                opts.UseInMemoryDatabase("IntegrationTestDb_" + Guid.NewGuid()));

            // Replace Hangfire SQL Server storage with in-memory storage
            services.RemoveAll<JobStorage>();
            services.AddSingleton<JobStorage>(new InMemoryStorage());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        return host;
    }
}

public class SmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public SmokeTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Health_Returns200()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
        var response = await client.GetAsync("/health");
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.OK ||
            response.StatusCode == System.Net.HttpStatusCode.Found ||
            response.StatusCode == System.Net.HttpStatusCode.MovedPermanently,
            $"Expected 2xx or 3xx but got {response.StatusCode}");
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
