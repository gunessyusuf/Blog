using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public interface ITagService : IService<TagModel>
    {
		// ihtiyaca göre uygulamamızın kullanacağı servisler bazında ihtiyaç duyulan method tanımları
		// burada yapılarak bu interface'i implemente eden tüm sınıflarda bu method tanımları üzerinden
		// methodların oluşturulması sağlanabilir ve bu method tanımları controller'larda servisler
        // üzerinden çağrılabilir
		List<TagModel> GetList();
        TagModel GetItem(int id);
    }

    public class TagService : ITagService
    {
        private readonly RepoBase<Tag> _tagRepo;

        public TagService(RepoBase<Tag> tagRepo)
        {
            _tagRepo = tagRepo;
        }

        public Result Add(TagModel model)
        {
            // tag adı tabloda tekil olmalı
            if (_tagRepo.Exists(t => t.Name.ToUpper() == model.Name.ToUpper().Trim()))
                return new ErrorResult("Tag with the same name exists!");

            Tag entity = new Tag()
            {
                IsPopular = model.IsPopular,
                Name = model.Name.Trim()
            };

            _tagRepo.Add(entity);

            model.Id = entity.Id;

            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            // önce tag ile ilişkili blog tag kayıtları silinir
            _tagRepo.Delete<BlogTag>(bt => bt.TagId == id);

            // burada başka bir yöntem olarak id yerine predicate (koşul) üzerinden kaydı siliyoruz
            _tagRepo.Delete(t => t.Id == id);

            return new SuccessResult();
        }

        public void Dispose()
        {
            _tagRepo.Dispose();
        }

        public IQueryable<TagModel> Query()
        {
            return _tagRepo.Query().Select(t => new TagModel()
            {
                Guid = t.Guid,
                Id = t.Id,
                IsPopular = t.IsPopular,
                Name = t.Name,

                BlogCountDisplay = t.BlogTags.Count
            });
        }

        public List<TagModel> GetList()
        {
            // 1. yöntem:
            //return _tagRepo.Query().OrderBy(t => t.Name).Select(t => new TagModel()
            //{
            //    Guid = t.Guid,
            //    Id = t.Id,
            //    IsPopular = t.IsPopular,
            //    Name = t.Name + (t.IsPopular ? " *" : ""),
            //    IsPopularDisplay = t.IsPopular ? "Yes" : "No",
            //    BlogCountDisplay = t.BlogTags.Count
            //}).ToList();
            // 2. yöntem:
            return Query().OrderBy(t => t.Name).Select(t => new TagModel()
            {
                Guid = t.Guid,
                Id = t.Id,
                IsPopular = t.IsPopular,
                Name = t.Name + (t.IsPopular ? " *" : ""),
                IsPopularDisplay = t.IsPopular ? "Yes" : "No",
                BlogCountDisplay = t.BlogCountDisplay
            }).ToList();
        }

        public TagModel GetItem(int id)
        {
            // 1. yöntem:
            //var entity = _tagRepo.Query().Include(t => t.BlogTags).SingleOrDefault(t => t.Id == id);
            //var model = new TagModel()
            //{
            //    Guid = entity.Guid,
            //    Id = entity.Id,
            //    IsPopular = entity.IsPopular,
            //    Name = entity.Name,
            //    IsPopularDisplay = entity.IsPopular ? "Yes" : "No",
            //    BlogCountDisplay = entity.BlogTags.Count 
            //};
            // 2. yöntem:
            var model = Query().Select(t => new TagModel()
            {
                Guid = t.Guid,
                Id = t.Id,
                IsPopular = t.IsPopular,
                Name = t.Name,
                IsPopularDisplay = t.IsPopular ? "Yes" : "No",
                BlogCountDisplay = t.BlogCountDisplay
            }).SingleOrDefault(t => t.Id == id);

            return model;
        }

        public Result Update(TagModel model)
        {
            // güncellenecek kayıt dışında tag adı tabloda tekil olmalı
            if (_tagRepo.Exists(t => t.Name.ToUpper() == model.Name.ToUpper().Trim() && t.Id != model.Id))
                return new ErrorResult("Tag with the same name exists!");

            // burada başka bir yöntem olarak yeni bir entity oluşturmak yerine id üzerinden mevcut
            // bir kaydı çekip özelliklerini model üzerinden güncelliyoruz
            Tag entity = _tagRepo.GetItem(model.Id);

            entity.Name = model.Name.Trim();
            entity.IsPopular = model.IsPopular;

            _tagRepo.Update(entity);

            return new SuccessResult();
        }
    }
}
