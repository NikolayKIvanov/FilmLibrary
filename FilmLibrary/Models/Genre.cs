using System;
using System.Collections.Generic;

namespace FilmLibrary.Models
{
    public class Genre
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<MovieGenre> Movies { get; set; }
    }
}