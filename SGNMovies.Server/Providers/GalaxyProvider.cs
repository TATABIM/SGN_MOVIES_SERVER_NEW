using System.Collections.Generic;
using System.IO;
using SGNMovies.Server.Models;
using System.Text;
using System.Linq;
using HtmlAgilityPack;
using System.Web;
using System.Net;
using System;

namespace SGNMovies.Server.Providers
{
    public class GalaxyProvider : IContentProvider
    {
        public const string GALAXY_BASE_URL = "http://www.galaxycine.vn";
        public const string GALAXY_MOVIE_NOW_SHOWING_URL = "/vi/movie?type=now-showing";
        public const string GALAXY_MOVIE_COMING_SOON_URL = "/vi/movie?type=coming-soon";
        public const string GALAXY_SEARCH_LINK = "http://www.galaxycine.vn/actionServlet";
        public const string GALAXY_POST_MOVIE_CONTENT = "type=reloadmovie&cinema={0}&token={1}";
        public const string GALAXY_POST_SESSION_CONTENT = "type=reloadshowtime&cinema={0}&movie={1}&token={2}";
        public string TOKEN_KEY { get; set; }

        #region XPath
        //movie's content in detail page
        public const string CONTENT_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]";
        //movie's image url in detail page
        public const string IMAGEURL_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div/div/div/a/img";
        //movie's title in detail page
        public const string TITLE_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/div/span";
        //movie's genre in detail page
        public const string GENRE_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[4]";
        //movie's description in detail page
        public const string DESCRIPTION_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[4]";
        //movie's web id in detail page
        public const string WEBID_MOVIE_XPATH = "//*[@id='movieid']";
        //movie's version (2D, 3D etc..) in detail page
        public const string VERSION_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[12]";
        //movie's director in detail page
        public const string DIRECTOR_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[8]";
        //movie's cast in detail page
        public const string CAST_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[6]";
        //movie's duration in detail page
        public const string DURATION_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[10]";
        //movie's language in detail page
        public const string LANGUAGE_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[16]";
        //movie's producer in detail page
        public const string PRODUCER_MOVIE_XPATH = "/html/body/div/div[3]/div[3]/div[2]/div/div/div[2]/div[3]/span[8]";
        //movie's trailer url in detail page
        public const string TRAILERURL_MOVIE_XPATH = "//*[@class='popupTrailerMovie']/div/object/param";
        //movie's info url in main page: current node's xpath + info path
        public const string INFOURL_MOVIE_XPATH = "{0}//a";
        //movie's content in main page: list movies
        public const string LIST_MOVIES_XPATH = "//div[@class='mov_t_box']";
        //movieWebId in page showtime: current node's xpath + webid path
        public const string WEBID_MOVIE_SHOWTIME_XPATH = "{0}//span[@class='cbDisplayMovie']";
        //session dates collection in page showtime
        public const string DATECOLLECTION_MOVIE_SHOWTIME_XPATH = "//div[@class='showtime_mov_date']";
        //session times collection in page showtime
        public const string TIMECOLLECTION_MOVIE_SHOWTIME_XPATH = "//div[@class='showtime_mov_time']";
        //session time in page showtime in normal day {2 3 4 5 6}: current node's xpath + path
        public const string TIME_MOVIE_SHOWTIME_XPATH = "{0}//a[@class='showtime_mov_hour btnShowtime']";
        //session time in page showtime in weekend: current node's xpath + path
        public const string TIMEREADONLY_MOVIE_SHOWTIME_XPATH = "{0}//div[@class='showtime_mov_hour readonly']";
        #endregion

        public GalaxyProvider()
        {
            WebResponse response = GetJSessionKey(GALAXY_BASE_URL);
            TOKEN_KEY = response.Headers.GetValues("Set-Cookie").FirstOrDefault().Split(';').FirstOrDefault();
            TOKEN_KEY = TOKEN_KEY.Replace("JSESSIONID=", "");
        }

        public WebResponse GetJSessionKey(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            return response;
        }

