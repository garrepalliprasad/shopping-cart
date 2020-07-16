using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckOutDemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CheckOutDemoApp.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Role Created Successfully";
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
        [Authorize(Roles ="Shopadmin")]
        public IActionResult AddUserToAdminRole()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Shopadmin")]
        public async Task<IActionResult> AddUserToAdminRole(AddUserToAdminRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user!=null)
                {
                    var result=await _userManager.AddToRoleAsync(user, "Shopadmin");
                    if (result.Succeeded)
                    {
                        ViewBag.SuccessMessage = "The User is Added to Admin Role";
                        return View("Success");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Given Mail is not Registered Try with Other");
                }
            }
            return View(model);
        }
    }
}
