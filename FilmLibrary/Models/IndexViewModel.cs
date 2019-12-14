using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Models
{
    public class IndexViewModel
    {
        public List<MovieShort> TopRatedMovies { get; set; }

        public List<MovieShort> RecommendedMovies { get; set; }
    }
}
