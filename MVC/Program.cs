using AppCore.DataAccess.EntityFramework.Bases;
using Business.Services;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MVC.Settings;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#region Localization
// Web uygulamas�n�n b�lgesel ayar� a�a��daki �ekilde tek seferde konfig�re edilerek t�m projenin bu ayar� kullanmas� sa�lanabilir,
// dolay�s�yla veri formatlama veya d�n��t�rme gibi i�lemlerde her seferinde CultureInfo objesinin kullan�m gereksinimi ortadan kalkar.
// Bu �ekilde sadece tek bir b�lgesel ayar projede kullan�labilir.
List<CultureInfo> cultures = new List<CultureInfo>()
{
    new CultureInfo("en-US") // e�er uygulama T�rk�e olacaksa CultureInfo constructor'�n�n parametresini ("tr-TR") yapmak yeterlidir.
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name);
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
#endregion

// Add services to the container.
#region IoC Container
// Alternatif olarak Business katman�nda Autofac ve Ninject gibi k�t�phaneler de kullan�labilir.

// Unable to resolve service hatalar� burada ��z�mlenmelidir.

// AddScoped: istek (request) boyunca objenin referans�n� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak)
// bir kere olu�turulur ve yan�t (response) d�nene kadar bu obje hayatta kal�r.
// AddSingleton: web uygulamas� ba�lad���nda objenin referansn� (genelde interface veya abstract class) kulland���m�z yerde obje (somut class'tan olu�turulacak)
// bir kere olu�turulur ve uygulama �al��t��� (IIS �zerinden uygulama durdurulmad��� veya yeniden ba�lat�lmad���) s�rece bu obje hayatta kal�r.
// AddTransient: istek (request) ba��ms�z ihtiya� olan objenin referans�n� (genelde interface veya abstract class) kulland���m�z her yerde bu objeyi new'ler.
// Genelde AddScoped methodu kullan�l�r.

var connectionString = builder.Configuration.GetConnectionString("Db");
builder.Services.AddDbContext<Db>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(RepoBase<>), typeof(Repo<>));

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRoleService, RoleService>();
#endregion

builder.Services.AddControllersWithViews();

#region Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    // projeye Cookie authentication default'lar�n� kullanarak kimlik do�rulama ekliyoruz

    .AddCookie(config =>
    {
        config.LoginPath = "/Account/Users/Login";
        // e�er sisteme giri� yap�lmadan bir i�lem yap�lmaya �al���l�rsa kullan�c�y� Account area -> Users controller -> Login action'�na y�nlendir

        config.AccessDeniedPath = "/Account/Users/AccessDenied";
        // e�er sisteme giri� yap�ld�ktan sonra yetki d��� bir i�lem yap�lmaya �al���l�rsa kullan�c�y� Account area -> Users controller -> AccessDenied
        // action'�na y�nlendir

        config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        // sisteme giri� yap�ld�ktan sonra olu�an cookie 30 dakika boyunca kullan�labilsin

        config.SlidingExpiration = true;
        // SlidingExpiration true yap�larak kullan�c� sistemde her i�lem yapt���nda cookie kullan�m s�resi yukar�da belirtilen 30 dakika uzat�l�r,
        // e�er false atan�rsa kullan�c�n�n cookie �mr� yukar�da belirtilen 30 dakika sonra sona erer ve yeniden giri� yapmak zorunda kal�r
    });
#endregion

#region Session
builder.Services.AddSession(config =>
{
    config.IdleTimeout = TimeSpan.FromMinutes(30);

    config.IOTimeout = Timeout.InfiniteTimeSpan;
});
#endregion

#region AppSettings
//var section = builder.Configuration.GetSection("AppSettings");
var section = builder.Configuration.GetSection(nameof(AppSettings));
section.Bind(new AppSettings());
#endregion

var app = builder.Build();

#region Localization
app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
});
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

#region Authentication
app.UseAuthentication(); // sen kimsin?
#endregion

app.UseAuthorization(); // sen i�lem i�in yetkili misin?

#region Session
app.UseSession();
#endregion

#region Area
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
