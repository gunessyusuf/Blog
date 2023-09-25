using System.Text;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC;

public class DbController : Controller
{
    private readonly Db _db;

    public DbController(Db db)
    {
        _db = db;
    }

    public IActionResult Seed()
    {
        #region Delete
        var blogTags = _db.BlogTags.ToList();
        _db.BlogTags.RemoveRange(blogTags);

        var tags = _db.Tags.ToList();
        _db.Tags.RemoveRange(tags);

        var users = _db.Users.ToList();
        _db.Users.RemoveRange(users);

        var roles = _db.Roles.ToList();
        _db.Roles.RemoveRange(roles);

        if (roles.Count > 0) // eğer veritabanında rol kaydı varsa eklenecek rollerin rol id'lerini aşağıdaki SQL komutu üzerinden 1'den başlayacak hale getiriyoruz
                             // eğer kayıt yoksa o zaman zaten rol tablosuna daha önce veri eklenmemiştir dolayısıyla rol id'leri 1'den başlayacaktır
        {
            _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Roles', RESEED, 0)"); // ExecuteSqlRaw methodu üzerinden istenilen SQL sorgusu elle yazılıp veritabanında çalıştırılabilir
        }

        var blogs = _db.Blogs.ToList();
        _db.Blogs.RemoveRange(blogs);

        _db.SaveChanges();
        #endregion

        #region Insert
        _db.Roles.Add(new Role()
        {
            Name = "Admin",
            Users = new List<User>()
            {
                new User()
                {
                    UserName = "cagil",
                    Password = "cagil",
                    IsActive = true
                }
            }
        });

        _db.Roles.Add(new Role()
        {
            Name = "User",
            Users = new List<User>()
            {
                new User()
                {
                    UserName = "leo",
                    Password = "leo",
                    IsActive = true
                }
            }
        });

       _db.Tags.Add(new Tag()
        {
            Name = "Programming",
            IsPopular = true
        });
        _db.Tags.Add(new Tag()
        {
            Name = "Social Media"
        });
        _db.Tags.Add(new Tag()
        {
            Name = "Technology"
        });
        _db.Tags.Add(new Tag()
        {
            Name = "Books"
        });
        _db.Tags.Add(new Tag()
        {
            Name = "Dogs",
            IsPopular = true
        });
        _db.Tags.Add(new Tag()
        {
            Name = "Pets",
            IsPopular = true
        });

        _db.SaveChanges();

        _db.Blogs.Add(new Blog()
        {
            Title = "Visual Studio Code",
            Content = "Developing applications using Visual Studio Code.",
            CreateDate = DateTime.Now,
            Score = 5,
            UserId = _db.Users.SingleOrDefault(u => u.UserName == "cagil").Id,
            BlogTags = new List<BlogTag>()
            {
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Programming").Id
                },
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Technology").Id
                }
            }
        });
        _db.Blogs.Add(new Blog()
        {
            Title = "White Fang",
            Content = "One of Jack London's popular books.",
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            Score = 4,
            UserId = _db.Users.SingleOrDefault(u => u.UserName == "cagil").Id,
            BlogTags = new List<BlogTag>()
            {
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Books").Id
                }
            }
        });
        _db.Blogs.Add(new Blog()
        {
            Title = "Leo",
            Content = "A great Sheltie.",
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            Score = 5,
            UserId = _db.Users.SingleOrDefault(u => u.UserName == "leo").Id,
            BlogTags = new List<BlogTag>()
            {
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Dogs").Id
                },
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Pets").Id
                }
            }
        });
        _db.Blogs.Add(new Blog()
        {
            Title = "Instagram",
            Content = "One of the biggest social networks.",
            CreateDate = DateTime.Now,
            Score = 3,
            UserId = _db.Users.SingleOrDefault(u => u.UserName == "leo").Id,
            BlogTags = new List<BlogTag>()
            {
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Programming").Id
                },
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Social Media").Id
                },
                new BlogTag()
                {
                    TagId = _db.Tags.SingleOrDefault(t => t.Name == "Technology").Id
                }
            }
        });

        _db.SaveChanges();
        #endregion

        //return Content("<p style=\"color:red;font-weight:bold;\">Database seed successful.</p>", "text/html", Encoding.UTF8);
        return View();
    }
}
