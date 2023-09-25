#nullable disable


using System.ComponentModel;

namespace MVC.Models
{
    public class FavoritesModel
    {
        public int BlogId { get; set; }

        public int UserId { get; set; }

        [DisplayName("Blog Title")]
        public string BlogTitle { get; set; }

        [DisplayName("Blog Score")]
        public double BlogScore { get; set; }

        public FavoritesModel(int blogId, int userId, string blogTitle, double blogScore)
        {
            BlogId = blogId;
            UserId = userId;
            BlogTitle = blogTitle;
            BlogScore = blogScore;
        }
    }
}
