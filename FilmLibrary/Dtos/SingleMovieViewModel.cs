namespace FilmLibrary.Dtos
{
    public class SingleMovieViewModel
    {
        public MovieLong MovieLong { get; set; }

        public bool IsInWatched { get; set; }

        public bool IsInFavorites { get; set; }

        public bool IsInWatchlist { get; set; }
    }
}