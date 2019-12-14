using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
