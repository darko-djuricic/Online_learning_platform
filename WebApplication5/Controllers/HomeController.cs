using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication5.Data;
using WebApplication5.Models;
using System.Security.Claims;

namespace WebApplication5.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //Start page
        public async Task<IActionResult> Index()
        {
            using (var _context=new ViserCoursesContext())
            {
                var categories= await _context.CourseUser.Select(el => el.Course).OrderBy(el => el.Participants.Count)
                                                        .Select(el=>el.Category).Distinct().ToListAsync();
                var length = categories.Count >= 10 ? 10 : categories.Count;
                return View(categories.Take(length).ToList());
            }
        }
        public async Task<IActionResult> Profile(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                //Finding user by name which is passed in route
                var user = await _userManager.FindByNameAsync(id);
                //If user doesnt exists, redirect to start page of platform
                if (user == null) return RedirectToAction("Index", "Home");
                //Getting image of user
                var image = _context.Users.Include(el => el.Image).SingleOrDefault(el => el.Username.Equals(id)).Image;
                //If image is not null, convert it to base64 string and pass it to view
                if (image != null)
                {
                    var imgData = Convert.ToBase64String(image.ImageData);
                    ViewData["Image"] = $"data:{image.ImageType};base64,{imgData}";
                }
                //Getting role of user
                var condition = await _userManager.IsInRoleAsync(user, "Teacher");
                ViewBag.Role = condition ? "Teacher" : await _userManager.IsInRoleAsync(user, "Admin") ? "Admin" : "Student";
                //If user is in teacher role, get sum of ratings and number of students for all his courses
                if (condition)
                {
                    var ratings =await _context.CourseUser.Where(el => el.Rating>0 && _context.Courses.Where(el => el.Author == user.UserName).Any(el2 => el2.CourseId.Equals(el.CourseId)))
                                                        .Select(el => el.Rating).ToListAsync();
                    ViewBag.Rating = ratings.Sum() / (ratings.Count == 0 ? 1 : ratings.Count);
                    ViewBag.Number = ratings.Sum();
                }

                return View(user);
            }
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
