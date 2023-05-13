using MinimalJwt.Models;

namespace MinimalJwt.Repositories
{
    public class MovieRepository
    {
        public static List<Movie> Movies = new()
        {
            new Movie{Id=1,Title="Eternal", Description="aaa", Rating=4.2},
            new Movie{Id=2,Title="Dune", Description="bb", Rating=4},
            new Movie{Id=3,Title="No time", Description="cc", Rating=5.2},
            new Movie{Id=4,Title="Read Notice", Description="ff", Rating=6.2},
            new Movie{Id=5,Title="the harder they fall", Description="hh", Rating=4.6},
        };
    }
}
