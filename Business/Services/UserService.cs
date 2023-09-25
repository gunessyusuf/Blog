using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess;

namespace Business.Services
{
    public interface IUserService : IService<UserModel>
    {
        // ihtiyaca göre uygulamamızın kullanacağı servisler bazında ihtiyaç duyulan method tanımları
        // burada yapılarak bu interface'i implemente eden tüm sınıflarda bu method tanımları üzerinden
        // methodların oluşturulması sağlanabilir ve bu method tanımları controller'larda servisler
        // üzerinden çağrılabilir
        List<UserModel> GetList();
        List<UserModel> GetListByRole(int? roleId = null);
    }

    public class UserService : IUserService
    {
        private readonly RepoBase<User> _userRepo;

        public UserService(RepoBase<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public Result Add(UserModel model)
        {
            List<User> users = _userRepo.GetList();

            if (users.Any(u => u.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)))
                return new ErrorResult("User with the same name exists!");

            User entity = new User()
            {
                IsActive = model.IsActive,
                Password = model.Password,
                RoleId = model.RoleId,
                UserName = model.UserName
            };

            _userRepo.Add(entity);

            return new SuccessResult();
        }

        public Result Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _userRepo.Dispose();
        }

        public List<UserModel> GetList()
        {
            // 1. yöntem:
            //return _userRepo.GetList().OrderByDescending(u => u.IsActive).ThenBy(u => u.UserName)
            //    .Select(u => new UserModel(u.UserName, u.Password, u.IsActive, u.RoleId, u.Id)).ToList();

            // 2. yöntem:
            return Query().ToList();
        }

		public List<UserModel> GetListByRole(int? roleId = null)
		{
            if (roleId is null)
                return new List<UserModel>();
            return Query().Where(q => q.RoleId == roleId).ToList();
		}

		public IQueryable<UserModel> Query()
        {
            return _userRepo.Query().OrderByDescending(u => u.IsActive).ThenBy(u => u.UserName)
                .Select(u => new UserModel()
                {
                    UserName = u.UserName, 
                    Password = u.Password, 
                    IsActive= u.IsActive, 
                    RoleId = u.RoleId, 
                    Id = u.Id,
                    Role = new RoleModel()
                    {
                        Name = u.Role.Name
                    }
                });
        }

        public Result Update(UserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
