using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckOutDemoApp.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IFormFile ProductImage { get; set; }
    }
}
