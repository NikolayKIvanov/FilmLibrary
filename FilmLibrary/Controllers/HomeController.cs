using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FilmLibrary.Models;
using System.Security.Claims;

namespace FilmLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoviesRepository _repository;

        public HomeController(IMoviesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(bool isResponseFalse = false)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var userIdParsed = Guid.Parse(userId);
                var indexModel = new IndexViewModel
                {
                    RecommendedMovies = await _repository.GetRecommendedMovies(userIdParsed),
                    TopRatedMovies = await _repository.GetTopRatedMovies()
                };

                return View(indexModel);
            };

            return View(new IndexViewModel
            {
                RecommendedMovies = new List<MovieShort>(),
                TopRatedMovies = await _repository.GetTopRatedMovies()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
