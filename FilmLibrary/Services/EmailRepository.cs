using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FilmLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FilmLibrary.Services
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IMoviesRepository _movieRepository;
        private readonly HttpContext _httpContext;
        private readonly string _emailEndpoint;

        public EmailRepository(IMoviesRepository movieRepository,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration)
        {
            _movieRepository = movieRepository;
            _httpContext = contextAccessor.HttpContext;
            _emailEndpoint = configuration.GetSection("EmailEndpoint").Value;

        }
        public async Task ShareMovie(Guid id, string to)
        {
            var movie = await _movieRepository.GetMovieById(id);

            var req = _httpContext.Request;

            var url = $"{req.Scheme}://{req.Host}{req.PathBase}/Movies/SingleMovie?imdbId={movie.ImdbId}" ;
     
            var email = new EmailForm()
            {
                Title = $"{_httpContext.User.Identity.Name} recommended the movie {movie.Title}!",
                EmailTo = to,
                Body = $"Check it out <a href={url}>HERE</a> !"
            };

            var content = JsonConvert.SerializeObject(email);

            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                await httpClient.PostAsync(_emailEndpoint, httpContent);
            }
        }
    }
}
