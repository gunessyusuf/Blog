using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MVC.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        const string SESSIONKEY = "favorites";

        int _userId;

        private readonly IBlogService _blogService;

        public FavoritesController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult GetFavorites()
        {
            _userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var favoritesList = GetSession(_userId);

            return View("Favorites", favoritesList);
        }

        public IActionResult AddToFavorites(int blogId)
        {
            _userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var favoritesList = GetSession(_userId);

            var blog = _blogService.Query().SingleOrDefault(b => b.Id == blogId);

            if (favoritesList.Any(f => f.BlogId == blogId && f.UserId == _userId))
            {
                TempData["Message"] = $"\"{blog.Title}\" already added to favorites.";
            }
            else
            {
                var favoritesItem = new FavoritesModel(blogId, _userId, blog.Title, blog.Score ?? 0);

                favoritesList.Add(favoritesItem);

                SetSession(favoritesList);

                TempData["Message"] = $"\"{blog.Title}\" added to favorites.";
            }

            return RedirectToAction("Index", "Blogs");
        }

        public IActionResult RemoveFromFavorites(int blogId, int userId)
        {
            _userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var favoritesList = GetSession(_userId);

            // 1. yöntem:
            //var favoritesItem = favoritesList.SingleOrDefault(f => f.BlogId == blogId && f.UserId == userId);
            //favoritesList.Remove(favoritesItem);

            // 2. yöntem:
            favoritesList.RemoveAll(f => f.BlogId == blogId && f.UserId == userId);

            SetSession(favoritesList);

            return RedirectToAction(nameof(GetFavorites));
        }

        public IActionResult ClearFavorites()
        {
            _userId = Convert.ToInt32(User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value);

            var favoritesList = GetSession(_userId);

            favoritesList.RemoveAll(f => f.UserId == _userId);

            SetSession(favoritesList);

            return RedirectToAction(nameof(GetFavorites));
        }

        private List<FavoritesModel> GetSession(int userId)
        {
            var favoritesList = new List<FavoritesModel>();

            var favoritesJson = HttpContext.Session.GetString(SESSIONKEY);

            if (!string.IsNullOrWhiteSpace(favoritesJson))
            {
                favoritesList = JsonConvert.DeserializeObject<List<FavoritesModel>>(favoritesJson);

                favoritesList = favoritesList.Where(f => f.UserId == userId).ToList();
            }

            return favoritesList;
        }

        private void SetSession(List<FavoritesModel> favoritesList)
        {
            var favoritesJson = JsonConvert.SerializeObject(favoritesList);

            HttpContext.Session.SetString(SESSIONKEY, favoritesJson);
        }
    }
}
