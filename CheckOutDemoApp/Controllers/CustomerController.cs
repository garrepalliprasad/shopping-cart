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
    public class CustomerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public CustomerController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateCustomer()
        {
            CustomerViewModel model = new CustomerViewModel();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCustomer(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customerCreateOptions = new CustomerCreateOptions
                {

                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = new AddressOptions
                    {
                        Line1 = model.Address.Line1,
                        Line2 = model.Address.Line2,
                        City = model.Address.City,
                        State = model.Address.State,
                        Country = model.Address.Country,
                        PostalCode = model.Address.PostalCode
                    },
                    Shipping = new ShippingOptions
                    {
                        Name = model.Name,
                        Address = new AddressOptions
                        {
                            Line1 = model.Shipping.Address.Line1,
                            Line2 = model.Shipping.Address.Line2,
                            City = model.Shipping.Address.City,
                            State = model.Shipping.Address.State,
                            Country = model.Shipping.Address.Country,
                            PostalCode = model.Shipping.Address.PostalCode
                        }
                    },
                    Description = model.Description,

                };
                var customerService = new CustomerService();
                var customer = customerService.Create(customerCreateOptions);

                IdentityUser user = new IdentityUser { Id = customer.Id, UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Customer Created Successfully";
                    return View("Success");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ListAllCustomers()
        {
            var customerListOptions = new CustomerListOptions
            {
                Limit = 100,
            };
            var customerService = new CustomerService();
            StripeList<Customer> Customers = await customerService.ListAsync(customerListOptions);
            return View(Customers);
        }
        [HttpGet]
        public IActionResult RetrieveCustomer(string CustomerId)
        {
            var customerService = new CustomerService();

            Customer model = customerService.Get(CustomerId);
            return View(model);
        }
        [HttpGet]
        public IActionResult DeleteCustomer(string CustomerId)
        {
            var customerService = new CustomerService();
            Customer customerDeleted = customerService.Delete(CustomerId);
            if (customerDeleted == null)
            {
                ViewBag.ErrorMessage = "Unable To Delete";
                return View("Error");
            }
            return View(customerDeleted);
        }
        [HttpGet]
        public IActionResult UpdateCustomer(string CustomerId)
        {
            var customerService = new CustomerService();
            Customer model = customerService.Get(CustomerId);
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(Customer model)
        {
            var customerService = new CustomerService();
            if (ModelState.IsValid)
            {
                var customerUpdateOptions = new CustomerUpdateOptions
                {
                    Address = new AddressOptions
                    {
                        Line1 = model.Address.Line1,
                        Line2 = model.Address.Line2
                    },
                    Description = model.Description,
                };
                var customer = customerService.Update(model.Id, customerUpdateOptions);
                if (customer == null)
                {
                    ViewBag.ErrorMessage = "Unable to Update Customer";
                    return View("Error");
                }
            }
            return RedirectToAction("RetrieveCustomer", "Customer", new { CustomerId = model.Id });
        }
    }
}
