using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Utilities;
using System;

namespace SGNMovies.Server.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ISGNMovies _sgnMovies;

        public CinemaController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        /// <summary>
        /// get all cinemas
        /// domain/cinema/list
        /// </summary>
        /// <returns>list cinemas</returns>
        public ActionResult List()
        {
            try
            {
                return Json(from c in _sgnMovies.Cinemas.AsEnumerable()
                            select Helper.GetCinemaObject(c), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// get all cinemas that showing or be going to show a specified movie
        /// GET: domain/cinema/movie?movieid=3
        /// </summary>
        /// <param name="movieId">field Id in table Movies</param>
        /// <returns>list cinemas</returns>
        public ActionResult Movie(String movieId)
        {
            try
            {
                int Movie_Id = Int32.Parse(movieId);
                var data = (from st in _sgnMovies.SessionTimes
                            join pc in _sgnMovies.ProviderCinemas on st.ProviderCinema_Id equals pc.Id
                            join c in _sgnMovies.Cinemas on pc.Cinema_Id equals c.Id
                            where st.Movie_Id == Movie_Id
                            select c).Distinct();

                return Json(from m in data.AsEnumerable()
                            select Helper.GetCinemaObject(m), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
