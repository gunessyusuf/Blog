using AppCore.Records.Bases;

namespace AppCore.DataAccess.Bases
{
    //public interface IRepoBase<TEntity> 
    // Tip olarak TEntity üzerinden herhangi bir tip kullanacak interface.

    //public interface IRepoBase<TEntity> : where TEntity : class
    // Referans tip olarak TEntity üzerinden herhangi bir tip kullanacak interface.

    //public interface IRepoBase<TEntity> : where TEntity : class, new()
    // new'lenebilen referans tip olarak TEntity üzerinden herhangi bir tip kullanacak interface.

    //public interface IRepoBase<TEntity> : where TEntity : RecordBase, new() 
    // new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak interface.



    /// <summary>
    /// new'lenebilen ve RecordBase'den miras alan tip olarak TEntity üzerinden entity tipini kullanacak
    /// ve IDisposable interface'indeki Dispose method tanımını da içeren interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepoBase<TEntity> : IDisposable where TEntity : RecordBase, new()
    {
        /// <summary>
        /// Entity tipindeki kayıtları getiren sorgu method tanımı.
        /// Eğer isNoTracking true ise dönülen sorgu üzerinden entity DbSet'indeki
        /// değişikliklerin takip edilmemesi sağlanır, false ise değişiklikler takip edilir.
        /// </summary>
        /// <param name="isNoTracking"></param>
        /// <returns>IQueryable</returns>
        IQueryable<TEntity> Query(bool isNoTracking = false);
        
        /// <summary>
        /// Entity tipindeki kayıtların ilgili tablosuna eklenmesini sağlayan method tanımı.
        /// Eğer save parametresi true gönderilirse entity parametresi ilgili entity kümesine eklenerek
        /// ekleme işlemi veritabanına yansıtılır, save parametresi false gönderilirse
        /// çoklu entity ekleme işlemlerinde önce ilgili entity kümesine entity'ler eklenebilir ve
        /// daha sonra Save methodu çağrılarak bu kümedeki tüm eklemeler tek seferde veritabanına yansıtılabilir.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        void Add(TEntity entity, bool save = true);

        /// <summary>
        /// Entity tipindeki kayıtların ilgili tablosunda güncellenmesini sağlayan method tanımı.
        /// Eğer save parametresi true gönderilirse entity parametresi ilgili entity kümesinde güncellenerek
        /// güncelleme işlemi veritabanına yansıtılır, save parametresi false gönderilirse
        /// çoklu entity güncelleme işlemlerinde önce ilgili entity kümesinde entity'ler güncellenebilir ve
        /// daha sonra Save methodu çağrılarak bu kümedeki tüm güncellemeler tek seferde veritabanına yansıtılabilir.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        void Update(TEntity entity, bool save = true);

        /// <summary>
        /// Entity tipindeki kayıtların ilgili tablosundan entity parametresi üzerinden silinmesini 
        /// sağlayan method tanımı. Eğer save parametresi true gönderilirse entity
        /// ilgili DbSet'ten silinerek silme işlemi veritabanına yansıtılır, save parametresi false gönderilirse
        /// çoklu entity silme işlemlerinde önce ilgili DbSet'ten entity'ler bir döngü üzerinden 
        /// bu method çağrılarak tek tek silinebilir, daha sonra Save methodu çağrılarak bu kümedeki tüm silmeler 
        /// tek seferde veritabanına yansıtılabilir.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="save"></param>
        void Delete(TEntity entity, bool save = true);

        /// <summary>
        /// Add, Update veya Delete methodları çağrıldıktan sonra entity kümeleri üzerinden yapılan tüm işlemlerin
        /// tek seferde veritabanına yansıtılmasını sağlayan method tanımı. Yapılan işlem veya işlemler sonucunda 
        /// ilgili entity tablolarında etkilenen satır sayısını geri döner. İhtiyaç halinde exception loglamaları
        /// bu method içerisinde gerçekleştirilebilir. 
        /// </summary>
        /// <returns>int</returns>
        int Save();
    }
}
