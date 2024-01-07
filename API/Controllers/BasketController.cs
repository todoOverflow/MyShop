using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BasketController : BaseApiController
{
    private readonly StoreContext _context;
    public BasketController(StoreContext Context)
    {
        _context = Context;
    }

    [HttpGet(Name = "GetBasket")]
    public async Task<ActionResult<BasketDto>> GetBasket()
    {
        var basket = await RetrieveBasket();
        if (basket == null) return NotFound();

        return MapBasketToDto(basket);
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItem(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        basket ??= await CreateBasket();

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        basket.AddItem(product, quantity);
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return CreatedAtRoute("GetBasket", MapBasketToDto(basket));

        return BadRequest(new ProblemDetails { Title = "Probelm saving item to basket" });
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItem(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        if (basket == null) return NotFound();

        if (!basket.FindItem(productId, out var item)) return BadRequest(
            new ProblemDetails { Title = "Product Not Found" }
        );

        basket.RemoveItem(productId, quantity);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest(new ProblemDetails { Title = "Probelm removing item from basket" });
    }

    private async Task<Basket> RetrieveBasket()
    {
        return await _context.Baskets
        .Include(x => x.Items)
        .ThenInclude(x => x.Product)
        .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
    }

    private async Task<Basket> CreateBasket()
    {
        var buyerId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.Now.AddDays(30)
        };
        Response.Cookies.Append("buyerId", buyerId, cookieOptions);
        var basket = new Basket
        {
            BuyerId = buyerId
        };
        await _context.Baskets.AddAsync(basket);
        return basket;
    }

    private static BasketDto MapBasketToDto(Basket basket)
    {
        return new BasketDto
        {
            Id = basket.Id,
            BuyerId = basket.BuyerId,
            Items = basket.Items.Select(
                x => new BasketItemDto
                {
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    PictureUrl = x.Product.PictureUrl,
                    Type = x.Product.Type,
                    Brand = x.Product.Brand,
                    Quantity = x.Quantity
                }
            ).ToList()
        };
    }

}
