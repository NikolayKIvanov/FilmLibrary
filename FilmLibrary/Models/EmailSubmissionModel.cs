using System;

namespace FilmLibrary.Models
{
    public class EmailSubmissionModel
    {
        public Guid MovieId { get; set; }
        public string EmailTo { get; set; }
    }
}
