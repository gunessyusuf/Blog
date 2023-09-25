using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess;

namespace Business.Services
{
    public interface IBlogService : IService<BlogModel>
    {
        Result DeleteImage(int blogId);
    }

    public class BlogService : IBlogService
    {
        private readonly RepoBase<Blog> _blogRepo;

        // private readonly RepoBase<BlogTag> _blogTagRepo; // constructor'da ikinci parametre olarak inject edilmeli

        public BlogService(RepoBase<Blog> blogRepo)
        {
            _blogRepo = blogRepo;
        }

        public IQueryable<BlogModel> Query()
        {
            // eğer istenirse Select ile projeksyion işlemi AutoMapper kütüphanesi üzerinden otomatik hale getirilebilir
            return _blogRepo.Query().OrderByDescending(b => b.CreateDate).ThenBy(b => b.Title).Select(b => new BlogModel()
            {
                Content = b.Content,
                CreateDate = b.CreateDate,
                Guid = b.Guid,
                Id = b.Id,
                Score = b.Score,
                Title = b.Title,
                UpdateDate = b.UpdateDate,
                UserId = b.UserId,

                UserNameDisplay = b.User.UserName,
                CreateDateDisplay = b.CreateDate.ToString("MM/dd/yyyy HH:mm"),
                UpdateDateDisplay = b.UpdateDate.HasValue ? b.UpdateDate.Value.ToString("MM/dd/yyyy HH:mm") : "",
                
                // 1. yöntem:
                //ScoreDisplay = b.Score.HasValue ? b.Score.Value.ToString("N1") : "0",
                // 2. yöntem:
                ScoreDisplay = (b.Score ?? 0).ToString("N1"),

				// Many to Many ilişkili kayıtları getirme 1. yöntem:
				TagsDisplay = b.BlogTags.Select(bt => new TagModel()
                {
                    Guid = bt.Tag.Guid,
                    Id = bt.Tag.Id,
                    IsPopular = bt.Tag.IsPopular,
                    Name = bt.Tag.Name
                }).ToList(),

                // Many to Many ilişkili kayıtları getirme 2. yöntem:
                //TagsDisplay = string.Join("<br />", b.BlogTags.Select(bt => bt.Tag.Name)),

                // Edit işleminde view'a ViewBag ile gönderilen MultiSelectList tipindeki tag listesinde
                // düzenlenen mevcut blog kaydının ilişkili BlogTag'leri üzerinden tag'lerin
                // TagIds ile seçili gelmesi için bu özelliğin atanması gerekir
                TagIds = b.BlogTags.Select(bt => bt.TagId).ToList(),

                //Image = b.Image,
                ImageExtension = b.ImageExtension,

                ImgSrcDisplay = b.Image != null ? 
                    (
                        b.ImageExtension == ".jpg" || b.ImageExtension == ".jpeg" ? 
                        "data:image/jpeg;base64,"
                        : "data:image/png;base64,"
                    )  + Convert.ToBase64String(b.Image)
                    : null
            });
        }

        public Result Add(BlogModel model)
        {
            // kullanıcı bazında blog başlığı tabloda tekil olmalı
            // 1. yöntem:
            //if (_blogRepo.Query().Any(b => b.UserId == model.UserId &&
            //  b.Title.ToLower() == model.Title.ToLower().Trim()))
            // 2. yöntem:
            if (_blogRepo.Exists(b => b.UserId == model.UserId &&
                b.Title.ToLower() == model.Title.ToLower().Trim()))
            {
                return new ErrorResult("Blog with the same title exists!");
            }

            // 1. yöntem:
            //Blog entity = new Blog()
            //{
            //    Content = model.Content.Trim(),
            //    CreateDate = DateTime.Now,
            //    Score = model.Score,
            //    Title = model.Title.Trim(),

            //    // 1. yöntem:
            //    //UserId = model.UserId ?? 0
            //    // 2. yöntem:
            //    UserId = model.UserId.Value,

            //    // *1
            //    BlogTags = new List<BlogTag>()
            //};
            //// *1
            //if (model.TagIds is not null && model.TagIds.Count > 0)
            //{
            //    foreach (int tagId in model.TagIds)
            //    {
            //        entity.BlogTags.Add(new BlogTag()
            //        {
            //            TagId = tagId
            //        });
            //    }
            //}

            // 2. yöntem:
            Blog entity = new Blog()
            {
                Content = model.Content.Trim(),
                CreateDate = DateTime.Now,
                Score = model.Score,
                Title = model.Title.Trim(),

                // 1. yöntem:
                //UserId = model.UserId ?? 0
                // 2. yöntem:
                UserId = model.UserId.Value,

                // *2
                BlogTags = model.TagIds.Select(tagId => new BlogTag()
                {
                    TagId = tagId
                }).ToList(),

                Image = model.Image,
                ImageExtension = model.ImageExtension
            };
            _blogRepo.Add(entity);

            model.Id = entity.Id;

            return new SuccessResult("Blog added successfully.");
        }

