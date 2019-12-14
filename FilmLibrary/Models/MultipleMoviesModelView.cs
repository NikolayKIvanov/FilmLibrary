using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FilmLibrary.Models
{
    public class MultipleMoviesModelView
    {
        [JsonProperty(PropertyName = "Search")]
        public List<MovieFromApiSearch> MovieShorts { get; set; }

        [JsonProperty(PropertyName = "totalResults")]
        public string TotalResults { get; set; }

        [JsonProperty(PropertyName = "Response")]
        public string Response { get; set; }

        [JsonProperty(PropertyName = "Error")]
        public string ErrorMessage { get; set; }

        [JsonIgnore]
        public string SearchText { get; set; }

        [JsonIgnore]
        public int PageNumber { get; set; }

    }
}
