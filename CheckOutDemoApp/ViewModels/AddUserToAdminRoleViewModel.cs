using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CheckOutDemoApp.ViewModels
{
    public class AddUserToAdminRoleViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
