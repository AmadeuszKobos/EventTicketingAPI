namespace Repositories.Repos
{
  public class TicketReadDto
  {
    public Guid TicketId { get; set; }
    public string Code { get; set; } = default!;
    public string Status { get; set; } = default!;
    public Guid EventId { get; set; }
    public string EventName { get; set; } = "";
    public string TierName { get; set; } = "";
    public decimal Amount { get; set; }
  }
}