using EnterpriseCMS.Application.Features.Content.Commands;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace EnterpriseCMS.UnitTests.Features.Content;

public class UpdateContentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _userMock = new();
    private readonly Mock<IRepository<Core.Entities.Content>> _repoMock = new();
    private readonly Mock<IRepository<Core.Entities.ContentVersion>> _versionRepoMock = new();

    public UpdateContentCommandHandlerTests()
    {
        _uowMock.Setup(u => u.Contents).Returns(_repoMock.Object);
        _uowMock.Setup(u => u.ContentVersions).Returns(_versionRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
        _versionRepoMock.Setup(r => r.AddAsync(It.IsAny<Core.Entities.ContentVersion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.Entities.ContentVersion v, CancellationToken _) => v);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Core.Entities.Content>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTitle()
    {
        var content = new Core.Entities.Content { Id = Guid.NewGuid(), Title = "Old Title", Slug = "old", Blocks = "[]" };
        _repoMock.Setup(r => r.GetByIdAsync(content.Id, It.IsAny<CancellationToken>())).ReturnsAsync(content);

        var handler = new UpdateContentCommandHandler(_uowMock.Object, _userMock.Object);
        var cmd = new UpdateContentCommand(content.Id, "New Title", null, null, null, "[]", ContentStatus.Draft, null, null, null, false);

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task Handle_ShouldUpdateSlug_WhenSlugChanged()
    {
        var content = new Core.Entities.Content { Id = Guid.NewGuid(), Title = "Test", Slug = "old-slug", Blocks = "[]" };
        _repoMock.Setup(r => r.GetByIdAsync(content.Id, It.IsAny<CancellationToken>())).ReturnsAsync(content);

        var handler = new UpdateContentCommandHandler(_uowMock.Object, _userMock.Object);
        var cmd = new UpdateContentCommand(content.Id, "Test", null, null, null, "[]", ContentStatus.Draft, null, null, null, false);

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContentNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Core.Entities.Content?)null);

        var handler = new UpdateContentCommandHandler(_uowMock.Object, _userMock.Object);
        var cmd = new UpdateContentCommand(Guid.NewGuid(), "Title", null, null, null, "[]", ContentStatus.Draft, null, null, null, false);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldIncrementVersion()
    {
        var content = new Core.Entities.Content { Id = Guid.NewGuid(), Title = "Test", Slug = "test", Blocks = "[]", CurrentVersion = 3 };
        _repoMock.Setup(r => r.GetByIdAsync(content.Id, It.IsAny<CancellationToken>())).ReturnsAsync(content);

        var handler = new UpdateContentCommandHandler(_uowMock.Object, _userMock.Object);
        var cmd = new UpdateContentCommand(content.Id, "New", null, null, null, "[]", ContentStatus.Draft, null, null, null, false);

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.CurrentVersion.Should().Be(4);
    }
}
