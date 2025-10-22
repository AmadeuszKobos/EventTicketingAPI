using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<PriceTier> PriceTiers => Set<PriceTier>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderTicket> OrderTickets => Set<OrderTicket>();

    protected override void OnModelCreating(ModelBuilder m)
    {
      m.Entity<OrderTicket>().HasKey(x => new { x.OrderId, x.TicketId });

      m.Entity<Venue>().HasIndex(v => new { v.Name, v.City }).IsUnique();
      m.Entity<PriceTier>().HasIndex(p => new { p.EventId, p.TierName }).IsUnique();
      m.Entity<Ticket>().HasIndex(t => new { t.EventId, t.Status });
      m.Entity<Ticket>().HasIndex(t => t.Code).IsUnique();
      m.Entity<Order>().HasIndex(o => new { o.CustomerId, o.CreatedAt });

      // quote identifiers to match PascalCase column names
      m.Entity<Venue>()
       .ToTable(tb => tb.HasCheckConstraint("ck_venue_capacity_pos", "\"Capacity\" > 0"));

      m.Entity<Event>()
       .ToTable(tb => tb.HasCheckConstraint("ck_event_time", "\"EndTime\" > \"StartTime\""));

      m.Entity<PriceTier>()
       .ToTable(tb => tb.HasCheckConstraint("ck_price_amount_nonneg", "\"Amount\" >= 0"));

      m.Entity<Ticket>()
       .ToTable(tb => tb.HasCheckConstraint("ck_ticket_status",
           "\"Status\" IN ('available','reserved','sold')"));
    }
  }
}