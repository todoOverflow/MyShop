using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        var basket = await RetrieveBasket(GetBuyerId());
        if (basket == null) return NotFound();

        return basket.MapBasketToDto();
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItem(int productId, int quantity)
    {
        var basket = await RetrieveBasket(GetBuyerId());
        basket ??= await CreateBasket();

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        basket.AddItem(product, quantity);
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return CreatedAtRoute("GetBasket", basket.MapBasketToDto());

        return BadRequest(new ProblemDetails { Title = "Probelm saving item to basket" });
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItem(int productId, int quantity)
    {
        var basket = await RetrieveBasket(GetBuyerId());
        if (basket == null) return NotFound();

        if (!basket.FindItem(productId, out var item)) return BadRequest(
            new ProblemDetails { Title = "Product Not Found" }
        );

        basket.RemoveItem(productId, quantity);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest(new ProblemDetails { Title = "Probelm removing item from basket" });
    }

    private async Task<Basket> RetrieveBasket(string buyerId = null)
    {
        if (string.IsNullOrEmpty(buyerId))
        {
            Response.Cookies.Delete("buyerId");
            return null;
        }
        return await _context.Baskets
        .Include(x => x.Items)
        .ThenInclude(x => x.Product)
        .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
    }

    private string GetBuyerId()
    {
        return User.Identity?.Name ?? Request.Cookies["buyerId"];
    }

    private async Task<Basket> CreateBasket()
    {
        var buyerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(buyerId))
        {
            buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.Now.AddDays(30)
            };
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
        }

        var basket = new Basket
        {
            BuyerId = buyerId
        };
        await _context.Baskets.AddAsync(basket);
        return basket;
    }
}
