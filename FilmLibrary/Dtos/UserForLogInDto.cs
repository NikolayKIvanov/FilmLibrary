using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Dtos
{
    public class UserForLogInDto
    {
        private bool _areCredentialsCorrect = true;

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "E-mail must be between 5 and 20 characters.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool AreCredentialsCorrect
        {
            get
            {
                return _areCredentialsCorrect;
            }
            set
            {
                _areCredentialsCorrect = value;
            }
        }
    }
}
