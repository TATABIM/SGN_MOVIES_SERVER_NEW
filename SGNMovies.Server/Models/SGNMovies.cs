using System.Collections.Generic;

namespace SGNMovies.Server.Models
{
    public partial class SGNMovieContainer : ISGNMovies
    {
        public void SaveSessionTimes(IEnumerable<SessionTime> oldSessionTimes, IEnumerable<SessionTime> sessionTimes)
        {
            foreach (var session in oldSessionTimes)
                SessionTimes.DeleteObject(session);
             SaveChanges();
            foreach (var session in sessionTimes)
                SessionTimes.AddObject(session);

            SaveChanges();
        }

        public void SaveMovies(IEnumerable<Movie> oldMovies, IEnumerable<Movie> movies)
        {
            foreach (var movie in oldMovies)
                Movies.DeleteObject(movie);
            SaveChanges();
            foreach (var movie in movies)
                Movies.AddObject(movie);

            SaveChanges();
        }
    }
}