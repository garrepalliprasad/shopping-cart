using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CheckOutDemoApp.Controllers
{
    public class PriceController : Controller
    {
        [HttpGet]
        public IActionResult CreatePrice(string ProductId)
        {
            var model = new Price();
            model.ProductId = ProductId;
            return View(model);
        }
        [HttpPost]
        public IActionResult CreatePrice(Price model)
        {
            if (ModelState.IsValid)
            {
                var priceCreateOptions = new PriceCreateOptions
                {
                    UnitAmountDecimal = model.UnitAmountDecimal,
                    Currency = model.Currency,
                    Product = model.ProductId,
                    BillingScheme = "per_unit",           
                    Nickname="This is Limited Period Price"
                };
                var priceService = new PriceService();
                var price = priceService.Create(priceCreateOptions);
                ViewBag.SuccessMessage = "Product With Given Price Created Successfully";
                return View("Success");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ListAllPrices()
        {
            var priceService = new PriceService();            
            StripeList<Price> prices = priceService.List();
            return View(prices);
        }
        [HttpGet]
        public IActionResult UpdatePrice(string PriceId)
        {
            var priceService = new PriceService();
            var price=priceService.Get(PriceId);
            return View(price);
        }
        [HttpPost]
        public IActionResult UpdatePrice(Price model)
        {
            var priceUpdateOptions = new PriceUpdateOptions
            {
                Nickname=model.Nickname,   
                
            };
            var priceServie = new PriceService();
            var price=priceServie.Update(model.Id, priceUpdateOptions);
            if (price == null)
            {
                ViewBag.ErrorMessage = "Cannot update price";
                return View("Error");
            }
            return RedirectToAction("RetrievePrice", new { PriceId = model.Id });
        }
        [HttpGet]
        public IActionResult RetrievePrice(string PriceId)
        {
            var priceGetOptions = new PriceGetOptions();
            priceGetOptions.AddExpand("product");
            var priceService = new PriceService();
            var price=priceService.Get(PriceId,priceGetOptions);
            
            return View(price);
        }
    }
}
