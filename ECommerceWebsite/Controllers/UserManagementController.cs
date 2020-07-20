using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceWebsite.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    [AuthorizeRoles("Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> Activate(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> Deactivate(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            user.LockoutEnabled = true;
            user.LockoutEnd = new DateTime(2999, 12, 31);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("UserList");
        }
    }
}