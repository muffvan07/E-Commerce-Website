using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ECommerceWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterRetailerModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterRetailerModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IEmailSender _emailSender;

        public RegisterRetailerModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterRetailerModel> logger)
        //IEmailSender emailSender

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Display(Name = "Name")]
            [Required(ErrorMessage = "Please enter Name"), MaxLength(30)]
            public string Name { get; set; }

            [Display(Name = "E-mail")]
            [DataType(DataType.EmailAddress)]
            [Required(ErrorMessage = "Please enter Email ID")]
            [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
            public string Email { get; set; }

            [Display(Name = "Business E-mail")]
            [DataType(DataType.EmailAddress)]
            [Required(ErrorMessage = "Please enter Business Email")]
            [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
            public string BusinessEmail { get; set; }

            [Display(Name = "Username")]
            [Required(ErrorMessage = "Please enter Username"), MaxLength(30)]
            public string Username { get; set; }

            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Please enter Password"), MinLength(6)]
            public string Password { get; set; }

            [Display(Name = "Contact Number")]
            [Required(ErrorMessage = "Please enter Mobile No"), MinLength(10)]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }

            [Display(Name = "Address Line 1")]
            [Required(ErrorMessage = "Please enter Address"), MaxLength(70)]
            public string Address1 { get; set; }

            [Display(Name = "Address Line 2")]
            [Required(ErrorMessage = "Please enter Address"), MaxLength(70)]
            public string Address2 { get; set; }
            [Display(Name = "City")]
            [Required(ErrorMessage = "Please enter City"), MaxLength(10)]
            public string City { get; set; }

            [Display(Name = "State")]
            [Required(ErrorMessage = "Please enter State"), MaxLength(10)]
            public string State { get; set; }

            [Display(Name = "Zipcode")]
            [Required(ErrorMessage = "Please enter Zipcode"), MinLength(6)]
            public string Zipcode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new RegistrationDetails
                {
                    Name = Input.Name,
                    UserName = Input.Username,
                    BusinessEmail = Input.BusinessEmail,
                    Email = Input.Email,
                    Address1 = Input.PhoneNumber,
                    Address2 = Input.Address2,
                    City = Input.City,
                    State = Input.State,
                    Zipcode = Input.Zipcode,
                    PhoneNumber = Input.PhoneNumber

                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }
                    if (!await _roleManager.RoleExistsAsync("Retailer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Retailer"));
                    }

                    await _userManager.AddToRoleAsync(user, "Retailer");

                    _logger.LogInformation("User created a new account with password.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        user.LockoutEnabled = false;
                        user.LockoutEnd = null;
                        await _userManager.UpdateAsync(user);
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