        //public int Id
        //{
        //    get { return 1; }
        //}

        public string Name
        {
            get { return "Galaxy"; }
        }

        public string BaseUrl
        {
            get { return GALAXY_BASE_URL; }
        }

        public IEnumerable<Movie> LoadAllMovies()
        {
            IEnumerable<Movie> now = GetMoviesFromStream(WebConnection.GetUrl(GALAXY_BASE_URL + GALAXY_MOVIE_NOW_SHOWING_URL, null), true);
            IEnumerable<Movie> coming = GetMoviesFromStream(WebConnection.GetUrl(GALAXY_BASE_URL + GALAXY_MOVIE_COMING_SOON_URL, null), false);

            return coming.Concat(now);
        }

        //public IEnumerable<Movie> LoadNowShowingMovies()
        //{
        //    return GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=1", null), true);
        //} 

        //public IEnumerable<Movie> LoadComingSoonMovies()
        //{
        //    return GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=2", null), false);
        //}

        public IEnumerable<SessionTime> LoadSessionTimes(ISGNMovies sgnMovies)
        {
            List<SessionTime> sessions = new List<SessionTime>();
            var cinema_Ids = from p in sgnMovies.Providers
                             join pc in sgnMovies.ProviderCinemas on p.Id equals pc.Provider_Id
                             where (p.Name == "Galaxy")
                             select pc;
            var cinemas = from pc in cinema_Ids
                          join c in sgnMovies.Cinemas on pc.Cinema_Id equals c.Id
                          select new { c, pc };

            foreach (var cinema in cinemas)
            {
                string content = string.Format(GALAXY_POST_MOVIE_CONTENT, cinema.c.CinemaWebId, TOKEN_KEY);
                Stream s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                string[] WebIds = LoadMovieWebIdCollection(s).Split('~');
                IEnumerable<Movie> movies = LoadMoviesFromWebIds(sgnMovies.Movies, WebIds);
                foreach (Movie movie in movies)
                {
                    content = string.Format(GALAXY_POST_SESSION_CONTENT, cinema.c.CinemaWebId, movie.MovieWebId, TOKEN_KEY);
                    s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                    IEnumerable<ShowingDateModel> dates = LoadDateTimeFromCinemaMovie(s);
                    foreach (ShowingDateModel dateTime in dates)
                    {
                        sessions.AddRange(dateTime.ShowingTimes.Select(time => new SessionTime
                                       {
                                           ProviderCinema_Id = cinema.pc.Id,
                                           Movie_Id = movie.Id,
                                           Date = dateTime.ShowingDate,
                                           Time = time.DateTime
                                       }));
                    }
                }
            }
            return sessions;
        }

        private IEnumerable<Movie> GetMoviesFromStream(Stream stream, bool isNowShowing)
        {
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection items = xdoc.DocumentNode.SelectNodes(LIST_MOVIES_XPATH);
            return items.Select(item => LoadMovieFromNode(item, isNowShowing)).ToList();
        }

