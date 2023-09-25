using AppCore.Results.Bases;

namespace AppCore.Results
{
    /// <summary>
    /// Servis class'larında çeşitli methodlardan başarılı olarak dönecek işlem sonucu class'ı.
    /// </summary>
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true, message) // Result class'ının constructor'ına isSuccessful parametresini true gönderiyoruz ki sonuç başarılı olsun
        {
        }

        public SuccessResult() : base(true, "") // mesaj göndermeden başarılı işlem sonucu oluşturmak için
        {
        }
    }
}
