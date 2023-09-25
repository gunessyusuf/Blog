using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class DbFactory : IDesignTimeDbContextFactory<Db> // Db objesini oluşturup kullanılmasını sağlayan fabrika class'ı,
                                                             // scaffolding işlemleri için bu class oluşturulmalıdır
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=BlogDB;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;");
            // önce veritabanımızın (development veritabanı kullanılması daha uygundur) connection string'ini içeren bir obje oluşturuyoruz

            return new Db(optionsBuilder.Options); // daha sonra yukarıda oluşturduğumuz obje üzerinden Db tipinde bir obje dönüyoruz
        }
    }
}
