using Accordly.Application.Agreements.Queries.ListAgreements;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class ListAgreementsQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_MapsVisibleAgreementsToResponses()
    {
        var userId = Guid.NewGuid();
        var agreements = new[]
        {
            new Agreement { Title = "First", OwnerId = userId },
            new Agreement { Title = "Second", OwnerId = Guid.NewGuid() }
        };
        var repository = new Mock<IAgreementRepository>();
        repository.Setup(item => item.GetAllForUserAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(agreements);
        var handler = new ListAgreementsQueryHandler(repository.Object);

        var result = await handler.Handle(new ListAgreementsQuery(userId), CancellationToken.None);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("First", result[0].Title);
        Assert.AreEqual("Second", result[1].Title);
        repository.Verify(item => item.GetAllForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}