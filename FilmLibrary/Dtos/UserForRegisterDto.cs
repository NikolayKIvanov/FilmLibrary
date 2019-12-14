using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Dtos
{
    public class UserForRegisterDto
    {
        private bool _isEmailTaken = false;

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "First name must be between 4 and 20 characters.")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Last name must be between 4 and 20 characters.")]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "E-mail must be between 5 and 50 characters.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string RepeatedPassword { get; set; }

        public bool IsEmailTaken {
            get
            {
                return _isEmailTaken;
            }
            set
            {
                _isEmailTaken = value;
            }
        }
    }
}
