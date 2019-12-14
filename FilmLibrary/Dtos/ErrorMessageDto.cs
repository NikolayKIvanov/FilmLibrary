using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Dtos
{
    public class ErrorMessageDto
    {
        public ErrorMessageDto(string message)
        {
            ErrorMessage = message ?? throw new ArgumentNullException(nameof(message));
        }
        public string ErrorMessage { get; set; }
    }
}
