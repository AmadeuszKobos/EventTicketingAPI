namespace Repositories.Repos
{
  public class TicketCreateDto
  {
    public Guid EventId { get; set; }
    public Guid PriceTierId { get; set; }
    public string Code { get; set; } = default!;
  }
}