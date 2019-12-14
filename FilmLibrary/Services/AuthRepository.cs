using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmLibrary.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FilmLibrary.Dtos;

namespace FilmLibrary.Services
{
    /// <summary>
    /// Respresents functionality related to authenticating users in a server.
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRepository"/> class.
        /// </summary>
        /// <param name="dataContext">corresponding database context.</param>
        /// <param name="configuration">information related to the configuration.</param>
        public AuthRepository(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task<User> GetUser(Guid id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        /// <summary>
        /// Checks if credentials of the user are correct and already exist in the database. 
        /// </summary>
        /// <param name="email">Email address of the user.</param>
        /// <param name="password">Password of the user in plain text.</param>
        /// <returns>Instance of class <see cref="User"/> corresponding to both email address and password.</returns>
        public async Task<User> LogIn(UserForLogInDto userToLogIn)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync<User>(x => string.Equals(x.Email, userToLogIn.Email));

            if (user == null || !BCrypt.Net.BCrypt.Verify(userToLogIn.Password, user.Password))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            return user;
        }

        /// <summary>
        /// Checks if email is not already taken and creates new user in the database.
        /// </summary>
        /// <param name="user">Instance of class <see cref="UserForRegisterDto"/> with information needed for registration.</param>
        /// <returns>Instace of class <see cref="User"/> with information about the new user.</returns>
        public async Task<User> Register(UserForRegisterDto user)
        {
            if (await CheckIfUserExists(user.Email) == true)
            {
                throw new ArgumentException("User with this e-mail already exists.");
            }

            string hashedPassword = PasswordHash(user.Password);
            user.Password = hashedPassword;

            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            };

             _dataContext.Users.Add(newUser);
            await _dataContext.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// Generates identity token asyncronously.
        /// </summary>
        /// <param name="email">Email address of the user waiting for token.</param>
        /// <param name="userId">Id of the user waiting for token.</param>
        /// <returns>String representation of token.</returns>
        public Task<string> GenerateTokenAsync(string email, string userId)
        {
            return Task.Run(() =>
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWTSecret").Value);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                       new Claim(ClaimTypes.Email, email),
                       new Claim(ClaimTypes.NameIdentifier, userId)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(3000),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;
            });
        }

        private async Task<bool> CheckIfUserExists(string email)
        {
            if (await _dataContext.Users.AnyAsync(x => string.Equals(x.Email, email)))
            {
                return true;
            }

            return false;
        }

        private string PasswordHash(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        public async Task<User> Edit(Guid Id, User user)
        {
            var userForEdit = await GetUser(Id);

            userForEdit.FirstName = user.FirstName;
            userForEdit.LastName = user.LastName;
            userForEdit.Email = user.Email;

            _dataContext.Users.Update(userForEdit);
            await _dataContext.SaveChangesAsync();

            return userForEdit;
        }

     

    }
}
