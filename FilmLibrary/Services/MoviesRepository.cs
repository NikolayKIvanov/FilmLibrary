using AutoMapper;
using FilmLibrary.Dtos;
using FilmLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FilmLibrary.Services
{
    public class MoviesRepository : IMoviesRepository
    {
        private const int NumberOfMoviesOnRow = 4;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesRepository"/> class.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        public MoviesRepository(DataContext dataContext, IConfiguration configuration, IMapper mapper)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the movie by id for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imdbId"></param>
        /// <returns>Instance of class <see cref="MoviesRepository"/> corresponding to the user and the id of the movie.</returns>
        public async Task<SingleMovieViewModel> GetMovieByImdbIdForUser(Guid userId, string imdbId)
        {
            var movie = await GetMovieByImdbId(imdbId);

            return new SingleMovieViewModel
            {
                MovieLong = movie.MovieLong,
                IsInFavorites = IsUserLinkedToMovieByCategory(userId, movie.MovieLong.Id, "Favorites"),
                IsInWatched = IsUserLinkedToMovieByCategory(userId, movie.MovieLong.Id, "Watched"),
                IsInWatchlist = IsUserLinkedToMovieByCategory(userId, movie.MovieLong.Id, "Watchlist")
            };
        }

        /// <summary>
        /// Gets the movie by the genre we want.
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="searchText"></param>
        /// <param name="category"></param>
        /// <returns>Instance of class <see cref="MoviesByCategoryModel"/> corresponding to movies we searched with the current genre.</returns>
        public async Task<MoviesByCategoryModel> GetMoviesByGenre(Guid userid, string searchText, string category)
        {
            var searchTextToLower = searchText.ToLower();

            var genres = _dataContext.Genres.Where(g => g.Name.ToLower().Contains(searchTextToLower));

            var genreMovies = new List<MovieGenre>();
            foreach (var genre in genres)
            {
                genreMovies.AddRange(_dataContext.Movies_Genres.Where(mG => mG.GenreId == genre.Id));
            }

            var movies = new List<Movie>();
            foreach (var genreMovie in genreMovies)
            {
                movies.Add(await _dataContext.Movies.SingleOrDefaultAsync(m => m.Id == genreMovie.MovieId));
            }

            movies = movies.Distinct().ToList();

            return new MoviesByCategoryModel
            {
                Movies = movies,
                PageNumber = 1,
                TotalCount = movies.Count
            };
        }

        /// <summary>
        /// Adds the current movie for the current user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task AddMovieToUser(Guid userId, Guid movieId, string category)
        {
            var newUserMovie = new UserMovie
            {
                UserId = userId,
                MovieId = movieId,
                Category = category
            };

            _dataContext.Users_Movies.Add(newUserMovie);
            await _dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the movie from the user's movie list.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task RemoveMovieFromUser(Guid userId, Guid movieId, string category)
        {
            var userMovie = _dataContext.Users_Movies.FirstOrDefault(uM => uM.UserId == userId && uM.MovieId == movieId && uM.Category == category);

            _dataContext.Users_Movies.Remove(userMovie);
            await _dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Checks whether or not the movie is in the database and if it's not it searches from omdb.
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns>Instance of class <see cref="SingleMovieViewModel"/> searched by imdbId with corresponding movie information.</returns>
        public async Task<SingleMovieViewModel> GetMovieByImdbId(string imdbId)
        {
            var movie = await _dataContext.Movies.FirstOrDefaultAsync(m => m.ImdbId == imdbId);
            var movieLong = new MovieLong();
            if (movie == null)
            {
                movieLong = await GetMovieByIdFromApi(imdbId);

                var newMovie = _mapper.Map<MovieLong, Movie>(movieLong);
                newMovie.Id = Guid.NewGuid();
                newMovie.Poster = $"{imdbId}.jpg";

                _dataContext.Movies.Add(newMovie);

                if (movieLong.Genre != "N/A")
                {
                    var genres = movieLong.Genre.Split(",").Select(genre => genre.Trim()).ToList();
                    await AddGenresToMovie(newMovie.Id, genres);
                }

                if (movieLong.Actors != "N/A")
                {
                    var actors = movieLong.Actors.Split(',').Select(genre => genre.Trim()).ToList();
                    await AddActorsToMovie(newMovie.Id, actors);
                }

                if (movieLong.Production != "N/A")
                {
                    var productions = movieLong.Production.Split(',').Select(genre => genre.Trim()).ToList();
                    await AddProductionToMovie(newMovie.Id, productions);
                }

                await _dataContext.SaveChangesAsync();

                movieLong.Id = newMovie.Id;
                movieLong.Poster = newMovie.Poster;
                return new SingleMovieViewModel
                {
                    MovieLong = movieLong
                };
            }

            movieLong = MovieLong.MovieWithGenres(movie, await GetGenresForMovie(movie.Id));

            return new SingleMovieViewModel
            {
                MovieLong = movieLong
            };
        }

        /// <summary>
        /// Searches for a list of movies by the director of the movies.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchText"></param>
        /// <param name="category"></param>
        /// <returns>Instance of class <see cref="MoviesByCategoryModel"/> corresponding to the list of movies searched by director name</returns>
        public async Task<MoviesByCategoryModel> GetMoviesByDirector(Guid userId, string searchText, string category)
        {
            var searchTextLower = searchText.ToLower();

            var userMovies = _dataContext.Users_Movies.Where(uM => uM.UserId == userId && uM.Category == category).ToList();

            var movies = new List<Movie>();

            foreach (var userMovie in userMovies)
            {
                movies.Add(await _dataContext.Movies.SingleOrDefaultAsync(m => m.Id == userMovie.MovieId));
            }

            movies = movies.Where(m => m.Director.ToLower().Contains(searchTextLower)).ToList();

            return new MoviesByCategoryModel
            {
                Movies = movies,
                PageNumber = 1,
                TotalCount = movies.Count
            };
        }

        /// <summary>
        /// Gets all the movies in the current category. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="category"></param>
        /// <param name="pageNumber"></param>
        /// <param name="moviesOnPage"></param>
        /// <returns>Instance of class <see cref="MoviesByCategoryModel"/> with all the movies in the category.</returns>
        public async Task<MoviesByCategoryModel> GetMoviesByCategory(Guid userId, string category, int pageNumber = 1, int moviesOnPage = 1)
        {
            var userMovies = GetMoviesIdByUserId(userId, pageNumber, category, moviesOnPage);

            var moviesByCategory = new MoviesByCategoryModel
            {
                Movies = new List<Movie>(),
                TotalCount = _dataContext.Users_Movies
                    .Count(mG => mG.UserId == userId && mG.Category.Equals(category)),
                PageNumber = pageNumber
            };

            foreach (var userMovie in userMovies)
            {
                var movie = await GetMovieById(userMovie.MovieId);
                moviesByCategory.Movies.Add(movie);
            }

            return moviesByCategory;
        }

        /// <summary>
        /// Gets information about movies searched by title from the omdb api.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="page"></param>
        /// <returns>Instance of class <see cref="MultipleMoviesModelView"/> with the information about movies found.</returns>
        public async Task<MultipleMoviesModelView> GetTenMoviesByTitle(string title, int page = 1)
        {
            using (var client = new HttpClient())
            {
                var key = "ImdbApi";

                var response = await client.GetStringAsync(new Uri($"http://www.omdbapi.com/?s={title}&apikey={_configuration.GetSection(key).Value}&page={page}&type=movie"));

                var result = JsonConvert.DeserializeObject<MultipleMoviesModelView>(response);

                if (result.Response == "False")
                {
                    return result;
                }

                foreach (var movie in result.MovieShorts)
                {
                    DownloadPoster(movie.Poster, movie.ImdbId, out string posterName);
                    movie.Poster = posterName;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets all the movies and orders them by descending order by the ratings of the movies.
        /// </summary>
        /// <returns>Instance of class <see cref="MovieShort"/> with information about the movies.</returns>
        public async Task<List<MovieShort>> GetTopRatedMovies()
        {
            var movies = await _dataContext.Movies
                .OrderByDescending(m => double.Parse(m.Rating))
                .Take(50)
                .OrderBy(a => Guid.NewGuid())
                .Take(15).ToListAsync();

            return MovieShort.ShortenRange(movies);
        }

        /// <summary>
        /// Recommendation about the user's movies.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Instance of class <see cref="MovieShort"/> with information about the recommended movies.</returns>
        public async Task<List<GenreCount>> GetGenresCountByCategoryForUser(Guid userId, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException();
            }

            var movies =
                await _dataContext.Users_Movies.Where(
                    uM => uM.UserId == userId && uM.Category.Equals(category)).ToListAsync();

            var genreCount = await GetGenresCount(movies);

            var topGenresByCount = new List<GenreCount>();

            foreach (var item in genreCount.OrderByDescending(gC => gC.Value).Take(5))
            {
                topGenresByCount.Add(new GenreCount
                {
                    Name = item.Key,
                    Count = item.Value
                });
            }

            return topGenresByCount;
        }

        public async Task<List<GenreCount>> GetActorsCountByCategoryForUser(Guid userId, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException();
            }

            var movies =
                await _dataContext.Users_Movies.Where(
                    uM => uM.UserId == userId && uM.Category.Equals(category)).ToListAsync();

            var actorCount = await GetActorsCount(movies);

            var topActorsByCount = new List<GenreCount>();

            foreach (var item in actorCount.OrderByDescending(aC => aC.Value).Take(5))
            {
                topActorsByCount.Add(new GenreCount
                {
                    Name = item.Key,
                    Count = item.Value
                });
            }

            return topActorsByCount;
        }

        public async Task<List<GenreCount>> GetProductionsCountByCategoryForUser(Guid userId, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException();
            }

            var movies =
                await _dataContext.Users_Movies.Where(
                    uM => uM.UserId == userId && uM.Category.Equals(category)).ToListAsync();

            var productionCount = await GetProductionsCount(movies);

            var topProductionsByCount = new List<GenreCount>();

            foreach (var item in productionCount.OrderByDescending(pC => pC.Value).Take(5))
            {
                topProductionsByCount.Add(new GenreCount
                {
                    Name = item.Key,
                    Count = item.Value
                });
            }

            return topProductionsByCount;
        }

        public async Task<List<MovieShort>> GetRecommendedMovies(Guid userId)
        {
            var movies =
                await _dataContext.Users_Movies.Where(
                    uM => uM.UserId == userId && uM.Category.Equals("Favorites")).ToListAsync();

            var genreCount = await GetGenresCount(movies);

            var topGenre = genreCount.OrderByDescending(x => x.Value).FirstOrDefault();

            var genreId = await GetGenreIdByName(topGenre.Key);
            var movieIdsFromFavoriteGenre = await _dataContext.Movies_Genres.Where(mG => mG.GenreId == genreId).ToListAsync();

            var moviesFromFavoriteGenre = new List<Movie>();
            foreach (var moviesGenre in movieIdsFromFavoriteGenre)
            {
                moviesFromFavoriteGenre.Add(await GetMovieById(moviesGenre.MovieId));
            }

            var distinctMovieIds = await GetDistinctMovieIdsForUser(userId);
            foreach (var distinctMovieId in distinctMovieIds)
            {
                moviesFromFavoriteGenre.Remove(moviesFromFavoriteGenre.SingleOrDefault(m => m.Id == distinctMovieId));
            }

            var moviesRange = moviesFromFavoriteGenre.Take(15).ToList();

            return MovieShort.ShortenRange(moviesRange);
        }

        /// <summary>
        /// Gets the movies by id without repeating movies.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns all the movies.</returns>
        private async Task<Dictionary<string, int>> GetGenresCount(List<UserMovie> userMovies)
        {
            Dictionary<string, int> genreCount = new Dictionary<string, int>();

            foreach (var userMovie in userMovies)
            {
                var movieGenres = _dataContext.Movies_Genres.Where(mG => mG.MovieId == userMovie.MovieId);
                foreach (var movieGenre in movieGenres)
                {
                    var genre = await _dataContext.Genres.SingleOrDefaultAsync(g => g.Id == movieGenre.GenreId);
                    if (genreCount.ContainsKey(genre.Name))
                    {
                        genreCount[genre.Name]++;
                    }
                    else
                    {
                        genreCount.Add(genre.Name, 1);
                    }
                }
            }

            return genreCount;
        }

        private async Task<Dictionary<string, int>> GetActorsCount(List<UserMovie> userMovies)
        {
            Dictionary<string, int> actorCount = new Dictionary<string, int>();

            foreach (var userMovie in userMovies)
            {
                var movieActors = _dataContext.Movies_Actors.Where(mA => mA.MovieId == userMovie.MovieId);
                foreach (var movieActor in movieActors)
                {
                    var actor = await _dataContext.Actors.SingleOrDefaultAsync(g => g.Id == movieActor.ActorId);
                    if (actorCount.ContainsKey(actor.Name))
                    {
                        actorCount[actor.Name]++;
                    }
                    else
                    {
                        actorCount.Add(actor.Name, 1);
                    }
                }
            }

            return actorCount;
        }

        private async Task<Dictionary<string, int>> GetProductionsCount(List<UserMovie> userMovies)
        {
            Dictionary<string, int> productionsCount = new Dictionary<string, int>();

            foreach (var userMovie in userMovies)
            {
                var movieProductions = _dataContext.Movies_Productions.Where(mP => mP.MovieId == userMovie.MovieId);
                foreach (var movieProduction in movieProductions)
                {
                    var production = await _dataContext.Productions.SingleOrDefaultAsync(g => g.Id == movieProduction.ProductionId);
                    if (productionsCount.ContainsKey(production.Name))
                    {
                        productionsCount[production.Name]++;
                    }
                    else
                    {
                        productionsCount.Add(production.Name, 1);
                    }
                }
            }

            return productionsCount;
        }

        private async Task<List<Guid>> GetDistinctMovieIdsForUser(Guid userId)
        {
            return await _dataContext.Users_Movies.Where(uM => uM.UserId == userId).Select(uM => uM.MovieId).Distinct().ToListAsync();
        }

        /// <summary>
        /// taking genres from the list with genres and adding them into the movies genres. 
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="genres"></param>
        /// <returns>Returns all the movies with the genres.</returns>
        private async Task AddGenresToMovie(Guid movieId, List<string> genres)
        {
            var movieGenres = new List<MovieGenre>();
            foreach (var genre in genres)
            {
                var genreId = await GetGenreIdByName(genre);
                if (genreId == Guid.Empty)
                {
                    var newGenre = new Genre
                    {
                        Id = Guid.NewGuid(),
                        Name = genre
                    };

                    _dataContext.Genres.Add(newGenre);
                    genreId = newGenre.Id;
                }

                movieGenres.Add(new MovieGenre
                {
                    GenreId = genreId,
                    MovieId = movieId,

                });
            }

            await _dataContext.Movies_Genres.AddRangeAsync(movieGenres);
        }

        private async Task AddActorsToMovie(Guid movieId, List<string> actors)
        {
            var movieActors = new List<MovieActor>();
            foreach (var actor in actors)
            {
                var actorId = await GetActorIdByName(actor);
                if (actorId == Guid.Empty)
                {
                    var newActor = new Actor
                    {
                        Id = Guid.NewGuid(),
                        Name = actor
                    };

                    _dataContext.Actors.Add(newActor);
                    actorId = newActor.Id;
                }

                movieActors.Add(new MovieActor
                {
                    ActorId = actorId,
                    MovieId = movieId,

                });
            }

            await _dataContext.Movies_Actors.AddRangeAsync(movieActors);
        }

        private async Task<Guid> GetActorIdByName(string name)
        {
            var actor = await _dataContext.Genres.FirstOrDefaultAsync(g => g.Name == name);
            if (actor == null)
            {
                return Guid.Empty;
            }

            return actor.Id;
        }

        private async Task AddProductionToMovie(Guid movieId, List<string> productions)
        {
            var movieProductions = new List<MovieProduction>();
            foreach (var production in productions)
            {
                var productionId = await GetProductionIdByName(production);
                if (productionId == Guid.Empty)
                {
                    var newProduction = new Production
                    {
                        Id = Guid.NewGuid(),
                        Name = production
                    };

                    _dataContext.Productions.Add(newProduction);
                    productionId = newProduction.Id;
                }

                movieProductions.Add(new MovieProduction
                {
                    ProductionId = productionId,
                    MovieId = movieId,

                });
            }

            await _dataContext.Movies_Productions.AddRangeAsync(movieProductions);
        }

        private async Task<Guid> GetProductionIdByName(string name)
        {
            var production = await _dataContext.Productions.FirstOrDefaultAsync(g => g.Name == name);
            if (production == null)
            {
                return Guid.Empty;
            }

            return production.Id;
        }

        /// <summary>
        /// Downloading the poster for the current movie.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="posterName"></param>
        private void DownloadPoster(string url, string title, out string posterName)
        {
            posterName = $"{title}.jpg";
            using (WebClient pictureDownloader = new WebClient())
            {
                try
                {
                    if (!File.Exists($@"wwwroot\images\{title}.jpg"))
                    {
                        pictureDownloader.DownloadFile(new Uri(url), $@"wwwroot\images\{title}.jpg");
                    }
                }
                catch (Exception e)
                {
                    posterName = "no+poster+available.jpg";
                }
            }
        }

        /// <summary>
        /// Gets the genre of the movie by movie name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the genre id.</returns>
        private async Task<Guid> GetGenreIdByName(string name)
        {
            var genre = await _dataContext.Genres.FirstOrDefaultAsync(g => g.Name == name);
            if (genre == null)
            {
                return Guid.Empty;
            }

            return genre.Id;
        }

        /// <summary>
        /// Gets the genres of a current movie by the movie id.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns>Returns a string with the genres for the current movie.</returns>
        private async Task<string> GetGenresForMovie(Guid movieId)
        {
            var movieGenres = await _dataContext.Movies_Genres.Where(mG => mG.MovieId == movieId).ToListAsync();

            var stringBuilder = new StringBuilder();
            foreach (var movieGenre in movieGenres)
            {
                var genre = await _dataContext.Genres.FirstOrDefaultAsync(g => g.Id == movieGenre.GenreId);
                stringBuilder.Append(genre.Name);
                stringBuilder.Append(", ");
            }

            return stringBuilder.ToString().Trim().TrimEnd(',');
        }

        /// <summary>
        /// Searched the database for a movie with the current movie id.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns>Returns the movie from database.</returns>
        public async Task<Movie> GetMovieById(Guid movieId)
        {
            return await _dataContext.Movies.SingleOrDefaultAsync(m => m.Id == movieId);
        }

        /// <summary>
        /// Search for the movies of a current user by the user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="category"></param>
        /// <param name="moviesOnPage"></param>
        /// <returns>Returns all the user's movies.</returns>
        private IEnumerable<UserMovie> GetMoviesIdByUserId(Guid userId, int pageNumber, string category, int moviesOnPage = NumberOfMoviesOnRow)
        {
            return _dataContext.Users_Movies
                .Where(uM => uM.UserId == userId && uM.Category.Equals(category))
                .OrderByDescending(m => m.MovieId)
                .Skip((pageNumber - 1) * moviesOnPage)
                .Take(moviesOnPage);
        }

        /// <summary>
        /// Gets a string of the movie information and deserializing it into an object.
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns>Instance of class <see cref="MovieLong"/> with the movie information. </returns>
        private async Task<MovieLong> GetMovieByIdFromApi(string imdbId)
        {
            using (var client = new HttpClient())
            {
                var key = "ImdbApi";

                var response = await client.GetStringAsync(new Uri($"http://www.omdbapi.com/?i={imdbId}&apikey={_configuration.GetSection(key).Value}&plot=full"));

                var movie = JsonConvert.DeserializeObject<MovieLong>(response);

                movie.Poster = imdbId;
                return movie;
            }
        }

        /// <summary>
        /// Searches for a movie by the given ids of the user and the movie.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieId"></param>
        /// <returns>Instance of class <see cref="SingleMovieViewModel"/> with information about the category in the users' profile</returns>
        public async Task<SingleMovieViewModel> GetMovieWithGenresById(Guid userId, Guid movieId)
        {
            var movie = _dataContext.Movies.SingleOrDefault(m => m.Id == movieId);
            var result = _mapper.Map<Movie, MovieLong>(movie);

            result.Genre = await GetGenresForMovie(movieId);

            return new SingleMovieViewModel
            {
                MovieLong = result,
                IsInFavorites = IsUserLinkedToMovieByCategory(userId, movieId, "Favorites"),
                IsInWatched = IsUserLinkedToMovieByCategory(userId, movieId, "Watched"),
                IsInWatchlist = IsUserLinkedToMovieByCategory(userId, movieId, "Watchlist")
            };
        }

        /// <summary>
        /// Searches and gets all the movies of an user by given movie title name.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchText"></param>
        /// <param name="category"></param>
        /// <returns>Instance of class <see cref="MoviesByCategoryModel"/> with a list of movies.</returns>
        public async Task<MoviesByCategoryModel> GetMoviesByTitle(Guid userId, string searchText, string category="All")
        {
            var searchTextLower = searchText.ToLower();

            var movies = new List<Movie>();
            var matchingMovies = new List<Movie>();

            if (userId != Guid.Empty && category != "All")
            {
                var userMovies = _dataContext.Users_Movies.Where(uM => uM.UserId == userId && uM.Category == category).ToList();

                foreach (var userMovie in userMovies)
                {
                    movies.Add(await _dataContext.Movies.SingleOrDefaultAsync(m => m.Id == userMovie.MovieId));
                }

                matchingMovies.AddRange(movies.Where(m => m.Title.ToLower().StartsWith(searchTextLower)).ToList());
                matchingMovies.AddRange(movies.Where(m => m.Title.ToLower().Contains(searchTextLower)).ToList());
            }
            else
            {
                matchingMovies.AddRange(_dataContext.Movies.Where(m => m.Title.ToLower().StartsWith(searchTextLower)).ToList());
                matchingMovies.AddRange(_dataContext.Movies.Where(m => m.Title.ToLower().Contains(searchTextLower)).ToList());
            }

            return new MoviesByCategoryModel
            {
                Movies = matchingMovies.Distinct().ToList(),
                PageNumber = 1,
                TotalCount = movies.Count
            };
        }

        /// <summary>
        /// Checks whether an user has the current movie by category by given parameters.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieId"></param>
        /// <param name="category"></param>
        /// <returns>Returns whether user is linked with the movie or not.</returns>
        private bool IsUserLinkedToMovieByCategory(Guid userId, Guid movieId, string category)
        {
            return _dataContext.Users_Movies
                       .Count(uM => uM.MovieId == movieId
                                    && uM.UserId == userId
                                    && uM.Category == category) != 0;
        }
    }
}
