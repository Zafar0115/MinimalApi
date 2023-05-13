using MinimalJwt.Models;

namespace MinimalJwt.Services
{
    public interface IMovieService
    {
        public Movie Create(Movie Movie);
        public Movie Update(Movie Movie);
        public bool Delete(int id);
        public Movie Get(int id);
        public List<Movie> GetAll();
    }
}
