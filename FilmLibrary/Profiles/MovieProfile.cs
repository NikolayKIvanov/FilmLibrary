using AutoMapper;
using FilmLibrary.Dtos;
using FilmLibrary.Models;

namespace FilmLibrary.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieLong>();
            CreateMap<MovieLong, Movie>();
        }
    }
}
