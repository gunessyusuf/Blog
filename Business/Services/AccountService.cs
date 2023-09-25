using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using Business.Models.Account;
using DataAccess.Enums;

namespace Business.Services
{
    public interface IAccountService // IAccountService'i IService'ten implemente etmiyoruz çünkü bu servis UserService enjeksiyonu üzerinden login ve register işlerini yapacak,
                                     // CRUD işlemlerinin hepsini yapmayacak, bu yüzden Login ve Register methodlarını içerisinde tanımlıyoruz
    {
        Result Login(AccountLoginModel accountLoginModel, UserModel userResultModel); // kullanıcıların kullanıcı girişi için
                                                                                      // accountLoginModel view üzerinden kullanıcıdan aldığımız verilerdir,
                                                                                      // userResultModel ise accountLoginModel'deki doğru verilere göre kullanıcıyı veritabanından çektikten sonra method içerisinde atayacağımız ve
                                                                                      // referans tip olduğu için de Login methodunu çağırdığımız yerde kullanabileceğimiz sonuç kullanıcı objesidir,
        Result Register(AccountRegisterModel model);                                                                           // böylelikle Login methodundan hem login işlem sonucunu Result olarak hem de işlem başarılıysa kullanıcı objesini UserModel objesi olarak dönebiliyoruz
    }

    public class AccountService : IAccountService
    {
        private readonly IUserService _userService; // CRUD işlemlerini yaptığımız UserService objesini bu servise enjekte ediyoruz ki Query methodu üzerinden Login,
                                                    // Add methodu üzerinden de Register işlemleri yapabilelim
        public AccountService(IUserService userService)
        {
            _userService = userService;
        }

        public Result Login(AccountLoginModel accountLoginModel, UserModel userResultModel) // kullanıcı girişi
        {
            // önce accountLoginModel üzerinden kullanıcının girmiş olduğu kullanıcı adı ve şifreye sahip aktif kullanıcı sorgusu üzerinden veriyi çekip user'a atıyoruz,
            // kullanıcı adı ve şifre hassas veri olduğu için trim'lemiyoruz ve büyük küçük harf duyarlılığını da ortadan kaldırmıyoruz
            var user = _userService.Query().SingleOrDefault(u => u.UserName == accountLoginModel.UserName &&
                u.Password == accountLoginModel.Password && u.IsActive);

            if (user is null) // eğer böyle bir kullanıcı bulunamadıysa
                return new ErrorResult("Invalid user name or password!"); // kullanıcı adı veya şifre hatalı sonucunu dönüyoruz

            // burada kullanıcı bulunmuş demektir dolayısıyla referans tip olduğu için userResultModel'i yukarıda çektiğimiz user'a göre dolduruyoruz,
            // dolayısıyla hem sorgulanan kullanıcı objesi (userResultModel) hem de işlem sonucunu SuccessResult objesi olarak methoddan dönüyoruz,
            // Account area -> Users controller -> Login action'ında sadece kullanıcı adı ve role ihtiyacımız olduğu için objemizi bu özellikler üzerinden dolduruyoruz
            userResultModel.UserName = user.UserName;
            userResultModel.Role.Name = user.Role.Name;
            userResultModel.Id = user.Id;

            return new SuccessResult();
        }

        public Result Register(AccountRegisterModel model)
        {
            // CAGIL -> cagıl -> cagil
            //List<UserModel> users = _userService.Query().ToList();

            //if (users.Any(u => u.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)))
            //    return new ErrorResult("User with the same user name exists!");

            UserModel userModel = new UserModel()
            {
                IsActive = true,
                Password = model.Password,
                RoleId = (int)Roles.User,
                UserName = model.UserName
            };

            return _userService.Add(userModel);
        }
    }
}
