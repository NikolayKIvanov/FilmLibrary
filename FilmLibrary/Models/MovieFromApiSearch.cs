using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FilmLibrary.Models
{
    public class MovieFromApiSearch
    {
        [JsonProperty(PropertyName = "imdbId")]
        public string ImdbId { get; set; }

        [JsonProperty(PropertyName = "Poster")]
        public string Poster { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Year")]
        public string Year { get; set; }
    }
}
