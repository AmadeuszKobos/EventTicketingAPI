using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventTicketingAPI.Tests.Infrastructure;
using Moq;
using Repositories.Repos;
using Services.Services;
using Xunit;

namespace EventTicketingAPI.Tests.Services;

public class OrderServiceTests
{
  [Fact]
  public async Task CreateAsync_WithAvailableTickets_ComputesTotalAndPersistsOrder()
  {
    // Arrange
    var customerId = Guid.NewGuid();
    var ticketIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

    var priceTiers = new[]
    {
      new PriceTier { PriceTierId = Guid.NewGuid(), Amount = 25m, TierName = "General" },
      new PriceTier { PriceTierId = Guid.NewGuid(), Amount = 40m, TierName = "VIP" }
    };

    var tickets = ticketIds.Select((id, index) => new Ticket
    {
      TicketId = id,
      Code = $"TKT-{index}",
      Status = "available",
      PriceTierId = priceTiers[index].PriceTierId,
      PriceTier = priceTiers[index]
    }).ToList();

    var customers = new Mock<ICustomerRepository>(MockBehavior.Strict);
    customers
      .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(true);

    var ticketsRepo = new Mock<ITicketRepository>(MockBehavior.Strict);
    ticketsRepo
      .Setup(r => r.GetByIdsWithPrice(It.IsAny<IEnumerable<Guid>>()))
      .Returns<IEnumerable<Guid>>(ids =>
        new TestAsyncEnumerable<Ticket>(tickets.Where(t => ids.Contains(t.TicketId))));
    ticketsRepo
      .Setup(r => r.MarkSoldAsync(It.Is<IEnumerable<Ticket>>(ts => ts.All(t => ticketIds.Contains(t.TicketId))), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask)
      .Verifiable();

    var savedOrder = new Order
    {
      OrderId = Guid.Empty,
      CustomerId = customerId,
      Customer = new Customer { CustomerId = customerId, FullName = "Ada Lovelace", Email = "ada@example.test", Phone = "123" },
      CreatedAt = DateTime.UtcNow,
      TotalAmount = tickets.Sum(t => t.PriceTier!.Amount),
      OrderTickets = tickets.Select(t => new OrderTicket
      {
        OrderId = Guid.Empty,
        TicketId = t.TicketId,
        Ticket = t
      }).ToList()
    };

    var ordersRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
    ordersRepo
      .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
      .Callback<Order, CancellationToken>((order, _) =>
      {
        savedOrder.OrderId = order.OrderId;
        savedOrder.CreatedAt = order.CreatedAt;
        foreach (var ot in savedOrder.OrderTickets) ot.OrderId = order.OrderId;
      })
      .Returns(Task.CompletedTask)
      .Verifiable();

    ordersRepo
      .Setup(r => r.GetById(It.Is<Guid>(id => id == savedOrder.OrderId || savedOrder.OrderId == Guid.Empty)))
      .Returns<Guid>(_ => new TestAsyncEnumerable<Order>(new[] { savedOrder }));

    var sut = new OrderService(ordersRepo.Object, ticketsRepo.Object, customers.Object);

    var dto = new OrderCreateDto { CustomerId = customerId, TicketIds = ticketIds.ToList() };

    // Act
    var result = await sut.CreateAsync(dto, CancellationToken.None);

    // Assert
    ordersRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    ticketsRepo.Verify();
    Assert.Equal(savedOrder.OrderId, result.OrderId);
    Assert.Equal(tickets.Sum(t => t.PriceTier!.Amount), result.TotalAmount);
    Assert.Equal(tickets.Count, result.Tickets.Count);
    Assert.Equal("Ada Lovelace", result.CustomerName);
  }
}