using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication5.Data;
using WebApplication5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("CoursesByTeacher/{author}")]
        public async Task<IActionResult> CoursesByTeacher(string author)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    return Ok(await _context.Courses
                                            .Include(el=>el.Comments).ThenInclude(el=>el.Replies)
                                            .Include(el => el.Participants).ThenInclude(el => el.User).Where(el=>el.Author.Equals(author)).ToListAsync());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse()
        {
            using (var context = new ViserCoursesContext())
            {
                var form = Request.Form;
                try
                {
                    var id = Guid.NewGuid().ToString("N");
                    await context.Courses.AddAsync(new Course()
                    {
                        CourseId = id,
                        Title = form["title"].ToString(),
                        Keywords = form["keywords"].ToString(),
                        TitleDescription = form["TitleDescription"].ToString(),
                        Author = HttpContext.User.Identity.Name,
                        Category = await context.Categories.SingleOrDefaultAsync(el => el.Id == form["category"].ToString()),
                        UpdateDate = DateTime.Now,
                        Published = false
                    });

                    await context.SaveChangesAsync();
                    return RedirectToAction("Content", "Teacher", new { id = id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    TeacherController.StatusMessageCreate = "Something went wrong, please try later.";
                    return RedirectToAction("Index", "Teacher");
                }

            }
        }

        [HttpPost("UpdateCourse/{id}")]
        public async Task<ActionResult> UpdateCourse(string id)
        {
            try
            {
                using(var context=new ViserCoursesContext())
                {
                    var form = Request.Form;
                    var course = await context.Courses.Include(c=>c.Category).Include(el=>el.Sections).Include(el=>el.Image).SingleOrDefaultAsync(el => el.CourseId == id);
                    course.Title = form["title"];
                    course.TitleDescription = form["titledescription"];
                    course.Keywords = form["keywords"];
                    course.Description = form["description"];
                    course.Category = await context.Categories.SingleOrDefaultAsync(el => el.Id.Equals(form["category"].ToString().Trim()));
                    course.UpdateDate = DateTime.Now;

                    if (form.Files.Count > 0)
                    {
                        if (course.Image == null) course.Image = new Image();

                        var image = form.Files[0];
                        MemoryStream ms = new MemoryStream();
                        await image.CopyToAsync(ms);
                        course.Image.ImageData = ms.ToArray();
                        course.Image.ImageType = form.Files[0].ContentType;
                        ms.Close();
                        await ms.DisposeAsync();
                    }
                    context.Entry(course).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                return Ok("Saved!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong!");
            }
            
        }

        [HttpPost("SectionsFunctionalities/{id}")]
        public async Task<ActionResult> UpdateContents([FromBody]IEnumerable<Section> sections, string id)
        {
            try
            {
                using (var context = new ViserCoursesContext())
                {
                    var dbSections = context.Courses.Include(el => el.Sections).ThenInclude(el => el.Contents)
                                                    .AsNoTracking().Single(el => el.CourseId.Equals(id)).Sections;

                    var updateSection = sections.Where(sect => dbSections.Any(el => el.SectionId == sect.SectionId));

                    var deleteContents = new List<Content>();
                    var insertContents = new List<Content>();

                    foreach (var section in updateSection)
                    {
                        var contents = section.Contents.ToList();
                        var dbContents = dbSections.Where(el => el.SectionId == section.SectionId).SelectMany(el => el.Contents);
                        var updateContent = section.Contents.Where(cont => dbContents.Any(el => el.Id == cont.Id)).ToList();
                        foreach (var content in updateContent)
                        {
                            context.Entry(content).State = EntityState.Modified;
                        }

                        deleteContents.AddRange(dbContents.Where(cont => !contents.Any(el => el.Id == cont.Id)).ToList());
                        insertContents.AddRange(contents.Where(cont => !dbContents.Any(el => el.Id == cont.Id)).ToList());

                        context.Entry(section).State = EntityState.Modified;
                    };

                    context.RemoveRange(deleteContents);
                    await context.AddRangeAsync(insertContents);

                    var deleteSections = dbSections.Where(sect => !sections.Any(el => el.SectionId == sect.SectionId)).ToList();
                    var insertSections = sections.Where(sect => !dbSections.Any(el => el.SectionId == sect.SectionId)).ToList();

                    if (deleteSections.Count > 0)
                    {
                        context.RemoveRange(deleteSections);
                        context.Contents.RemoveRange(deleteSections.SelectMany(el => el.Contents));
                    }

                    if (insertSections.Count > 0)
                    {
                        var course = await context.Courses.SingleOrDefaultAsync(el => el.CourseId == id);
                        course.Sections.AddRange(insertSections);
                        context.Entry(course).State = EntityState.Modified;
                    }

                    await context.SaveChangesAsync();
                    return Ok();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }

        }

        [HttpGet("Publish/{id}/{publish}")]
        public async Task<IActionResult> Publish(string id, bool publish)
        {
            try
            {
                using(var context=new ViserCoursesContext())
                {
                    var course = await context.Courses.SingleOrDefaultAsync(el => el.CourseId == id);
                    course.Published = publish;
                    context.Entry(course).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                TeacherController.StatusMessageUpdate = $"Succesfully {(publish ? "published! Congrats!" : "unpublished!")}";
                return RedirectToAction("Content", "Teacher", new { id = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TeacherController.StatusMessageCreate = "Publishing failed!";
                return RedirectToAction("Index", "Teacher");
            }
        }

        [HttpGet("QA/{id}")]
        public async Task<IActionResult> All(string id)
        {
            using(var _context=new ViserCoursesContext())
            {
                var course = await _context.Courses.Include(el => el.Sections).ThenInclude(el => el.Contents).ThenInclude(el => el.QA).SingleOrDefaultAsync(el => el.CourseId == id);
                var contents = course.Sections.SelectMany(el => el.Contents);
                var qa = contents.SelectMany(el => el.QA).ToList();

                return Ok(qa.OrderBy(el=>el.Date));
            }
        }

        [HttpGet("AddQA/{id}")]
        public async Task<IActionResult> AddQA(string id, [FromQuery]Comment qa)
        {
            try
            {
                using(var context=new ViserCoursesContext())
                {
                    var content = await context.Contents.Include(el => el.QA).SingleOrDefaultAsync(el => el.Id == id);
                    if (content != null)
                    {
                        qa.IsQA = true;
                        qa.Author = User.Identity.Name;
                        content.QA.Add(qa);
                    }
                    context.Entry(content).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                return Ok(qa);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("AddComment/{id}")]
        public async Task<IActionResult> AddComment(string id, [FromQuery] Comment comment)
        {
            try
            {
                using (var context = new ViserCoursesContext())
                {
                    comment.IsQA = false;
                    comment.Author = User.Identity.Name;
                    var course = await context.Courses.Include(el => el.Comments).ThenInclude(el=>el.UserWhoLiked).SingleOrDefaultAsync(el => el.CourseId == id);
                    course.Comments.Add(comment);
                    context.Entry(course).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                return Ok(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("AddReply/{id}")]
        public async Task<IActionResult> AddReply(string id, [FromQuery] Comment reply)
        {
            try
            {
                using (var context = new ViserCoursesContext())
                {
                    var comment = await context.Comments.Include(el=>el.Replies).SingleOrDefaultAsync(el => el.Id == id);
                    
                    if (comment != null)
                    {
                        reply.Author = User.Identity.Name;
                        comment.Replies.Add(reply);
                    }
                    context.Entry(comment).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("Rating/{id}")]
        public async Task<IActionResult> Rating(string id, [FromQuery]int rating)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var course = await _context.Courses.Include(el=>el.Participants).ThenInclude(el=>el.User).SingleOrDefaultAsync(el => el.CourseId.Equals(id));
                    course.Participants.SingleOrDefault(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))).Rating = rating;
                    _context.Entry(course).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpGet("LikeUnlike/{id}")]
        public async Task<IActionResult> LikeUnlike(string id,[FromQuery] bool IsLiked)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var comment = await _context.Likes.SingleOrDefaultAsync(el => el.IdComment.Equals(id));
                    if (IsLiked)
                    {
                        if(comment == null)
                        {
                            await _context.Likes.AddAsync(new Likes()
                            {
                                IdComment = id,
                                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                User = await _context.Users.SingleOrDefaultAsync(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier))),
                                Comment = await _context.Comments.SingleOrDefaultAsync(el => el.Id.Equals(id))
                            });
                        }
                    }
                    else
                    {
                        _context.Entry(comment).State = EntityState.Deleted;
                    }
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Listened/{id}/{courseId}")]
        public async Task<IActionResult> Listened(string id, string courseId)
        {
            using (var _context=new ViserCoursesContext())
            {
                var content = await _context.Contents.SingleOrDefaultAsync(el => el.Id.Equals(id));
                var progresses = await _context.Progresses.Include(el => el.Content)
                                        .Include(el => el.CourseUser).ThenInclude(el => el.Course).ThenInclude(el => el.Sections).ThenInclude(el => el.Contents)
                                        .Where(el=>el.CourseUser.CourseId.Equals(courseId) && el.CourseUser.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                                        .ToListAsync();
                var courseUser = _context.CourseUser.Include(el=>el.Course).ThenInclude(el=>el.Sections).ThenInclude(el=>el.Contents)
                                                    .Single(el => el.CourseId.Equals(courseId) && 
                                                                    el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));

                if (content != null && !courseUser.Listened && !progresses.Any(el=>el.Content.Id.Equals(id)))
                {
                    var progress = new Progress()
                    {
                        Content = content,
                        ProgressId = Guid.NewGuid().ToString("N"),
                        CourseUser = await _context.CourseUser.SingleAsync(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)) && el.CourseId.Equals(courseId))
                    };
                    _context.Progresses.Add(progress);
                    if (Progress.ProgressCounter(progresses) > 99)
                    {
                        progresses.Remove(progress);
                        _context.Progresses.RemoveRange(progresses);
                        courseUser.Listened = true;
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
        }

        [HttpGet("Subcategories/{id}")]
        public IActionResult Subcategories(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                return Ok(_context.Categories.Include(el=>el.Subcategories).SingleOrDefaultAsync(el=>el.Id.Equals(id)).Result.Subcategories.ToList());
            }
        }

        [HttpGet("ImageOfUserByName/{username}")]
        public async Task<IActionResult> ImageOfUserByName(string username)
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var user = await _context.Users.Include(el => el.Image).SingleOrDefaultAsync(el => el.Username.Equals(username));
                    string imageDataUrl = String.Empty;
                    if (user != null)
                    {
                        var img = user.Image;
                        if (img != null)
                        {
                            string imageBase64Data = Convert.ToBase64String(img.ImageData);
                            imageDataUrl = $"data:{img.ImageType};base64,{imageBase64Data}";
                        }
                    }
                    return Ok(imageDataUrl);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet, ActionName("DeleteProfilePicture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            try
            {
                using (var _context = new ViserCoursesContext())
                {
                    var user = await _context.Users.Include(el => el.Image).SingleAsync(el => el.UserId.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                    _context.Images.Remove(user.Image);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                var comment = await _context.Comments
                    .Include(el => el.Replies).ThenInclude(el => el.UserWhoLiked)
                    .Include(el => el.UserWhoLiked)
                    .SingleOrDefaultAsync(el => el.Id.Equals(id));

                if (comment == null) return BadRequest();

                if (comment.Replies != null && comment.Replies.Count > 0)
                {
                    if (comment.Replies.SelectMany(el=>el.UserWhoLiked).Count() > 0)
                        _context.Likes.RemoveRange(comment.Replies.SelectMany(el => el.UserWhoLiked));

                    _context.Comments.RemoveRange(comment.Replies);

                    if(comment.UserWhoLiked!=null && comment.UserWhoLiked.Count > 0)
                        _context.Likes.RemoveRange(comment.UserWhoLiked);
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpGet("TeacherChart")]
        public async Task<IActionResult> TeacherChart()
        {
            using (var _context = new ViserCoursesContext())
            {
                var chartData = new List<object>();
                var participants = await _context.CourseUser.Include(el => el.Course).ThenInclude(el=>el.Comments)
                                                            .Include(el=>el.User)
                                                            .Where(el => el.Course.Author.Trim().Equals(User.Identity.Name.Trim())).ToListAsync();
                foreach (var part in participants)
                {
                    var year = part.EnrollDate.Year;
                    var filterParts = participants.Where(el => el.CourseId.Equals(el.CourseId) && el.EnrollDate.Year == year);
                    //Rating by monts for certain year. First, we get comments for course. Then we sum ratings where username are in comments collectio, and then we divide it with comments count
                    var rating = Enumerable.Range(1, 13).Select(i =>
                    {
                        var comments = part.Course.Comments.Where(el => !el.IsQA && el.Date.Year == year && el.Date.Month == i);
                        return Math.Round(filterParts.Sum(el => comments.Any(el2 => el2.Author.Equals(el.User.Username)) ? el.Rating : 0) / comments.Count());
                    }).ToList();

                    chartData.Add(new
                    {
                        id = part.CourseId,
                        year = year,
                        //Popularity by monts of certain year. Popularity = Participants who enrolled course in certain mont / Rating for certain mont (if rating is Nan => 1)
                        popularity = Enumerable.Range(1, 13).Select(i => filterParts.Count(el => el.EnrollDate.Month == i) + (Double.IsNaN(rating[i - 1]) ? 0 : rating[i - 1])),
                        rating = rating
                    });
                }
                return Ok(chartData);
            }
        }

        [HttpGet("AdminChart")]
        public async Task<IActionResult> AdminChart()
        {
            var chartData = new List<object>();
            using (var _context = new ViserCoursesContext())
            {
                var users = await _context.Users.ToListAsync();
                foreach(var year in users.Select(el=>el.Date.Year).Distinct())
                {
                    chartData.Add(new { year = year, popularity = Enumerable.Range(1, 13).Select(i => users.Count(el => el.Date.Year == year && el.Date.Month == i)) });
                };
                return Ok(chartData);
            }
        }

    }

}
