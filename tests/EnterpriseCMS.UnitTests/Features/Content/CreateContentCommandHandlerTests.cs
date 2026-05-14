using EnterpriseCMS.Application.Features.Content.Commands;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace EnterpriseCMS.UnitTests.Features.Content;

public class CreateContentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _userMock = new();
    private readonly Mock<ISlugService> _slugMock = new();
    private readonly Mock<IRepository<Core.Entities.Content>> _repoMock = new();

    public CreateContentCommandHandlerTests()
    {
        _uowMock.Setup(u => u.Contents).Returns(_repoMock.Object);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Core.Entities.Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.Entities.Content c, CancellationToken _) => c);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _userMock.Setup(u => u.TenantId).Returns(Guid.NewGuid());
        _slugMock.Setup(s => s.GenerateUniqueAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<bool>>>()))
            .ReturnsAsync((string slug, Func<string, Task<bool>> _) => slug.ToLowerInvariant().Replace(" ", "-"));
    }

    [Fact]
    public async Task Handle_ShouldCreateContent_WithExpectedTitle()
    {
        var handler = new CreateContentCommandHandler(_uowMock.Object, _userMock.Object, _slugMock.Object);
        var command = new CreateContentCommand(
            "My Test Page", null, null, "<p>Hello</p>", "[]", "page",
            ContentStatus.Draft, null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("My Test Page");
        result.ContentType.Should().Be("page");
        result.Status.Should().Be(ContentStatus.Draft);
    }

    [Fact]
    public async Task Handle_ShouldUseSlugFromTitle_WhenNoSlugProvided()
    {
        var handler = new CreateContentCommandHandler(_uowMock.Object, _userMock.Object, _slugMock.Object);
        var command = new CreateContentCommand(
            "My New Page", null, null, null, "[]", "page",
            ContentStatus.Draft, null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Slug.Should().Be("my-new-page");
    }

    [Fact]
    public async Task Handle_ShouldPersistContent_ToRepository()
    {
        var handler = new CreateContentCommandHandler(_uowMock.Object, _userMock.Object, _slugMock.Object);
        var command = new CreateContentCommand(
            "Test", "test", null, null, "[]", "page",
            ContentStatus.Published, null, null, null, null, null);

        await handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Core.Entities.Content>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
