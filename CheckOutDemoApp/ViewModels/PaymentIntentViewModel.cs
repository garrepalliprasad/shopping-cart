using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Stripe;

namespace CheckOutDemoApp.ViewModels
{
    public class PaymentIntentViewModel
    {
        public Price Price { get; set; }
        public Customer Customer { get; set; }
        [Required]
        [MaxLength(16,ErrorMessage ="16 Digits required")]
        public string CardNumber { get; set; }
        [Required]
        [Range(1,12,ErrorMessage ="Month Should be 1-12")]
        public int ExpMonth { get; set; }
        [Required]
        [Range(2020,2030,ErrorMessage ="Invalid Year")]
        public int ExpYear { get; set; }
        [Required]
        [MaxLength(3,ErrorMessage="3 digit CVC")]
        public string Cvc { get; set; }
    }
}
