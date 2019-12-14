using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibrary.Models
{
    public class MovieGenre
    {
        [ForeignKey("Movie"), Key]
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        [ForeignKey("Genre"), Key]
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}