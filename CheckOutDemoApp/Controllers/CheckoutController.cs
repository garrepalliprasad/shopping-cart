using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace CheckOutDemoApp.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public CheckoutController(SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index(string PriceId)
        {
            var customerId=_userManager.GetUserId(User);
            var options = new SessionCreateOptions
            {
                Customer = customerId,
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },  
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = PriceId,
                        Quantity = 1,
                    },
                    //new SessionLineItemOptions
                    //{
                    //    Price = "price_1H3vKSGulqmqriPwTgHBob5Z",
                    //    Quantity = 10,
                    //},
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:44354/success",
                CancelUrl = "https://localhost:44354/error",
            };
            
            var service = new SessionService();
            var session = service.Create(options);
            ViewData["SessionID"] = session.Id;
            return View();
        }
    }
}
