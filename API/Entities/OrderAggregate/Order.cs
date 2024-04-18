using System.ComponentModel.DataAnnotations;

namespace API.Entities.OrderAggregate;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    [Required]
    public ShippingAddress ShippingAddress { get; set; }
    public DateTime OrderDate => DateTime.UtcNow;
    public List<OrderItem> OrderItems { get; set; }
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public long GetTotal() => Subtotal + DeliveryFee;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public string PaymentIntentId { get; set; }
}
