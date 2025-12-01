using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class CreateOrderRequest
{
    [Required]
    public List<OrderItem> Items { get; set; }
}

public class OrderItem
{
    [Required]
    public string? ProductId { get; set; }

    [Required]
    public int? Quantity { get; set; }
}
