namespace Repositories.Repos
{
  public class OrderReadDto
  {
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<TicketReadDto> Tickets { get; set; } = new();
  }
}
