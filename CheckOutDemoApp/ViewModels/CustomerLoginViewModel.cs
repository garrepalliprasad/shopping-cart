using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Stripe;

namespace CheckOutDemoApp.ViewModels
{
    public class CustomerLoginViewModel:Customer
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
