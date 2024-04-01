using API.Entities;
using Stripe;

namespace API.Services;

public class PaymentService
{
    private readonly IConfiguration _config;
    public PaymentService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<PaymentIntent> CreatOrUpdatePaymentIntent(Basket basket)
    {
        StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

        var service = new PaymentIntentService();
        var intent = new PaymentIntent();
        var subtotal = basket.Items.Sum(x => x.Quantity * x.Product.Price);
        var deliveryFee = subtotal > 10000 ? 0 : 500;

        if (string.IsNullOrWhiteSpace(basket.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = subtotal + deliveryFee,
                Currency = "aud",
                PaymentMethodTypes = ["card"]
            };

            intent = await service.CreateAsync(options);
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = subtotal + deliveryFee
            };
            intent = await service.UpdateAsync(basket.PaymentIntentId, options);
        }
        return intent;
    }
}
