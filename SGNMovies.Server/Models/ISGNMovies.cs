using System.Collections.Generic;
using System.Data.Objects;

namespace SGNMovies.Server.Models
{
    public interface ISGNMovies
    {
        ObjectSet<Provider> Providers { get; }
        ObjectSet<Cinema> Cinemas { get; }
        ObjectSet<Movie> Movies { get; }
        ObjectSet<SessionTime> SessionTimes { get; }
        ObjectSet<ProviderCinema> ProviderCinemas { get; }

        int SaveChanges();
        void SaveSessionTimes(IEnumerable<SessionTime> oldSessions, IEnumerable<SessionTime> sessions);
        void SaveMovies(IEnumerable<Movie> oldMovies, IEnumerable<Movie> movies);
    }
}
