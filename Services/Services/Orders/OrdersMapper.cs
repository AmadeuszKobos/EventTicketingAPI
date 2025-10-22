using Domain;

namespace Repositories.Repos
{
  public partial class Mapper
  {
    public static OrderReadDto Map(Order o) => new()
    {
      OrderId = o.OrderId,
      CustomerId = o.CustomerId,
      CustomerName = o.Customer?.FullName ?? "",
      CreatedAt = o.CreatedAt,
      TotalAmount = o.TotalAmount,
      Tickets = o.OrderTickets.Select(ot => MapTicket(ot.Ticket!)).ToList()
    };
  }
}