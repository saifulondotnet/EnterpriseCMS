using EnterpriseCMS.Application.Features.Auth.Commands;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace EnterpriseCMS.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    public LoginCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsValid()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", IsActive = true };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Editor" });
        _tokenServiceMock.Setup(t => t.GenerateTokensAsync(user)).ReturnsAsync(("access-token", "refresh-token"));

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object);
        var result = await handler.Handle(new LoginCommand("test@test.com", "password"), CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new LoginCommand("bad@test.com", "password"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", IsActive = true };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object);
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(new LoginCommand("test@test.com", "wrong"), CancellationToken.None));
    }
}
