using Newtonsoft.Json;

namespace FilmLibrary.Models
{
    public class EmailForm
    {
        [JsonProperty("emailTo")]
        public string EmailTo { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
