namespace AppCore.Records.Bases
{
    /// <summary>
    /// İlişki entity'leri dışında tüm entity'lerin ve model'lerin miras alacağı ve veritabanındaki entity'lerin 
    /// karşılığı olan tablolarda sütunları oluşacak özellikler.
    /// </summary>
    public abstract class RecordBase
    {
        public int Id { get; set; } // Id zorunlu olsun
        public string? Guid { get; set; } // Guid zorunlu olmasın
    }
}
