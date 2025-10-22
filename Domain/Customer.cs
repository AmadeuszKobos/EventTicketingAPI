namespace Domain
{
  public class Customer
  {
    public Guid CustomerId { get; set; }

    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
  }
}
