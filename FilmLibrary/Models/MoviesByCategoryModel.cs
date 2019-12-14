using System.Collections.Generic;

namespace FilmLibrary.Models
{
    public class MoviesByCategoryModel
    {
        public List<Movie> Movies { get; set; }

        public int PageNumber { get; set; }

        public int TotalCount { get; set; }
    }
}
