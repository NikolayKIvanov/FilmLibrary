using System;
using System.Linq;
using System.Security.Claims;

namespace FilmLibrary.Services
{
    public class UserRepository : IUserRepository
    {
        public Guid GetUserId(ClaimsPrincipal claimsPrincipal)       
            => Guid.Parse(claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
    }
}
