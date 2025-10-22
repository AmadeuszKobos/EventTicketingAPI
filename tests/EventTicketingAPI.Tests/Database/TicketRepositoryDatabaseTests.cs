using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Repos;
using Testcontainers.PostgreSql;
using Xunit;

namespace EventTicketingAPI.Tests.Database;

public class TicketRepositoryDatabaseTests : IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
    .WithImage("postgres:16-alpine")
    .Build();

  private AppDbContext _context = null!;

  public async Task InitializeAsync()
  {
    await _postgres.StartAsync();

    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseNpgsql(_postgres.GetConnectionString())
      .Options;

    _context = new AppDbContext(options);
    await _context.Database.MigrateAsync();
  }

  public async Task DisposeAsync()
  {
    if (_context is not null)
    {
      await _context.DisposeAsync();
    }

    await _postgres.DisposeAsync();
  }

  [Fact]
  public async Task MarkSoldAsync_UpdatesTicketStatusWithinTransaction()
  {
    // Arrange
    await using var transaction = await _context.Database.BeginTransactionAsync();

    var venue = new Venue
    {
      VenueId = Guid.NewGuid(),
      Name = "Integration Hall",
      City = "Gotham",
      Capacity = 1000
    };

    var @event = new Event
    {
      EventId = Guid.NewGuid(),
      VenueId = venue.VenueId,
      Name = "Sample Show",
      StartTime = DateTime.UtcNow,
      EndTime = DateTime.UtcNow.AddHours(2)
    };

    var tier = new PriceTier
    {
      PriceTierId = Guid.NewGuid(),
      EventId = @event.EventId,
      TierName = "Floor",
      Amount = 75m
    };

    var ticket = new Ticket
    {
      TicketId = Guid.NewGuid(),
      EventId = @event.EventId,
      PriceTierId = tier.PriceTierId,
      Code = "INT-001"
    };

    _context.Venues.Add(venue);
    _context.Events.Add(@event);
    _context.PriceTiers.Add(tier);
    _context.Tickets.Add(ticket);
    await _context.SaveChangesAsync();

    var repo = new TicketRepository(_context);

    // Act
    var tickets = await repo.GetByIdsWithPrice(new[] { ticket.TicketId }).ToListAsync();
    await repo.MarkSoldAsync(tickets, CancellationToken.None);

    // Assert
    Assert.Single(tickets);
    Assert.Equal(75m, tickets[0].PriceTier!.Amount);

    var reloaded = await _context.Tickets.AsNoTracking().FirstAsync(t => t.TicketId == ticket.TicketId);
    Assert.Equal("sold", reloaded.Status);

    // Cleanup
    await transaction.RollbackAsync();
  }
}
