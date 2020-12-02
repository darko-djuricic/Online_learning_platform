using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            using (var _context = new ViserCoursesContext())
            {
                ViewData["Years"] = _context.Users.ToList().Where(user => _userManager.Users.Single(el => el.UserName.Equals(user.Username)).EmailConfirmed)
                                                .Select(el => el.Date).GroupBy(el => el.Date.Year).Select(x => x.First()).ToList();

                var categories = await _context.Categories.Include(el => el.Subcategories).ToListAsync();
                var subcategories = categories.SelectMany(el => el.Subcategories).ToList();
                categories = categories.Where(el => !subcategories.Any(x => x.Id.Equals(el.Id)))
                                                                .OrderBy(el => el.Title)
                                                                .ToList();
                ViewData["Categories"] = categories;
                ViewData["Subcategories"] = categories.SelectMany(el => el.Subcategories).ToList();

                return View(_userManager);
            }
        }
        public async Task<IActionResult> UpgradeAdmin(string id)
        {
            try
            {
                using (_userManager)
                {
                    bool condition = await _roleManager.RoleExistsAsync("Admin");
                    if (!condition)
                    {
                        var role = new IdentityRole();
                        role.Name = "Admin";
                        await _roleManager.CreateAsync(role);
                    }
                    var result = await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(id), "Admin");
                    TempData["AdminStatus"] = result.Succeeded ? "Succesfully upgraded user to admin" : "Something went wrong.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["AdminStatus"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddCategory()
        {
            try
            {
                var form = Request.Form;
                using (var _context = new ViserCoursesContext())
                {
                    var category = await _context.Categories.Include(el => el.Subcategories).SingleOrDefaultAsync(el => el.Id.Equals(form["parent"].ToString()));
                    var newCategory = new Categories()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = form["title"].ToString()
                    };
                    TempData["AdminStatus"] = $"Category \"{newCategory.Title}\" alerady exists!";
                    if (category != null)
                    {
                        if(!category.Subcategories.Any(el => el.Title.ToLower().Trim().Equals(newCategory.Title.ToLower().Trim())))
                        {
                            category.Subcategories.Add(newCategory);
                            _context.Categories.Update(category);
                            TempData["AdminStatus"] = "Added new subcategory for "+category.Title+"!";
                        }
                    }
                    else
                    {
                        if (!_context.Categories.Any(el => el.Title.ToLower().Trim().Equals(newCategory.Title.ToLower().Trim())))
                        {
                            _context.Categories.Add(newCategory);
                            TempData["AdminStatus"] = "Added new category \"" + newCategory.Title + "\".";
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["AdminStatus"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateCategory(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                var form = Request.Form;
                var category = await _context.Categories.Include(el => el.Subcategories).SingleOrDefaultAsync(el => el.Id.Equals(id));
                if (category != null)
                {
                    category.Title = form["title"].ToString();
                    _context.Categories.Update(category);

                    var parentId = form["parent"].ToString();
                    if (!String.IsNullOrEmpty(parentId))
                    {
                        var parent= await _context.Categories.Include(el => el.Subcategories).SingleOrDefaultAsync(el => el.Id.Equals(parentId));
                        if (!parent.Subcategories.Contains(category))
                        {
                            parent.Subcategories.Add(category);
                            _context.Categories.Update(parent);
                        }
                    }
                    TempData["AdminStatus"] = "Category updated!";
                }
                else
                {
                    TempData["AdminStatus"] = "Category not found!";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var category = await _context.Categories.Include(el => el.Subcategories).SingleOrDefaultAsync(el => el.Id.Equals(id));
                    if (category != null)
                    {
                        if (category.Subcategories != null) _context.RemoveRange(category.Subcategories);
                        _context.Remove(category);
                    }
                    await _context.SaveChangesAsync();
                    TempData["AdminStatus"] = $"Deleted category \"{category.Title}\"";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["AdminStatus"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (!await _userManager.IsInRoleAsync(user, "Admin")) 
                    {
                        foreach (var role in await _userManager.GetRolesAsync(user))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                        var delete=await _userManager.DeleteAsync(user);
                        if (delete.Succeeded)
                        {
                            if (await _context.Users.SingleOrDefaultAsync(el => el.UserId.Equals(id)) != null) ViserCoursesContext.DeleteUserById(id);
                        }
                        TempData["AdminStatus"] = delete.Succeeded ? "Succesfully deleted user " + user.UserName : "Something went wrong";
                        await _context.SaveChangesAsync();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["AdminStatus"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
    }
}
