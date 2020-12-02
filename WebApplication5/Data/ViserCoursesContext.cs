using WebApplication5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Data
{
    public class ViserCoursesContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<CourseUser> CourseUser { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Progress> Progresses{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ViserCourses;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseUser>()
                .HasKey(cu => new { cu.CourseId, cu.UserId });
            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.Course)
                .WithMany(c => c.Participants)
                .HasForeignKey(cu => cu.CourseId);
            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.Courses)
                .HasForeignKey(cu => cu.UserId);

            modelBuilder.Entity<Likes>()
                .HasKey(cu => new { cu.IdComment, cu.UserId });
            modelBuilder.Entity<Likes>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.LikedComments)
                .HasForeignKey(uc => uc.UserId);
            modelBuilder.Entity<Likes>()
                .HasOne(cu => cu.Comment)
                .WithMany(u => u.UserWhoLiked)
                .HasForeignKey(cu => cu.IdComment);

            
        }

        public async static void DeleteUserById(string id)
        {
            using (var _context = new ViserCoursesContext())
            {
                var user = await _context.Users.Include(el => el.Courses)
                                                .Include(el => el.LikedComments)
                                                .Include(el => el.Image)
                                                .SingleOrDefaultAsync(el => el.UserId.Equals(id));
                if (user != null)
                {
                    _context.CourseUser.RemoveRange(user.Courses);
                    _context.Likes.RemoveRange(user.LikedComments);
                    _context.Images.Remove(user.Image);
                    _context.Users.Remove(user);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
