using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibrary.Models
{
    public class MovieActor
    {
        [ForeignKey("Movie"), Key]
        public Guid MovieId { get; set; }

        public Movie Movie { get; set; }

        [ForeignKey("Production"), Key]
        public Guid ActorId { get; set; }

        public Actor Actor { get; set; }
    }
}