        public Result Update(BlogModel model)
        {
            // güncellenecek kayıt dışında kullanıcı bazında blog başlığı tabloda tekil olmalı
            if (_blogRepo.Exists(b => b.UserId == model.UserId &&
                b.Title.ToLower() == model.Title.ToLower().Trim() &&
                b.Id != model.Id))
            {
                return new ErrorResult("Blog with the same title exists!");
            }

            // önce blog ile ilişkili blog tag kayıtları silinir,
            // BlogTag tipi üzerinden DbSet'te BlogId'si parametre olarak gelen model.Id olan ilişkili kayıtları siler,
            // bir aşağıdaki Delete methodunun save paremetresi default false'tur ve bu şekilde çağrılır,
            // nedeni önce BlogTag DbSet'i üzerinden blog'un id'sine sahip ilişkili blog tag kayıtları silinsin
            // ancak bu BlogTag tipindeki DbSet'teki değişiklikler veritabanına bu aşamada yansıtılmasın
            // daha sonra blog güncellendiğinde veya silindiğinde save parametresi true default olarak kullanılır ki
            // hem BlogTag hem de Blog DbSet'lerinde yapılan tüm değişiklikler tek seferde veritabanına yansıtılsın
            // (Unit of Work)
            _blogRepo.Delete<BlogTag>(bt => bt.BlogId == model.Id);

            // _blogTagRepo.Delete(bt => bt.BlogId == model.Id);

            // sonra blog güncellemesi model üzerinden gelen TagIds üzerinden blog tag kayıtlarının
            // oluşturulmasıyla birlikte yapılır
            var entity = _blogRepo.Query().SingleOrDefault(b => b.Id == model.Id);

            //entity.Id = model.Id; // id mutlaka atanmalı ki Entity Framework hangi kaydı güncelleyeceğini bilsin
            entity.Content = model.Content.Trim();
            entity.Score = model.Score;
            entity.Title = model.Title.Trim();
            entity.UpdateDate = DateTime.Now;
            entity.UserId = model.UserId.Value;
            entity.BlogTags = model.TagIds.Select(tagId => new BlogTag()
            {
                TagId = tagId
            }).ToList();

            if (model.Image is not null)
            {
                entity.Image = model.Image;
                entity.ImageExtension = model.ImageExtension;
            }
            
            _blogRepo.Update(entity);

            return new SuccessResult("Blog updated successfully.");
        }

        public Result Delete(int id)
        {
            // önce blog ile ilişkili blog tag kayıtları silinir
            _blogRepo.Delete<BlogTag>(bt => bt.BlogId == id);

            // sonra blog kaydı silinir
            _blogRepo.Delete(id);

            return new SuccessResult("Blog deleted successfully.");
        }

        public void Dispose()
        {
            _blogRepo.Dispose();
        }

        public Result DeleteImage(int blogId)
        {
            var entity = _blogRepo.Query().SingleOrDefault(b => b.Id == blogId);
            entity.Image = null;
            entity.ImageExtension = null;
            _blogRepo.Update(entity);
            return new SuccessResult();
        }
    }
}
