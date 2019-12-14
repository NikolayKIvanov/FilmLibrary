using Newtonsoft.Json;
using System;
using System.Reflection.Metadata.Ecma335;
using FilmLibrary.Models;

namespace FilmLibrary.Dtos
{
    public class MovieLong
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Released")]
        public string Released { get; set; }

        [JsonProperty(PropertyName = "Poster")]
        public string Poster { get; set; }

        [JsonProperty(PropertyName = "Plot")]
        public string Plot { get; set; }

        [JsonProperty(PropertyName = "Director")]
        public string Director { get; set; }

        [JsonProperty(PropertyName = "imdbRating")]
        public string Rating { get; set; }

        [JsonProperty(PropertyName = "Genre")]
        public string Genre { get; set; }

        [JsonProperty(PropertyName = "Actors")]
        public string Actors { get; set; }

        [JsonProperty(PropertyName = "Production")]
        public string Production { get; set; }

        [JsonProperty(PropertyName = "imdbID")]
        public string ImdbId { get; set; }

        public static MovieLong MovieWithGenres(Movie movie, string genre)
        {
            return new MovieLong
            {
                Id = movie.Id,
                Director = movie.Director,
                Plot = movie.Plot,
                Poster = movie.Poster,
                Rating = movie.Rating,
                Released = movie.Released,
                Title = movie.Title,
                Genre = genre,
                Production = movie.Production,
                Actors = movie.Actors,
                ImdbId = movie.ImdbId
            };
        }
    }
}
