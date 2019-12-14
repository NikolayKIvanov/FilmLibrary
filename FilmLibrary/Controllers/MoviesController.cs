using System;
using System.Threading.Tasks;
using FilmLibrary.Dtos;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FilmLibrary.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMoviesRepository _repository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesRepository"/>
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userRepository"></param>
        public MoviesController(IMoviesRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Searches for the movies in the database by the given search text, type, and category.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <returns>Initializes a new instance of the <see cref="MoviesByCategoryModel"/> in a Json format.</returns>
        public async Task<IActionResult> SearchMoviesInDatabase(string searchText, string type, string category)
        {
            var movies = new MoviesByCategoryModel();
            switch (type)
            {
                case "Title":
                    movies = await _repository.GetMoviesByTitle(_userRepository.GetUserId(this.User), searchText, category);
                    break;
                case "Director":
                    movies = await _repository.GetMoviesByDirector(_userRepository.GetUserId(this.User), searchText, category);
                    break;
                case "Genre":
                    movies = await _repository.GetMoviesByGenre(_userRepository.GetUserId(this.User), searchText, category);
                    break;
                default:
                    break;
            }

            return Json(movies);
        }

        /// <summary>
        /// Searched for movies from the omdb api and gets information about the movies.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>Returns a view with all the movies if the search was successful and if it was not, it redirects you to the home page.</returns>
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Search(IFormCollection collection)
        {
            var result = await _repository.GetTenMoviesByTitle(collection["SearchText"]);
            result.SearchText = collection["SearchText"];
            result.PageNumber = 1;
            return View("MoviesList", result);
        }

        /// <summary>
        /// Gets information about a current movie.
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns>Returns a view with the movie information.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SingleMovie(string imdbId)
        {
            var movie = new SingleMovieViewModel();

            if (User.Identity.IsAuthenticated)
            {
                movie = await _repository.GetMovieByImdbIdForUser(_userRepository.GetUserId(this.User), imdbId);
            }
            else
            {
                movie = await _repository.GetMovieByImdbId(imdbId);
            }

            return View(movie);
        }

        /// <summary>
        /// Adding a movie in a current category.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns>Returns a view with the information about the current movie.</returns>
        public async Task<IActionResult> LinkMovieToUser(Guid movieId, string category)
        {
            var userId = _userRepository.GetUserId(this.User);
            await _repository.AddMovieToUser(userId, movieId, category);
            var movie = await _repository.GetMovieWithGenresById(userId, movieId);
            return View("SingleMovie", movie);
        }

        /// <summary>
        /// Removing a movie from a current category.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns>Returns a view with the movie information.</returns>
        public async Task<IActionResult> RemoveMovieFromUser(Guid movieId, string category)
        {
            var userId = _userRepository.GetUserId(this.User);
            await _repository.RemoveMovieFromUser(userId, movieId, category);
            var movie = await _repository.GetMovieWithGenresById(userId, movieId);
            if (!movie.IsInWatched && movie.IsInFavorites)
            {
                await _repository.RemoveMovieFromUser(userId, movieId, "Favorites");
                movie = await _repository.GetMovieWithGenresById(userId, movieId);
            }

            return View("SingleMovie", movie);
        }

        /// <summary>
        /// Removes a movie from the watchlist category and adding the movie to the watched category.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns>Returns a view with the movie information.</returns>
        public async Task<IActionResult> AddToWatchedRemoveFromUserWatchlist(Guid movieId, string category)
        {
            var userId = _userRepository.GetUserId(this.User);
            await _repository.RemoveMovieFromUser(userId, movieId, "Watchlist");
            var movie = await _repository.GetMovieWithGenresById(userId, movieId);
            movie.IsInWatchlist = false;
            movie.IsInWatched = true;
            await _repository.AddMovieToUser(userId, movie.MovieLong.Id, category);
            return View("SingleMovie", movie);
        }

        /// <summary>
        /// Gets another page with movie information if the movies cannot be displayed on a single page.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="pageNumber"></param>
        /// <returns>Returns a view with a list of movies.</returns>
        public async Task<IActionResult> GetAnotherPage(string searchText, int pageNumber)
        {
            var result = await _repository.GetTenMoviesByTitle(searchText, pageNumber);
            result.SearchText = searchText;
            result.PageNumber = pageNumber;

            return View("MoviesList", result);
        }

        public IActionResult Watchlist()
        {
            return View("Watchlist");
        }

        public IActionResult Favorites()
        {
            return View("Favorites");
        }

        public IActionResult History()
        {
            return View("Watched");
        }

        /// <summary>
        /// Gets all the movies in a current page by given category.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="moviesOnPage"></param>
        /// <param name="category"></param>
        /// <returns>Returns information about the movies in a Json format.</returns>
        [HttpGet]
        public async Task<IActionResult> GetMoviesByCategory(int pageNumber, int moviesOnPage, string category)
        {
            var movies = await _repository.GetMoviesByCategory(_userRepository.GetUserId(this.User), category, pageNumber, moviesOnPage);
            return Json(movies);
        }
    }
}