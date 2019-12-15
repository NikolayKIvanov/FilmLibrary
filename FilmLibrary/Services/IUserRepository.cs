using FilmLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilmLibrary.Services
{
    public interface IUserRepository
    {
        Guid GetUserId(ClaimsPrincipal claimsPrincipal);
    }
}
