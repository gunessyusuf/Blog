using AppCore.Records.Bases;
using AppCore.Results.Bases;

namespace AppCore.Business.Services.Bases
{
    // kullanıcı ile etkileşimde bulunacak model ile veritabanındaki entity dönüşümlerinin gerçekleştirildiği,
    // aynı zamanda veri formatlama ve validasyon gibi işlemlerin yapılabileceği servis class'larının interface'i.



    // public interface IService<TModel>
    // interface'in implemente edileceği class'larda herhangi bir TModel tipi üzerinden işlemlerin yapılabileceği interface.

    // public interface IService<TModel> : where TModel : class
    // interface'in implemente edileceği class'larda referans bir TModel tipi üzerinden işlemlerin yapılabileceği interface.

    // public interface IService<TModel> : where TModel : class, new()
    // interface'in implemente edileceği class'larda new'lenebilen bir referans TModel tipi üzerinden işlemlerin yapılabileceği interface.



    /// <summary>
    /// new'lenebilen ve RecordBase'den miras alan tipler üzerinden işlemlerin yapılabileceği ve IDisposable interface'ini implemente edecek interface. 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IService<TModel> : IDisposable where TModel : RecordBase, new()
    {
        /// <summary>
        /// Read işlemleri
        /// </summary>
        IQueryable<TModel> Query();

        /// <summary>
        /// Create işlemleri
        /// </summary>
        Result Add(TModel model);

        /// <summary>
        /// Update işlemleri
        /// </summary>
        Result Update(TModel model);

        /// <summary>
        /// Delete işlemleri
        /// </summary>
        Result Delete(int id); 
    }
}
