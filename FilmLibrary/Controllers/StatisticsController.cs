using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilmLibrary.Dtos;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmLibrary.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IMoviesRepository _repository;
        private readonly IUserRepository _userRepository;

        public StatisticsController(IMoviesRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public IActionResult Statistics()
        {
            return RedirectToAction("Statistics", "Users");
        }

        public async Task<IActionResult> GetTopGenresCount(string category)
        {
            try
            {
                var result =
                    await _repository.GetGenresCountByCategoryForUser(_userRepository.GetUserId(User), category);
                return Json(result);
            }
            catch (ArgumentNullException e)
            {
                return Json(new List<GenreCount>());
            }
        }

        public async Task<IActionResult> GetTopActorsCount(string category)
        {
            try
            {
                var result = await _repository.GetActorsCountByCategoryForUser(_userRepository.GetUserId(User), category);
                return Json(result);
            }
            catch (ArgumentNullException e)
            {
                return Json(new List<GenreCount>());
            }
        }

        public async Task<IActionResult> GetTopProductionsCount(string category)
        {
            try
            {
                var result = await _repository.GetProductionsCountByCategoryForUser(_userRepository.GetUserId(User), category);
                return Json(result);
            }
            catch (ArgumentNullException e)
            {
                return Json(new List<GenreCount>());
            }
        }
    }
}