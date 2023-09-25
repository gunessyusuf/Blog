#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Business.Models
{
    public class BlogModel : RecordBase // modeller de RecordBase'den miras almalıdır ki hem Id ve Guid alanlarını
                                        // miras alsın hem de servislerde tip olarak kullanılabilsin.
    {
		// ilgili entity'de referans olmayan özellikler veya başka bir deyişle veritabanındaki ilgili tablosundaki
		// sütun karşılığı olan özellikler entity'den kopyalanır.

		// SOLID prensipleri gereği bu class'ta validasyon için data annotation'lar kullanmak yerine MVC FluentValidation
		// gibi bir kütüphane üzerinden başka bir class'ta validasyonları yönetmek daha uygun olacaktır.



		#region Entity'den Kopyalanan Özellikler
		[Required(ErrorMessage = "{0} is required!")] // istenirse view'larda gösterilecek validasyon mesajları bu şekilde özelleştirilebilir,
													  // 0: varsa DisplayName'i (Blog Title) yoksa özellik ismini (Title) kullanır

		//[StringLength(150)] // kullanıcıdan gelen model verisi validasyonu için verinin maksimum 150 karakter olacağını belirtir 

		//[MinLength(3)] // alternatif olarak kullanıcıdan gelen model verisi validasyonu için verinin minimum 3 karakter olacağını belirten data annotation kullanılabilir
		[MinLength(3, ErrorMessage = "{0} must be minimum {1} characters!")] // 0: varsa DisplayName'i (Blog Title) yoksa özellik ismini (Title) kullanır,
																			 // 1: MinLength attribute'unun ilk parametresi olan 3'ü kullanır

		//[MaxLength(150)] // alternatif olarak kullanıcıdan gelen model verisi validasyonu için verinin maksimum 150 karakter olacağını belirten data annotation kullanılabilir
		[MaxLength(150, ErrorMessage = "{0} must be maximum {1} characters!")] // 0: varsa DisplayName'i (Blog Title) yoksa özellik ismini (Title) kullanır,
																			   // 1: MaxLength attribute'unun ilk parametresi olan 150'yi kullanır

		[DisplayName("Blog Title")]  // her view'da elle Blog Title yazmak yerine model üzerinden bu özelliğin DisplayName'ini kullanacağız,
									 // eğer yazılmazsa DisplayName üzerinden özelliğin adı (Title) kullanılır.  
		public string Title { get; set; }



		[Required(ErrorMessage = "{0} is required!")]
		[StringLength(300, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string Content { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }

        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }



		//[Range(1, double.MaxValue)] // isteğe göre kullanıcıdan gelen model verisi validasyonu için verinin pozitif bir double değer olacağını belirten data annotation kullanılabilir
		[Range(1, 5, ErrorMessage = "{0} must be between {1} and {2}!")] // 0: varsa DisplayName'i yoksa özellik ismini (Score) kullanır,
                                                                         // 1: Range constructor'ının birinci parametresi (1),
                                                                         // 2: Range constructor'ının ikinci parametresidir (5)
        public double? Score { get; set; }



		[DisplayName("User")]
		[Required(ErrorMessage = "{0} is required!")]
		public int? UserId { get; set; } // blog'un kullanıcısı view'da bir drop down list üzerinden seçileceği ve Seçiniz (Select) item'ı üzerinden null gönderilebileceği
                                         // için UserId nullable yapılmalıdır ve eğer mutlaka seçim yapılması isteniyorsa Required attribute'u (data annotation)
                                         // ile işaretlenmelidir, bu örnekte seçim yapılmalıdır



        #region Binary Data
        [Column(TypeName = "image")]
        [JsonIgnore]
        public byte[] Image { get; set; }

        [StringLength(5)]
        [JsonIgnore]
        public string ImageExtension { get; set; } // .jpg, .png
        #endregion

        #endregion



        // ihtiyaç halinde view'larda gösterim veya veri girişi için entity verilerini özelleştirip (formatlama,
        // ilişkili entity referansı üzerinden özellik kullanma, vb.) kullanacağımız yeni özellikler oluşturulabilir.
        #region View'larda Gösterim veya Veri Girişi için Kullanacağımız Özellikler
        [DisplayName("Create Date")]
        public string CreateDateDisplay { get; set; }

        [DisplayName("Update Date")]
        public string UpdateDateDisplay { get; set; }

        [DisplayName("User")]
        public string UserNameDisplay { get; set; }

        [DisplayName("Score")]
        [JsonIgnore]
        public string ScoreDisplay { get; set; }

        // Many to Many ilişkili kayıtları getirme 1. yöntem:
        [DisplayName("Tags")]
        public List<TagModel> TagsDisplay { get; set; }

		// Many to Many ilişkili kayıtları getirme 2. yöntem:
		//[DisplayName("Tags")]
		//[Required(ErrorMessage = "{0} are required!")]
		//public string TagsDisplay { get; set; }

		//[Display(Name = "Tags")]
		[DisplayName("Tags")]
		[Required(ErrorMessage = "{0} are required!")]
		public List<int> TagIds { get; set; }

        #region Binary Data
        [DisplayName("Image")]
        public string ImgSrcDisplay { get; set; }
        #endregion

        #endregion



        /*
        Entity ve model özelliklerinde kullanılabilecek bazı genel data annotation'lar (attribute): 
        NOT: Data annotation'lar ile sadece model verisi üzerinden basit validasyonlar yapılabilir, örneğin veritabanındaki bir tablo üzerinden 
        validasyon gerekiyorsa bu validasyon service class'larında yapılmalıdır.

        Key (Entity): Özelliğin birincil anahtar olduğunu belirtir ve veritabanı oluşturulurken tablodaki sütun karşılığı otomatik artan sayı olarak ayarlanır.
        Required (Entity ve Model): Özelliğin zorunlu olduğunu belirtir.
        Column (Entity): Özelliğin veritabanı tablosundaki sütunu ile ilgili ayarlarını belirtir, örneğin sütun adı (Name), sütun veri tipi (TypeName) ve sütun sırası (Order: çoklu key için kullanılır).
        DataType (Model): Özelliğin veri tipi için kullanılır, örneğin Text, Date, Time, DateTime, Currency, EmailAddress, PhoneNumber, Password, v.b.
        ReadOnly (Model): Özelliğin sadece okunabilir olması için kullanılır.
        DisplayFormat (Model): Metinsel veri gösteriminde kullanılacak format'ı belirtir ve genellikle tarih, ondalık sayı, v.b. formatlama işlemleri için kullanılır.
        Table (Entity): Veritabanında oluşacak tablonun adını (Name) değiştirmek için kullanılır.
        StringLength (Entity ve Model): Metinsel tipte özellikler için girilecek karakter sayısının maksimumunu belirtmede kullanılır.
        MinLength (Model): Metinsel tipte özellikler için girilecek karakter sayısının minimumunu belirtmede kullanılır.
        MaxLength (Model): Metinsel tipte özellikler için girilecek karakter sayısının maksimumunu belirtmede kullanılır.
        Compare (Model): Tanımlandığı özelliğin başka bir özellik üzerinden verilerinin karşılaştırılması için kullanılır.
        RegularExpression (Model): Verilerin daha detaylı validasyonu için öğrenilip kullanılabilecek bir doğrulama desenidir.
        Range (Model): Sayısal değerler için aralık belirtmede kullanılır.
        EmailAddress (Model): Özellik verisinin e-posta formatında olması için kullanılır.
        Phone (Model): Özellik verisinin telefon formatında olması için kullanılır.
        NotMapped (Entity): Özelliğin veritabanında ilgili tablosunda sütununun oluşturulmaması için kullanılır.
        JsonIgnore (Model): Özelliğin oluşturulacak JSON formatındaki veriye dahil edilmemesini sağlar.
        */
    }
}
