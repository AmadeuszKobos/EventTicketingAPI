namespace Domain
{
  public class PriceTier
  {
    public Guid PriceTierId { get; set; }
    public Guid EventId { get; set; }
    public string TierName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public Event? Event { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
  }

}
