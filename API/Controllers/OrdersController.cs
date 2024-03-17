using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly StoreContext _context;
    public OrdersController(StoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(x => x.BuyerId == User.Identity.Name)
                .ToListAsync();
    }


    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        return await _context.Orders
                    .ProjectOrderToOrderDto()
                    .Where(x => x.BuyerId == User.Identity.Name && x.Id == id)
                    .FirstOrDefaultAsync();
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto)
    {
        var basket = await _context.Baskets
            .RetrieveBasketWithItems(User.Identity.Name)
            .FirstOrDefaultAsync();

        if (basket == null)
        {
            return BadRequest(new ProblemDetails { Title = "could not find basket" });
        }

        var items = new List<OrderItem>();
        foreach (var basketItem in basket.Items)
        {
            var dbProduct = await _context.Products.FindAsync(basketItem.ProductId);

            var orderItem = new OrderItem
            {
                ItemOrdered = new ProductItemOrdered
                {
                    ProductId = dbProduct.Id,
                    Name = dbProduct.Name,
                    PictureUrl = dbProduct.PictureUrl
                },
                Price = dbProduct.Price,
                Quantity = basketItem.Quantity
            };

            items.Add(orderItem);
            dbProduct.QuantityInStock -= basketItem.Quantity;
        }

        var subTotal = items.Sum(x => x.Price * x.Quantity);
        var deliveryFee = subTotal > 10000 ? 0 : 500;

        var order = new Order
        {
            OrderItems = items,
            BuyerId = User.Identity.Name,
            ShippingAddress = orderDto.ShippingAddress,
            Subtotal = subTotal,
            DeliveryFee = deliveryFee,
        };

        _context.Orders.Add(order);
        _context.Baskets.Remove(basket);
        if (orderDto.SaveAddress)
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var address = new UserAddress
            {
                FullName = orderDto.ShippingAddress.FullName,
                Address1 = orderDto.ShippingAddress.Address1,
                Address2 = orderDto.ShippingAddress.Address2,
                City = orderDto.ShippingAddress.City,
                State = orderDto.ShippingAddress.State,
                Country = orderDto.ShippingAddress.Country,
                Zip = orderDto.ShippingAddress.Zip,
            };

            user.Address = address;
        }

        var result = await _context.SaveChangesAsync() > 0;
        if (result)
        {
            return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);
        }

        return BadRequest(new ProblemDetails { Title = "could not create order" });
    }
}
