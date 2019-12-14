using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLibrary.Models
{
    public class MovieProduction
    {
        [ForeignKey("Movie"), Key]
        public Guid MovieId { get; set; }

        public Movie Movie { get; set; }

        [ForeignKey("Production"), Key]
        public Guid ProductionId { get; set; }

        public Production Production { get; set; }
    }
}