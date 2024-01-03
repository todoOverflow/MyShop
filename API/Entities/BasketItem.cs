using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("BasketItems")]
public class BasketItem
{
    //auto generated
    public int Id { get; set; }
    public int Quantity { get; set; }

    //navigation property
    public int ProductId { get; set; }
    public Product Product { get; set; }

    //navigation property
    public int BasketId { get; set; }
    public Basket Basket { get; set; }
}
