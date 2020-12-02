using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Data;
using WebApplication5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace WebApplication5.Controllers
{
    [Authorize(Roles ="Teacher")]
    public class TeacherController : Controller
    {
        [TempData]
        public static string StatusMessageCreate { get; set; }
        [TempData]
        public static string StatusMessageUpdate { get; set; }
        public async Task<IActionResult> Index()
        {
            ViewBag.Message = StatusMessageCreate;
            StatusMessageUpdate = null;
            List<Course> list;
            using (var context=new ViserCoursesContext())
            {
                list = await context.Courses.Include(el=>el.Image).Where(el => el.Author == User.Identity.Name).ToListAsync();
                var listOfCategories = new SelectList(await context.Categories
                                                                    .Include(el=>el.Subcategories).OrderBy(el => el.Title)
                                                                    .Where(el=>!(el.Subcategories==null || el.Subcategories.Count==0))
                                                                    .ToListAsync(), "Id", "Title");
                listOfCategories.First().Selected = true;
                ViewData["Categories"] = listOfCategories;
            }
            return View(list);
        }
        public new async Task<IActionResult> Content(string id)
        {
            try
            {
                ViewBag.Status = StatusMessageUpdate;
                using (var context = new ViserCoursesContext())
                {
                    var course = await context.Courses
                                    .Include(el=>el.Image)
                                    .Include(c => c.Sections)
                                    .ThenInclude(c=>c.Contents)
                                    .Include(c => c.Category)
                                    .SingleOrDefaultAsync(el => el.CourseId == id);

                    if (!course.Author.Equals(User.Identity.Name)) return BadRequest("Permission denied");

                    var categories = await context.Categories.Include(el => el.Subcategories).ToListAsync();
                    var subcategories = categories.SelectMany(el => el.Subcategories);

                    var listOfCategories = new SelectList(categories.Where(el => el.Subcategories.Count > 0).OrderBy(el => el.Title), "Id", "Title");
                    SelectList listOfSubcat = null;

                    var selectedCategory = listOfCategories.SingleOrDefault(el => el.Value == course.Category.Id);

                    if (selectedCategory != null)
                    {
                        selectedCategory.Selected = true;
                        listOfSubcat= new SelectList(categories.SingleOrDefault(el => el.Id==selectedCategory.Value).Subcategories, "Id", "Title");
                    }
                    else
                    {
                        var subcat = subcategories.SingleOrDefault(el => el.Id == course.Category.Id);
                        if (subcat != null)
                        {
                            var cat = categories.Single(el => el.Subcategories.Any(el => el.Id.Equals(subcat.Id)));
                            listOfCategories.Single(el => el.Value.Equals(cat.Id)).Selected = true;
                            listOfSubcat = new SelectList(cat.Subcategories, "Id", "Title");
                            listOfSubcat.Single(el => el.Value.Equals(subcat.Id)).Selected = true;
                        }
                    }

                    ViewBag.Subcat = listOfSubcat;
                    ViewBag.Categories = listOfCategories;
                    var keywords = course.Keywords;
                    ViewBag.Keywords = new List<string>(keywords==null?new string[0]:keywords.Split(";")).GetEnumerator();
                    return View(course);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                StatusMessageCreate = "Something went wrong! Please try later";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Communications()
        {
            using (var _context = new ViserCoursesContext())
            {
                var courses = await _context.Courses
                                .Include(el => el.Comments).ThenInclude(el => el.Replies)
                                .Include(el => el.Comments).ThenInclude(el => el.UserWhoLiked)
                                .Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA)
                                .Where(el => el.Author.Equals(User.Identity.Name) && el.Published).ToListAsync();
                return View(courses);
            }
        }
        public async Task<IActionResult> Performance()
        {
            using (var _context = new ViserCoursesContext())
            {
                var courses = await _context.Courses
                            .Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA)
                            .Include(el=>el.Image)
                            .Include(el => el.Participants)
                            .Include(el => el.Sections)
                            .Where(el => el.Author.Trim().Equals(User.Identity.Name.Trim()) && el.Published)
                            .ToListAsync();
                ViewData["Participants"] = await _context.CourseUser.Include(el => el.Course).Where(el => el.Course.Author.Trim().Equals(User.Identity.Name.Trim())).ToListAsync();

                return View(courses);
            }
        }
    }
}
