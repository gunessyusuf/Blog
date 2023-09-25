#nullable disable

using AppCore.DataAccess.Bases;
using AppCore.Records.Bases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore.DataAccess.EntityFramework.Bases
{
    // Repository Pattern: Veritabanındaki tablolarda (entity) kolay ve merkezi olarak CRUD (create, read, update, delete)
    // işlemlerinin yapılmasını sağlayan tasarım desenidir (design pattern). Önce DbSet'ler üzerinde istenilen değişiklikler yapılır
    // daha sonra tek bir iş olarak veritabanına yapılan değişiklikler SQL sorguları çalıştırılarak yansıtılır (Unit of Work).



    //public abstract class RepoBase<TEntity> 
    // Tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : class 
    // Referans tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : class, new() 
    // new'lenebilen referans tip olarak TEntity üzerinden herhangi bir tip kullanacak abstract class.

    //public abstract class RepoBase<TEntity> : where TEntity : RecordBase, new() 
    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak abstract class.



    /// <summary>
    /// new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak,
    /// aynı zamanda IDisposable interface'ini IRepoBase interface'i ile birlikte implemente edecek abstract class.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class RepoBase<TEntity> : IRepoBase<TEntity> where TEntity : RecordBase, new()
    {
        protected readonly DbContext _dbContext; // DbContext EntityFramework'ün CRUD işlemleri yapmamızı sağlayan temel class'ı,
                                                 // readonly olarak sadece constructor üzerinden veya bu satırda set edilebilir.
                                                 // protected erişim bildirgeci ile _dbContext'in ihtiyaç halinde sadece bu class'tan miras alan repository'lerde kullanılması sağlanır.

        protected RepoBase(DbContext dbContext) // dbContext Dependency Injection (Constructor Injection) ile RepoBase'e dışarıdan new'lenerek enjekte edilecek.
        {
            _dbContext = dbContext;
        }



        #region Temel Repository Methodları
        /// <summary>
        /// Read işlemi: ilgili entity için sorguyu oluşturur ancak çalıştırmaz.
        /// Sorguyu çalıştırmak için ToList, SingleOrDefault, vb. methodları çağrılmalıdır.
        /// isNoTracking parametresi false gönderildiğinde sorgu üzerinden dönülen DbSet'te değişikliklerin
        /// takip edilmesi sağlanır, değikiliklerin takip edilmemesi için isNoTracking parametresi true gönderilmelidir.
        /// virtual tanımladık ki bu class'dan miras alan class'larda ihtiyaca göre bu method ezilebilsin ve
        /// implementasyonu özelleştirilebilsin.
        /// </summary>
        /// <param name="isNoTracking"></param>
        /// <returns>IQueryable</returns>
        public virtual IQueryable<TEntity> Query(bool isNoTracking = false)
        {
            if (isNoTracking)
                return _dbContext.Set<TEntity>().AsNoTracking();
            return _dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Create işlemi: gönderilen entity'yi DbSet'e ekler ve eğer save parametresi true ise değişikliği 
        /// Save methodu üzerinden veritabanına yansıtır.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        public virtual void Add(TEntity entity, bool save = true)
        {
            entity.Guid = Guid.NewGuid().ToString(); // her eklenecek kayıt için tekil bir Guid oluşturup entity'e atıyoruz ki istenirse Id yerine Guid üzerinden de işlemler yapılabilsin

            //_dbContext.Set<TEntity>().Add(entity); // aşağıdaki satır ile de ekleme işlemi yapılabilir.
            _dbContext.Add(entity);

            if (save)
                Save();
        }

        /// <summary>
        /// Update işlemi: gönderilen entity'yi DbSet'te günceller ve eğer save parametresi true ise değişikliği 
        /// Save methodu üzerinden veritabanına yansıtır.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        public virtual void Update(TEntity entity, bool save = true)
        {
            //_dbContext.Set<TEntity>().Update(entity); // aşağıdaki satır ile de güncelleme işlemi yapılabilir.
            _dbContext.Update(entity);

            if (save)
                Save();
        }

        /// <summary>
        /// Delete işlemi: gönderilen entity'yi DbSet'ten çıkarır, Save methodu ile de save parametresi true gönderildiyse
        /// silme işlemini veritabanına yansıtır.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        public virtual void Delete(TEntity entity, bool save = true)
        {
            //_dbContext.Set<TEntity>().Remove(entity); // aşağıdaki satır ile de silme işlemi yapılabilir.
            _dbContext.Remove(entity);

            if (save)
                Save();
        }

        /// <summary>
        /// DbSet'lerdeki tüm değişikliklerden sonra oluşturulacak sorguların (insert, update ve delete) tek seferde 
        /// veritabanında çalıştırılması: Unit of Work SaveChanges methodu ile sorgunun çalıştırılması sonucunda 
        /// etkilenen kayıt sayısı dönülebilir.
        /// </summary>
        /// <returns>int</returns>
        public virtual int Save()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                // eğer istenirse buraya loglama kodları yazılarak hata alındığında örneğin exc.Message üzerinden logların
                // veritabanında, dosyada veya Windows Event Log'da tutulması sağlanabilir.

                throw exc; // hatayı SaveChanges methodunu çağırdığımız methoda fırlatıyoruz.
            }
        }

        /// <summary>
        /// Garbage Collector'a işimizin bittiğini söylüyoruz ki objeyi en kısa sürede hafızadan temizlesin.
        /// </summary>
        public void Dispose()
        {
            _dbContext?.Dispose(); // ?: _dbContext null ise bu satırı atla, değilse Dispose et.
            GC.SuppressFinalize(this);
        }
        #endregion



        #region Temel Repository Methodlarını Kullanan ve CRUD İşlemlerini Kolaylaştıran Methodlar
        /// <summary>
        /// Read işlemi: yukarıdaki temel isNoTracking parametreli Query methodunun predicate (koşul veya koşullar) 
        /// ile bool sonuç dönen, bir veya isteğe göre daha fazla koşulun and ya da or ile birleştirilerek sorguyu 
        /// where ile filtreleyen ve dönen overload methodu.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isNoTracking"></param>
        /// <returns>IQueryable</returns>
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool isNoTracking = false)
        {
            return Query(isNoTracking).Where(predicate);
        }

        /// <summary>
        /// Read işlemi: bu class'ta belirtilen tip dışında belirtilen başka bir entity tipi üzerinden sorgu oluşturmamızı 
        /// sağlayan overload methodu, where tip tanımlanan methodlarda da class'larda kullanıldığı şekilde kullanılabilir. 
        /// </summary>
        /// <typeparam name="TRelationalEntity"></typeparam>
        /// <returns>IQueryable</returns>
        public virtual IQueryable<TRelationalEntity> Query<TRelationalEntity>() where TRelationalEntity : class, new()
        {
            return _dbContext.Set<TRelationalEntity>();
        }

        /// <summary>
        /// Read işlemi: yukarıdaki temel isNoTracking parametreli Query methodunu çağırarak dönen sorgu üzerinden 
        /// liste oluşturup geri döner.
        /// </summary>
        /// <param name="isNoTracking"></param>
        /// <returns>List</returns>
        public virtual List<TEntity> GetList(bool isNoTracking = false)
        {
            return Query(isNoTracking).ToList();
        }

        /// <summary>
        /// Read işlemi: yukarıdaki predicate ve isNoTracking parametreli Query methodunu çağırarak dönen sorgu üzerinden
        /// liste oluşturup geri döner. 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isNoTracking"></param>
        /// <returns>List</returns>
        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, bool isNoTracking = false)
        {
            return Query(predicate, isNoTracking).ToList();
        }

        /// <summary>
        /// Read işlemi: yukarıdaki temel isNoTracking parametreli Query methodunu çağırarak dönen sorgu üzerinden
        /// parametre olarak gönderilen id'ye sahip tek bir kaydı geri döner. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isNoTracking"></param>
        /// <returns>TEntity</returns>
        public virtual TEntity GetItem(int id, bool isNoTracking = false)
        {
            return Query(isNoTracking).SingleOrDefault(entity => entity.Id == id);
        }

        /// <summary>
        /// Read işlemi: yukarıdaki temel isNoTracking parametreli Query methodunu çağırarak dönen sorgu üzerinden
        /// parametre olarak gönderilen predicate koşul veya koşullara sahip herhangi bir kayıt
        /// varsa true, yoksa false döner. 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isNoTracking"></param>
        /// <returns>bool</returns>
        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate, bool isNoTracking = false)
        {
            return Query(isNoTracking).Any(predicate);
        }

        /// <summary>
        /// Delete işlemi: gönderilen id üzerinden tek bir entity'e ulaşır ve DbSet'ten entity'i çıkarır.
        /// Save methodu ile save parametresi true gönderildiyse değişikliği veritabanına yansıtır.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="save"></param>
        public virtual void Delete(int id, bool save = true)
        {
            var entity = GetItem(id, false); // GetItem methodu id'ye göre tek bir obje döner,
                                             // isNoTracking parametresini özellikle false gönderiyoruz ki entity değişikliği
                                             // takip edilip entity silindi olarak işaretlenebilsin.

            Delete(entity, save); // entity'yi yukarıdaki temel delete methodu üzerinden DbSet'ten çıkarıyoruz ve
                                  // değişiklikliğin veritabanına yansıyıp yansımaması için de save parametresini gönderiyoruz.
        }

        /// <summary>
        /// Delete işlemi: gönderilen koşul veya koşullar üzerinden entity'lere ulaşır, DbSet'ten entity'leri çıkarır,
        /// son olarak Save methodu ile save parametresi true gönderildiyse tüm değişiklikleri tek seferde veritabanına yansıtır. 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="save"></param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate, bool save = true)
        {
            var entities = GetList(predicate, false); // GetList methodu predicate (koşul) üzerinden sorgu sonucundan dönen
                                                      // kayıtları bir obje listesi olarak döner, isNoTracking parametresini
                                                      // özellikle false gönderiyoruz ki tüm entity değişiklikleri takip edilip
                                                      // entity'ler silindi olarak işaretlenebilsin.

            //_dbContext.Set<TEntity>().RemoveRange(entities); // aşağıdaki kodlar ile de silme işlemi yapılabilir.
            foreach (var entity in entities) // döngü üzerinden listedeki her bir entity'yi veritabanına yanıstmayacak şekilde
                                             // yukarıdaki temel Delete methodu üzerinden DbSet'ten çıkarıyoruz.
                Delete(entity, false);
            
            if (save) // eğer methoda gönderilen save parametresi true ise yukarıda DbSet üzerinde yaptığımız değişiklikleri
                      // tek seferde veritabanına yansıtıyoruz.
                Save();
        }

        /// <summary>
        /// Delete işlemi: TEntity tipindeki entity'nin başka entity tipindeki bir referansı üzerinden ilişkilerini tutan 
        /// TRelationalEntity tipi ile bir veya daha fazla koşul için TEntity tipindeki entity'nin ilişkili kayıtlarının 
        /// silinmesini sağlar, bu method çağrıldıktan sonra genelde TEntity tipindeki entity güncellendiği veya silindiği 
        /// için save parametresi default false olarak atanmıştır. Dolayısıyla TEntity tipindeki entity de güncellendikten 
        /// veya silindikten sonra değişiklikler Save methodu çağrılarak tek seferde veritabanına yansıtılabilir.
        /// TEntity tipi dışındaki generic tipler bu methodda kullanılabilir.
        /// </summary>
        /// <typeparam name="TRelationalEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="save"></param>
        public virtual void Delete<TRelationalEntity>(Expression<Func<TRelationalEntity, bool>> predicate, bool save = false) where TRelationalEntity : class, new()
        {
            var relationalEntities = Query<TRelationalEntity>().Where(predicate).ToList(); // yukarıdaki ilişkili entity'ler için
                                                                                           // oluşturduğumuz Query methodundan
                                                                                           // listeyi çekiyoruz.

            _dbContext.Set<TRelationalEntity>().RemoveRange(relationalEntities); // daha sonra ilişkili entity DbSet'inden çektiğimiz
                                                                                 // ilişkili entity listesini siliyoruz.
            
            if (save)
                Save();
		}
        #endregion
    }
}