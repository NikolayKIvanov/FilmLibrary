using FilmLibrary.Dtos;
using FilmLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLibrary.Services
{
    public interface IAuthRepository
    {
        Task<User> Register(UserForRegisterDto user);

        Task<User> LogIn(UserForLogInDto user);

        Task<User> GetUser(Guid id);

        Task<string> GenerateTokenAsync(string email, string userId);

        Task<User> Edit(Guid Id, User user);

     
       
    }
}
