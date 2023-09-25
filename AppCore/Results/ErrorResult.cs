using AppCore.Results.Bases;

namespace AppCore.Results
{
    /// <summary>
    /// Servis class'larında çeşitli methodlardan başarısız olarak dönecek işlem sonucu class'ı.
    /// </summary>
    public class ErrorResult : Result
    {
        public ErrorResult(string message) : base(false, message) // Result class'ının constructor'ına isSuccessful parametresini false gönderiyoruz ki sonuç başarısız olsun
        {
        }

        public ErrorResult() : base(false, "") // mesaj göndermeden başarısız işlem sonucu oluşturmak için
        {
        }
    }
}
