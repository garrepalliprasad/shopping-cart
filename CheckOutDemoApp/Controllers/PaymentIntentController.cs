using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckOutDemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CheckOutDemoApp.Controllers
{
    [Authorize]
    public class PaymentIntentController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public PaymentIntentController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult CreatePaymentIntent(string priceId)
        {
            var customerId = _userManager.GetUserId(User);
            var customerService = new CustomerService();
            var customer = customerService.Get(customerId);
            var priceService = new PriceService();
            var price = priceService.Get(priceId);
            PaymentIntentViewModel viewModel = new PaymentIntentViewModel
            {
                Price = price,
                Customer = customer
            };
            //var paymentIntentOptions = new PaymentIntentCreateOptions
            //{
            //    Amount = (long?)model.UnitAmountDecimal,
            //    Currency = "inr",
            //    PaymentMethodTypes = new List<string>
            //        {
            //        "card",
            //        },
            //    Customer = customer.Id,

            //};
            //var paymentIntentService = new PaymentIntentService();
            //var paymentIntent = paymentIntentService.Create(paymentIntentOptions);

            //ViewData["ClientSecret"] = paymentIntent.ClientSecret;

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult CreatePaymentIntent(PaymentIntentViewModel model)
        {

            if (ModelState.IsValid)
            {
                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long?)model.Price.UnitAmountDecimal,
                    Currency = "inr",
                    PaymentMethodTypes = new List<string>
                    {
                      "card",
                    },
                    Customer = model.Customer.Id,

                };
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(paymentIntentOptions);

                //ViewData["ClientSecret"] = paymentIntent.ClientSecret;
                var paymentMethodCreateOptions = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardCreateOptions
                    {
                        Number = model.CardNumber,
                        ExpMonth = model.ExpMonth,
                        ExpYear = model.ExpYear,
                        Cvc = model.Cvc,
                    },
                };
                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = paymentMethodService.Create(paymentMethodCreateOptions);
                //model.PaymentMethod = paymentMethod;
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethod.Id,
                };
                var service = new PaymentIntentService();
                service.Confirm(paymentIntent.Id, options);
                ViewBag.SuccessMessage = "Your Payment is Successfull!.....";

                return View("Success");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ListAllPaymentIntents()
        {

            var paymentIntentListOptions = new PaymentIntentListOptions
            {
                Limit = 100,
                Customer = _userManager.GetUserId(User)
            };
            var service = new PaymentIntentService();
            StripeList<PaymentIntent> paymentIntents = service.List(
              paymentIntentListOptions
            );

            return View(paymentIntents);
        }
    }
}
