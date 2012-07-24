using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Providers;
using SGNMovies.Server.Utilities;
using System.IO;
using System;

namespace SGNMovies.Server.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISGNMovies _sgnMovies;

        public SessionController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        /// <summary>
        /// get all session times
        /// domain/session/list
        /// </summary>
        /// <returns>list session</returns>
        public ActionResult List()
        {
            try
            {
                //var data =  from st in _sgnMovies.SessionTimes 
                //            join pc in _sgnMovies.ProviderCinemas on st.ProviderCinema_Id equals pc.Id
                //            join c in _sgnMovies.Cinemas on pc.Cinema_Id equals c.Id
                //            join m in _sgnMovies.Movies on st.Movie_Id equals m.Id
                //            select new {c, m, st};

                return Json(from s in _sgnMovies.SessionTimes.AsEnumerable()
                            select Helper.GetSessionTimeObject(s), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// updating for a provider id
        /// </summary>
        /// <param name="providerId">field Id in table Providers</param>
        /// <returns>message "Sucess" or "Failed"</returns>
        public ActionResult Update(string providerId)
        {
            try
            {
                IContentProvider cp = ContentProviderFactory.Create(providerId);
                var data = from st in _sgnMovies.SessionTimes select st;
                _sgnMovies.SaveSessionTimes(data, cp.LoadSessionTimes(_sgnMovies));
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