        private Movie LoadMovieFromNode(HtmlNode parent_node, bool isNowShowing)
        {
            try
            {
                string InfoUrl = parent_node.SelectSingleNode(string.Format(INFOURL_MOVIE_XPATH, parent_node.XPath)).GetAttributeValue("href", string.Empty);
                Stream stream = WebConnection.GetUrl(GALAXY_BASE_URL + InfoUrl, null);
                StreamReader reader = new StreamReader(stream, Encoding.Unicode);
                HtmlDocument xdoc = new HtmlDocument();
                xdoc.Load(reader.BaseStream, true);
                HtmlNode node = xdoc.DocumentNode;
                Movie movie = new Movie();
                movie.InfoUrl = InfoUrl;
                movie.ImageUrl = node.SelectSingleNode(IMAGEURL_MOVIE_XPATH).GetAttributeValue("loading", string.Empty);
                movie.MovieWebId = node.SelectSingleNode(WEBID_MOVIE_XPATH).GetAttributeValue("value", string.Empty);
                movie.Title = node.SelectSingleNode(TITLE_MOVIE_XPATH).InnerHtml;
                movie.Director = node.SelectSingleNode(DIRECTOR_MOVIE_XPATH).InnerHtml;
                movie.Duration = node.SelectSingleNode(DURATION_MOVIE_XPATH).InnerHtml;
                movie.Description = node.SelectSingleNode(DESCRIPTION_MOVIE_XPATH).InnerText;
                movie.Genre = node.SelectSingleNode(GENRE_MOVIE_XPATH).InnerHtml;
                movie.Cast = node.SelectSingleNode(CAST_MOVIE_XPATH).InnerHtml;
                movie.Language = node.SelectSingleNode(LANGUAGE_MOVIE_XPATH).InnerHtml;
                movie.Producer = node.SelectSingleNode(PRODUCER_MOVIE_XPATH).InnerHtml;
                movie.Version = node.SelectSingleNode(VERSION_MOVIE_XPATH).InnerHtml;
                movie.IsNowShowing = isNowShowing;
                HtmlNode trailerNode = node.SelectSingleNode(TRAILERURL_MOVIE_XPATH);
                movie.TrailerUrl = trailerNode != null ? trailerNode.GetAttributeValue("value", string.Empty) : string.Empty;

                return movie;
            }
            catch (Exception ex)
            {
                return new Movie();
            }
        }

        #region Load SessionTime New Code
        public string LoadMovieWebIdCollection(Stream s)
        {
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection items = xdoc.DocumentNode.ChildNodes;
            return items.Aggregate("", (current, item) => current + (LoadMovieWebId(item) + "~"));
        }

        private string LoadMovieWebId(HtmlNode node)
        {
            if (node.Name.Contains("text"))
                return string.Empty;
            HtmlNode span = node.SelectSingleNode(string.Format(WEBID_MOVIE_SHOWTIME_XPATH, node.XPath));
            if (span == null)
                return string.Empty;
            return span.GetAttributeValue("value", string.Empty);
        }

        public IEnumerable<Movie> LoadMoviesFromWebIds(IEnumerable<Movie> movies, IEnumerable<string> vars)
        {
            List<Movie> results = new List<Movie>();
            foreach (var str in vars)
                if (!string.IsNullOrEmpty(str) && movies != null)
                    results.AddRange(movies.Where(movie => movie.MovieWebId == str));

            return results;
        }
        #endregion Load SessionTime New Code

        public IEnumerable<ShowingDateModel> LoadDateTimeFromCinemaMovie(Stream s)
        {
            List<ShowingDateModel> result = new List<ShowingDateModel>();
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection dates = xdoc.DocumentNode.SelectNodes(DATECOLLECTION_MOVIE_SHOWTIME_XPATH);
            HtmlNodeCollection times = xdoc.DocumentNode.SelectNodes(TIMECOLLECTION_MOVIE_SHOWTIME_XPATH);

            if (dates != null && times != null)
            {
                foreach (HtmlNode date in dates)
                {
                    int index = dates.GetNodeIndex(date);
                    HtmlNode time = times[index];
                    HtmlNodeCollection sessions =
                        time.SelectNodes(string.Format(TIME_MOVIE_SHOWTIME_XPATH, time.XPath)) ??
                        time.SelectNodes(string.Format(TIMEREADONLY_MOVIE_SHOWTIME_XPATH, time.XPath));
                    ShowingDateModel dateTime = new ShowingDateModel { ShowingDate = HttpUtility.HtmlDecode(date.InnerText) };

                    foreach (HtmlNode session in sessions)
                    {
                        ShowingTimeModel sessionTime = new ShowingTimeModel
                                                           {
                                                               Id = "",
                                                               DateTime = HttpUtility.HtmlDecode(session.InnerText)
                                                           };
                        dateTime.ShowingTimes.Add(sessionTime);
                    }
                    result.Add(dateTime);
                }
            }
            return result;
        }
    }
}