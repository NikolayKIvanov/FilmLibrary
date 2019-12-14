using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FilmLibrary.Models;
using FilmLibrary.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using AutoMapper;
using FilmLibrary.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace FilmLibrary.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/>
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="configuration"></param>
        public UsersController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public IActionResult Statistics()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new UserForRegisterDto());
        }

        /// <summary>
        /// Registers an user with given information from <see cref="UserForRegisterDto"/> model.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Redirecting to the login page if register was successful and if it was not , returning the view of the register to try again.</returns>
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserForRegisterDto user)
        {
            try
            {
                await _repository.Register(user);
                return RedirectToAction("Login", "Users");
            }
            catch (ArgumentException exception)
            {
                return View("Register", new UserForRegisterDto()
                {
                    IsEmailTaken = true
                });
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new UserForLogInDto());
        }

        /// <summary>
        /// Logging in an user with given information from <see cref="UserForLogInDto"/> model.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Redirecting the user to the home page if successful and if it was not, returning the view of the login to try again.</returns>
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(UserForLogInDto user)
        {
            try
            {
                User detailedUser = await _repository.LogIn(user);

                ClaimsIdentity identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, detailedUser.FirstName),
                    new Claim(ClaimTypes.Surname, detailedUser.LastName),
                    new Claim(ClaimTypes.Email, detailedUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, detailedUser.Id.ToString()),
                    new Claim(ClaimTypes.Sid, await _repository.GenerateTokenAsync(user.Email, detailedUser.Id.ToString()))
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException exception)
            {
                return View("Login", new UserForLogInDto()
                {
                    AreCredentialsCorrect = false
                }
                );
            }
        }

        /// <summary>
        /// Logging out an user from the theirs profile.
        /// </summary>
        /// <returns>Redirecting to the login page.</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var userForEdit = await _repository.GetUser(Guid.Parse(userId));
            return View(userForEdit);
        }

        /// <summary>
        /// Editing the profile of the current user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>After finished editing, redirecting the user to the home page.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var userForEdit = await _repository.Edit(Guid.Parse(userId), user);

            return RedirectToAction("Index", "Home");
        }
    }
}

