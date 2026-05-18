using EnterpriseCMS.Application.Features.Content.Commands;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace EnterpriseCMS.UnitTests.Features.Content;

public class PublishContentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _userMock = new();
    private readonly Mock<IRepository<Core.Entities.Content>> _repoMock = new();

    public PublishContentCommandHandlerTests()
    {
        _uowMock.Setup(u => u.Contents).Returns(_repoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_ShouldSetStatusToPublished()
    {
        var content = new Core.Entities.Content { Id = Guid.NewGuid(), Title = "Test", Status = ContentStatus.Draft };
        _repoMock.Setup(r => r.GetByIdAsync(content.Id, It.IsAny<CancellationToken>())).ReturnsAsync(content);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Core.Entities.Content>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new PublishContentCommandHandler(_uowMock.Object, _userMock.Object);
        var result = await handler.Handle(new PublishContentCommand(content.Id), CancellationToken.None);

        result.Status.Should().Be(ContentStatus.Published);
    }

    [Fact]
    public async Task Handle_ShouldSetPublishedAt_WhenPublishing()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var content = new Core.Entities.Content { Id = Guid.NewGuid(), Title = "Test", Status = ContentStatus.Draft };
        _repoMock.Setup(r => r.GetByIdAsync(content.Id, It.IsAny<CancellationToken>())).ReturnsAsync(content);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Core.Entities.Content>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new PublishContentCommandHandler(_uowMock.Object, _userMock.Object);
        await handler.Handle(new PublishContentCommand(content.Id), CancellationToken.None);

        content.PublishedAt.Should().BeAfter(before);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenContentNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Core.Entities.Content?)null);

        var handler = new PublishContentCommandHandler(_uowMock.Object, _userMock.Object);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new PublishContentCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
