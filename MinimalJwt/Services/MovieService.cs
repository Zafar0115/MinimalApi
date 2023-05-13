using MinimalJwt.Models;
using MinimalJwt.Repositories;

namespace MinimalJwt.Services
{
    public class MovieService : IMovieService
    {
        public Movie Create(Movie movie)
        {
            movie.Id = MovieRepository.Movies.Count + 1;

            MovieRepository.Movies.Add(movie);

            return movie;
        }

        public bool Delete(int id)
        {
            Movie? movie=MovieRepository.Movies.FirstOrDefault(m=> m.Id == id);

            if(movie is null) return false;

            MovieRepository.Movies.Remove(movie);
            return true;
        }

        public Movie Get(int id)
        {
            Movie? movie=MovieRepository.Movies.FirstOrDefault(m=>m.Id== id);

            if(movie is null ) return null;

            return movie;

        }

        public List<Movie> GetAll()
        {
            return MovieRepository.Movies;
        }

        public Movie Update(Movie movie)
        {
            Movie? oldMovie= MovieRepository.Movies.FirstOrDefault(m=>m.Id== movie.Id);

            if (oldMovie is null) return null;

            oldMovie.Title = movie.Title;
            oldMovie.Description = movie.Description;
            oldMovie.Rating= movie.Rating;

            return movie;
        }
    }
}
