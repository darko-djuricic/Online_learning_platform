using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApplication5.Areas.Identity.Pages.Account.Manage
{
    public class UpgradeModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UpgradeModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (User.IsInRole("Teacher"))
            {
                StatusMessage = "Already upgraded!";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            bool condition = await _roleManager.RoleExistsAsync("Teacher");
            if (!condition)
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                await _roleManager.CreateAsync(role);
            }

            var result1 = await _userManager.AddToRoleAsync(user, "Teacher");
            StatusMessage = result1.Succeeded ? "Your profile has been upgraded!" : "Something went wrong, please try later.";
            await _signInManager.SignInAsync(user, false);
            return RedirectToPage("Index");

        }
    }
 
}
