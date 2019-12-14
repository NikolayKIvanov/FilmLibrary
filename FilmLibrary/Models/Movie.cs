using System;

namespace FilmLibrary.Models
{
    public class Movie
    {        
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Released { get; set; }

        public string Poster { get; set; }

        public string Plot { get; set; }

        public string Director { get; set; }

        public string Rating { get; set; }

        public string ImdbId { get; set; }

        public string Actors { get; set; }

        public string Production { get; set; }
    }
}