namespace Repositories.Repos
{
  public class OrderCreateDto
  {
    public Guid CustomerId { get; set; }
    public List<Guid> TicketIds { get; set; } = new();
  }
}
