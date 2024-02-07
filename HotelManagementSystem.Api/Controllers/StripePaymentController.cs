using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DataModels;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace HotelManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StripePaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StripePaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create(StripePaymentDTO payment)
        {
            try
            {
                var domain = _configuration["WasmUrl"];

                var options = new SessionCreateOptions
                {
                    //Customer = payment.UserId,
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = payment.Amount * 100, // to convert cent
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = payment.ProductName,
                                    //Images = new List<string>()
                                    //{
                                    //    payment.ImageUrl
                                    //}
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = domain + "/success-payment?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = domain + "/failed-payment",
                };
                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new SuccessModel()
                {
                    Data = session.Id
                });
            }
            catch (StripeException e)
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = e.Message
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = e.Message
                });
            }
        }
    }
}
