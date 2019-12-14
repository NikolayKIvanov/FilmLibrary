using System.Collections.Generic;

namespace FilmLibrary.Models
{
    public class MovieShort
    {
        public string ImdbId{ get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public static MovieShort Shorten(Movie movie)
        {
            return new MovieShort
            {
                ImdbId = movie.ImdbId,
                Title = movie.Title,
                Poster = movie.Poster
            };
        }

        public static List<MovieShort> ShortenRange(List<Movie> movies)
        {
            var shortenMovies = new List<MovieShort>();
            foreach (var movie in movies)
            {
                shortenMovies.Add(Shorten(movie));
            }

            return shortenMovies;
        }
    }
}