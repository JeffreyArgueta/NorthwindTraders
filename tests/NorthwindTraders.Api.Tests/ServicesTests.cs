using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using NorthwindTraders.Application.Services;
using NorthwindTraders.Domain.Contracts;
using FluentAssertions;

namespace NorthwindTraders.Api.Tests;

public class ServicesTests
{
    [Fact]
    public async Task OrderService_GetAll_ForwardsToRepository()
    {
        var repo = new Mock<IOrderRepository>();
        repo.Setup(r => r.GetAllAsync(1, 20, It.IsAny<CancellationToken>())).ReturnsAsync(new PagedResult<OrderDto>(new[] { new OrderDto(1, null, null, null, null, null, null, 0m, null, null, null, null, null) }, 1, 1, 20));
        var svc = new OrderService(repo.Object);
        var result = await svc.GetAllAsync(1, 20, CancellationToken.None);
        result.TotalCount.Should().Be(1);
    }
}
