using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Data;
using WebApplication5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApplication5.Controllers
{
    public class CourseController : Controller
    {
        static int number_of_filter_objects = 10;
        public async Task<IActionResult> Index([FromQuery]string search, [FromQuery] string order)
        {
            using (var _context = new ViserCoursesContext())
            {
                var courses = await _context.Courses.Include(el => el.Image)
                                            .Include(el => el.Category).ThenInclude(el => el.Subcategories)
                                            .Include(el => el.Sections).ThenInclude(el => el.Contents)
                                            .Include(el => el.Participants).ThenInclude(el => el.User)
                                            .Where(el => el.Published)
                                            .ToListAsync();

                if (!String.IsNullOrEmpty(search))
                {
                    var searchToLower = search.Trim().ToLower();
                    courses = courses.Where(el => !String.IsNullOrEmpty(el.Keywords) && (el.Keywords.ToLower().Contains(searchToLower) || 
                                                    el.Title.ToLower().Contains(searchToLower) || el.TitleDescription.ToLower().Contains(searchToLower) ||
                                                    el.Description.ToLower().Contains(searchToLower) || el.Author.ToLower().Contains(searchToLower))).ToList();
                }
                //Order
                switch (order)
                {
                    case "":
                    case null:
                        break;
                    case "popular":
                        courses = courses.OrderByDescending(el => el.Participants.Count).ToList();
                        break;
                    case "new":
                        courses = courses.OrderByDescending(el => el.UpdateDate.Ticks).ToList();
                        break;
                    case "highest":
                        courses = courses.OrderByDescending(el => el.Participants.Sum(el2 => el2.Rating) / el.Participants.Where(el => el.Rating > 0).Count()).ToList();
                        break;
                }

                ViewData["Topics"] = courses.Select(el => el.GetKeywords()).ToList().SelectMany(el => el).Where(el => !String.IsNullOrEmpty(el)).Distinct().OrderBy(el => el).ToList();
                ViewData["Categories"] = await _context.Categories.Include(el => el.Subcategories).ToListAsync();

                return View(courses);
            }
        }
        public async Task<IActionResult> Explore(string id)
        {
            using (var _context=new ViserCoursesContext())
            {
                var courses = await _context.Courses.Include(el => el.Image)
                                            .Include(el => el.Category).ThenInclude(el => el.Subcategories)
                                            .Include(el => el.Sections).ThenInclude(el => el.Contents)
                                            .Include(el => el.Participants).ThenInclude(el => el.User)
                                            .Where(el => el.Published)
                                            .ToListAsync();

                var categories= await _context.Categories.Include(el => el.Subcategories).ToListAsync();
                if (!String.IsNullOrEmpty(id))
                {
                    var category = await _context.Categories.Include(el => el.Subcategories).SingleOrDefaultAsync(el=>el.Id.Trim().Equals(id.Trim()));

                    if (category != null)
                    {
                        courses = (category.Subcategories != null && category.Subcategories.Count > 0) ?
                                        courses.Where(el => category.Subcategories.Any(el2=>el2.Id.Trim().Equals(el.Category.Id.Trim()))).ToList() :
                                        courses.Where(el => el.Category.Id.Trim().Equals(category.Id.Trim())).ToList();
                    }

                    ViewData["Category"] = category;
                }

                if (User.Identity.IsAuthenticated && _context.CourseUser.Where(el=>el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))).Count()>0)
                {
                    var keywords = _context.CourseUser.Include(el => el.Course)
                                                    .Where(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                                    .Select(el => el.Course.GetKeywords())
                                                    .ToListAsync().Result
                                                    .SelectMany(el=>el).ToList();
                        
                    ViewData["Topics"] = keywords.Select(el => el.Split(' ')).SelectMany(el=>el).Select(el=>el.ToLower()).Distinct().ToList();
                    ViewData["Recomended"] = courses.Where(el => !el.Participants.Any(el2 => el2.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))) &&
                                                                keywords.Any(el2 => el.Keywords.Contains(el2))).ToList();
                }

                ViewData["MostPopular"] = courses.OrderByDescending(el => el.Participants.Count).Take(courses.Count < number_of_filter_objects ? courses.Count : number_of_filter_objects).ToList();
                ViewData["HotNew"] = courses.OrderByDescending(el => el.UpdateDate.Ticks)
                            .ThenByDescending(el => el.Participants.Count)
                            .ThenByDescending(el => (el.Participants.Sum(el2 => el2.Rating) / el.Participants.Count))
                            .Where(el => (DateTime.Now - el.UpdateDate).TotalDays < 30)
                            .ToList();

                ViewData["Categories"] = categories;
                
                return View(courses);
            }
        }
        [Authorize]
        public async Task<IActionResult> Lecture(string id,[FromQuery]string content, [FromQuery]bool view)
        {
            Course course = null;
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    course = await _context.Courses.Include(el=>el.Participants).ThenInclude(el=>el.User)
                                                    .Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA).ThenInclude(el => el.Replies)
                                                    .SingleOrDefaultAsync(el => el.CourseId == id);

                    if (course == null || !course.Published || !course.Participants.Any(el => el.User.Username == User.Identity.Name) || (view && !(User.IsInRole("Admin") || course.Author.Equals(User.Identity.Name))))
                    {
                        return RedirectToAction("Explore");
                    }

                    var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);                    
                    var contents = course.Sections.OrderBy(el=>el.Number).SelectMany(el => el.Contents.OrderBy(el=>el.Number)).ToList();
                    var cont = contents.SingleOrDefault(el => el.Id == content) ?? (view ? contents.First() : new Content());
                    var progressContents = new List<Content>();
                    if (!view)
                    {
                        progressContents = await _context.Progresses
                                            .Include(el => el.CourseUser).ThenInclude(el=>el.Course).ThenInclude(el=>el.Sections).ThenInclude(el=>el.Contents)
                                            .Where(el => el.CourseUser.CourseId.Equals(id) && el.CourseUser.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                            .Select(el => el.Content)
                                            .ToListAsync();
                        ViewData["ProgressContents"] = progressContents;
                    }

                    //getting last listened content to continue watching
                    if (String.IsNullOrEmpty(content) && !view)
                    {
                        if (progressContents.Count > 1)
                        {
                            var section = course.Sections.Single(el => el.Number.Equals(progressContents.Count(el => el.Number == 1)));
                            var filteredCont = section.Contents.Where(el => progressContents.Any(el2 => el2.Id.Equals(el.Id))).OrderBy(el => el.Number).ToList().LastOrDefault();
                            cont = filteredCont==null || filteredCont.Id.Equals(contents.Last().Id) ? contents.First() : contents[contents.IndexOf(filteredCont) + 1];
                        }
                        else
                        {
                            cont = progressContents.Count == 0 || progressContents[0].Id.Equals(contents.Last().Id) ? contents.First() : contents[contents.IndexOf(progressContents[0]) + 1];
                        }
                    }
                    
                    ViewData["Content"] = cont;
                    ViewData["QA"] = contents.SelectMany(el => el.QA).OrderByDescending(el=>el.Date.Ticks).ToList();
                    ViewData["LikedComments"] = _context.Likes.Include(el => el.User).Include(el => el.Comment).ToList();

                    if (!view)
                    {
                        ViewBag.NextVideo = cont.Id.Equals(contents.Last().Id) ? null : contents[contents.IndexOf(cont) + 1].Id;
                        ViewBag.PreviousVideo = cont.Id.Equals(contents.First().Id) ? null : contents[contents.IndexOf(cont) - 1].Id;
                        ViewBag.Listened = _context.CourseUser.Single(el => el.CourseId.Equals(id) && el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))).Listened;
                    }
                    ViewBag.View = view;
                    return View(course);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
        public async Task<IActionResult> Info(string id)
        {
            using (var _context= new ViserCoursesContext())
            {
                var course = await _context.Courses.Include(el=>el.Participants).ThenInclude(el=>el.User)
                                                    .Include(el=>el.Image)
                                                    .Include(el=>el.Category)
                                                    .Include(el=>el.Comments).ThenInclude(el=>el.Replies)
                                                    .Include(el=>el.Sections).ThenInclude(el=>el.Contents)
                                                    .SingleOrDefaultAsync(el => el.CourseId == id);

                if (course == null || !course.Published) return RedirectToAction("Explore");

                var ratings = await _context.CourseUser.Include(el => el.Course).Where(el => el.CourseId == course.CourseId && el.Rating>0).Select(el => el.Rating).ToListAsync();
                var amount = ratings.Count;

                ViewBag.Rating = ratings.Sum() / (amount > 0 ? amount : 1);
                ViewBag.RatingsAmount = amount;
                
                var comments = course.Comments.Where(el => !el.IsQA);
                ViewData["LikedComments"] = await _context.Likes.Include(el => el.User).Include(el => el.Comment).ThenInclude(el=>el.UserWhoLiked)
                                                                                        .Include(el=>el.Comment).ThenInclude(el=>el.Replies).ThenInclude(el=>el.UserWhoLiked).ToListAsync();
                ViewData["Users"] = await Task.Run(() => _context.Users.Include(el => el.Image).ToListAsync().Result
                                         .Where(el => comments.Any(el2 => el.Username.Equals(el2.Author) || el2.Replies.Any(el3 => el.Username.Equals(el3.Author))))
                                         .ToList());

                var temp = await _context.CourseUser.SingleOrDefaultAsync(el => el.CourseId.Equals(id) && el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));

                if (temp != null)
                {
                var listened = temp.Listened;
                ViewBag.Progress = listened ? 100 : 
                                        Progress.ProgressCounter(await _context.Progresses
                                            .Include(el => el.CourseUser).ThenInclude(el => el.Course).ThenInclude(el => el.Sections).ThenInclude(el => el.Contents)
                                            .Where(el => el.CourseUser.CourseId.Equals(id) && el.CourseUser.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                            .ToListAsync());
                }
                
                return View(course);
            }
        }
        [Authorize]
        public async Task<IActionResult> MyCourses()
        {
            using (var _context=new ViserCoursesContext())
            {
                var courses = await _context.CourseUser.Include(el => el.Course).ThenInclude(el => el.Image)
                                                        .Include(el=>el.Course).ThenInclude(el=>el.Category)
                                                        .Where(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                                        .ToListAsync();
                var courseIds = courses.Select(el => el.CourseId);
                var progresses = await _context.Progresses
                                        .Include(el => el.CourseUser).ThenInclude(el => el.Course).ThenInclude(el => el.Sections).ThenInclude(el => el.Contents)
                                        .Where(el => courseIds.Any(el2 => el2.Equals(el.CourseUser.CourseId)) && el.CourseUser.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                        .ToListAsync();

                ViewData["Courses"] = courses.Where(el => !el.Listened || progresses.Any(x => x.CourseUser.CourseId.Equals(el.CourseId))).ToList();
                ViewData["CourseUsers"] = _context.CourseUser.Include(el => el.Course).ToList();
                ViewData["Progresses"] = progresses;

                return View(courses.OrderBy(el => el.Course.UpdateDate).ToList());
            }
        }
        public async Task<IActionResult> Enroll(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                var user = await _context.Users.Include(el => el.Courses).SingleOrDefaultAsync(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                if (user == null) return RedirectToPage("/Account/Register", new { area = "Identity" });
                _context.CourseUser.Add(new CourseUser()
                {
                    CourseId = id,
                    Course = await _context.Courses.Include(el => el.Category)
                            .Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA).ThenInclude(el => el.Replies)
                            .Include(el => el.Comments).ThenInclude(el => el.Replies)
                            .Include(el => el.Participants).SingleOrDefaultAsync(el => el.CourseId == id),
                    User = await _context.Users.Include(el => el.Courses).SingleOrDefaultAsync(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Info", new { id = id });
            }
        }
        [Authorize(Roles ="Teacher, Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                var course = await _context.Courses
                                    .Include(el => el.Image)
                                    .Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA).ThenInclude(el => el.Replies).ThenInclude(el => el.UserWhoLiked)
                                    .Include(el => el.Participants)
                                    .SingleOrDefaultAsync(el => el.CourseId.Equals(id));

                if (course == null) return NotFound();

                if(course.Image!=null) _context.Images.Remove(course.Image);

                if (course.Sections != null)
                {
                    var qa = course.Sections.SelectMany(el => el.Contents).SelectMany(el => el.QA).ToList();
                    if (qa != null)
                    {
                        _context.Likes.RemoveRange(qa.SelectMany(el => el.UserWhoLiked).ToList());
                        _context.Comments.RemoveRange(qa.SelectMany(el => el.Replies).ToList());
                        _context.Comments.RemoveRange(qa);
                    }
                    _context.Contents.RemoveRange(course.Sections.SelectMany(el => el.Contents).ToList());
                    _context.Sections.RemoveRange(course.Sections);
                }
                if (course.Participants != null)
                    _context.CourseUser.RemoveRange(course.Participants);

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                if(User.IsInRole("Admin"))
                    return RedirectToAction("Explore", "Course");

                TeacherController.StatusMessageCreate = "Deleted course. If you had some troubleties, contact us!";
                return RedirectToAction("Index","Teacher");
            }
        }
    }

}
