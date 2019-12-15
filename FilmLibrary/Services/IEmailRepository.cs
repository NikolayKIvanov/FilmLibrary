using FilmLibrary.Models;
using System;
using System.Threading.Tasks;

namespace FilmLibrary.Services
{
    public interface IEmailRepository
    {
        Task ShareMovie(Guid movieId, string to);
    }
}
