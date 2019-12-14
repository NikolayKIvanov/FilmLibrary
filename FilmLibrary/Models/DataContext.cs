using Microsoft.EntityFrameworkCore;
using System;

namespace FilmLibrary.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserMovie>().HasKey(table => new {
                table.UserId,
                table.MovieId,
                table.Category
            });

            builder.Entity<MovieGenre>().HasKey(table => new {
                table.MovieId,
                table.GenreId
            });

           builder.Entity<MovieActor>().HasKey(table => new
            {
                table.MovieId,
                table.ActorId
            });

            builder.Entity<MovieProduction>().HasKey(table => new
            {
                table.MovieId,
                table.ProductionId
            });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Production> Productions { get; set; }

        public DbSet<UserMovie> Users_Movies { get; set; }

        public DbSet<MovieGenre> Movies_Genres { get; set; }

        public DbSet<MovieActor> Movies_Actors { get; set; }

        public DbSet<MovieProduction> Movies_Productions { get; set; }
    }
}
