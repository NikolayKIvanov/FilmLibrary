using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Dtos
{
    public class UserForUpdateDto
    {
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "First name must be between 4 and 20 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "First name must be between 4 and 20 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "E-mail must be between 5 and 20 characters.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string RepeatedPassword { get; set; }

    }
}
