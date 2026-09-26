namespace Infra;

public class Order
{
    public Guid Id { get; set; }
    public required string Reference { get; set; }
}