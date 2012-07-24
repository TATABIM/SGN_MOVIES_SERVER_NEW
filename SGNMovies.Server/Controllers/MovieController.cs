using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Utilities;
using SGNMovies.Server.Providers;

namespace SGNMovies.Server.Controllers
{
    public class MovieController : Controller
    {

        private readonly ISGNMovies _sgnMovies;

        public MovieController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        /// <summary>
        /// get all movies of all cinemas
        /// domain/movie/list
        /// domain/movie/list?type=nowshowing
        /// domain/movie/list?type=comingsoon
        /// </summary>
        /// <param name="type">type of movie: nowshowing, comingsoon</param>
        /// <returns>list movies</returns>
        public ActionResult List(String type)
        {
            try
            {
                if (type == "nowshowing")
                {
                    // Return now showing movies only
                    return Json(from m in _sgnMovies.Movies.AsEnumerable()
                                where m.IsNowShowing
                                select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
                }

                if (type == "comingsoon")
                {
                    // Return now showing movies only
                    return Json(from m in _sgnMovies.Movies.AsEnumerable()
                                where !m.IsNowShowing
                                select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
                }

                // Return all
                return Json(from m in _sgnMovies.Movies.AsEnumerable()
                            select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failed: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// get the information of particular movie such as: title, direction, cast, ..., trailer,...
        /// domain/movie/getmovie?movieid=3
        /// </summary>
        /// <param name="movieId">field Id in table Movies</param>
        /// <returns>list movies</returns>
        public ActionResult GetMovie(String movieId)
        {
            try
            {
                int Movie_Id = Int32.Parse(movieId);
                return Json(from m in _sgnMovies.Movies.AsEnumerable()
                            where m.Id == Movie_Id
                            select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failed: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// get all movies of a specified cinema
        /// domain/movie/cinema?cinemaid=5
        /// </summary>
        /// <param name="cinemaId">field Id of table Cinemas</param>
        /// <returns>list movies</returns>
        public ActionResult Cinema(String cinemaId)
        {
            try
            {
                int Cinema_Id = Int32.Parse(cinemaId);
                var data = (from pc in _sgnMovies.ProviderCinemas
                            join st in _sgnMovies.SessionTimes on pc.Id equals st.ProviderCinema_Id
                            join m in _sgnMovies.Movies on st.Movie_Id equals m.Id
                            where pc.Cinema_Id == Cinema_Id
                            select m).Distinct();

                return Json(from m in data.AsEnumerable()
                            select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failed: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updating for a provider id
        /// domain/movie/update?providerid=1
        /// </summary>
        /// <param name="id">field Id in table Providers</param>
        /// <returns>message "Sucess" or "Failed"</returns>
        public ActionResult Update(String providerId)
        {
            try
            {
                IContentProvider cp = ContentProviderFactory.Create(providerId);
                var data = from m in _sgnMovies.Movies select m;
                _sgnMovies.SaveMovies(data, cp.LoadAllMovies());
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Failed: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
