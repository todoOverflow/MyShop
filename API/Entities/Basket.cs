namespace API.Entities;

public class Basket
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; } = [];

    public void AddItem(Product product, int Quantity)
    {

        if (FindItem(product.Id, out var item))
        {
            item.Quantity += Quantity;
        }
        else
        {
            Items.Add(new BasketItem
            {
                Product = product,
                Quantity = Quantity,
            });
        }
    }

    public void RemoveItem(int productId, int Quantity)
    {
        if (FindItem(productId, out var item))
        {
            item.Quantity -= Quantity;
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
            }
        }
    }

    public bool FindItem(int productId, out BasketItem item)
    {
        item = Items.FirstOrDefault(x => x.ProductId == productId);
        return item != null;
    }
}