using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Repos;

namespace Services.Services
{
  public class OrderService : IOrderService
  {
    private readonly IOrderRepository _orders;
    private readonly ITicketRepository _tickets;
    private readonly ICustomerRepository _customers;

    public OrderService(IOrderRepository orders, ITicketRepository tickets, ICustomerRepository customers)
    {
      _orders = orders;
      _tickets = tickets;
      _customers = customers;
    }

    public async Task<List<OrderReadDto>> GetAllAsync(CancellationToken ct = default)
    {
      var entities = await _orders.GetAll().ToListAsync(ct);
      return entities.Select(Mapper.Map).ToList();
    }

    public async Task<OrderReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _orders.GetById(id).FirstOrDefaultAsync(ct);
      return e is null ? null : Mapper.Map(e);
    }

    public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default)
    {
      if (dto.TicketIds is null || dto.TicketIds.Count == 0)
        throw new ValidationException("At least one ticket is required.");

      if (!await _customers.ExistsAsync(dto.CustomerId, ct))
        throw new NotFoundException($"Customer {dto.CustomerId} not found.");

      var tickets = await _tickets.GetByIdsWithPrice(dto.TicketIds).ToListAsync(ct);
      if (tickets.Count != dto.TicketIds.Count)
        throw new NotFoundException("One or more tickets not found.");
      if (tickets.Any(t => t.Status != "available"))
        throw new ConflictException("One or more tickets are not available.");

      var total = tickets.Sum(t => t.PriceTier!.Amount);

      var order = new Order
      {
        OrderId = Guid.NewGuid(),
        CustomerId = dto.CustomerId,
        CreatedAt = DateTime.UtcNow,
        TotalAmount = total,
        OrderTickets = tickets.Select(t => new OrderTicket
        {
          OrderId = Guid.Empty, // set after we have order id
          TicketId = t.TicketId
        }).ToList()
      };
      foreach (var ot in order.OrderTickets) ot.OrderId = order.OrderId;

      await _orders.AddAsync(order, ct);          // persists order + join rows
      await _tickets.MarkSoldAsync(tickets, ct);  // sets Status = "sold"

      // reload with includes for mapping
      var saved = await _orders.GetById(order.OrderId).FirstAsync(ct);
      return Mapper.Map(saved);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        _orders.DeleteAsync(id, ct);
  }
}
