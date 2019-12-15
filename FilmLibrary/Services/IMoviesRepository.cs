using FilmLibrary.Dtos;
using FilmLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMoviesRepository
{
    Task<SingleMovieViewModel> GetMovieByImdbId(string imdbId);

    Task<Movie> GetMovieById(Guid id);

    Task<SingleMovieViewModel> GetMovieByImdbIdForUser(Guid userId, string imdbId);

    Task<MoviesByCategoryModel> GetMoviesByDirector(Guid userId, string directorName, string category);

    Task<MoviesByCategoryModel> GetMoviesByGenre(Guid userid, string genreName, string category);

    Task AddMovieToUser(Guid userId, Guid movieId, string category);

    Task RemoveMovieFromUser(Guid userId, Guid movieId, string category);

    Task<MoviesByCategoryModel> GetMoviesByCategory(Guid userId, string category, int pageNumberWatched = 1, int moviesOnPage = 8);

    Task<MultipleMoviesModelView> GetTenMoviesByTitle(string title, int page = 1);

    Task<List<MovieShort>> GetTopRatedMovies();

    Task<List<MovieShort>> GetRecommendedMovies(Guid userId);

    Task<SingleMovieViewModel> GetMovieWithGenresById(Guid userId, Guid movieId);

    Task<MoviesByCategoryModel> GetMoviesByTitle(Guid userId, string searchText, string category);

    Task<List<GenreCount>> GetGenresCountByCategoryForUser(Guid userId, string category);

    Task<List<GenreCount>> GetActorsCountByCategoryForUser(Guid userId, string category);

    Task<List<GenreCount>> GetProductionsCountByCategoryForUser(Guid userId, string category);
}
