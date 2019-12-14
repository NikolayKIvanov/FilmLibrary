using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibrary.Models
{
    public class UserMovie
    {
        [ForeignKey("User"), Key]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [ForeignKey("Movie"), Key]
        public Guid MovieId { get; set; }

        public Movie Movie { get; set; }

        [Key]
        public string Category { get; set; }
    }
}