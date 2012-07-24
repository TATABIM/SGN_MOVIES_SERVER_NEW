using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using HtmlAgilityPack;
using SGNMovies.Server.Models;
using SGNMovies.Server.Utilities;

namespace SGNMovies.Server.Providers
{
    public class MegaStarProvider : IContentProvider
    {
        public string CINEMA_TO_FILM { get; set; }
        public string CINEME_FILM_TO_SESSIONTIME { get; set; }

        public MegaStarProvider()
        {
            CINEME_FILM_TO_SESSIONTIME = BaseUrl + "/megastarXMLData.aspx?RequestType=GetSessionTimes&&CinemaID={0}"
                                                 + "&&MovieName={1}&&Time=TodayAndTomorrow&&visLang=1";
            CINEMA_TO_FILM = BaseUrl + "/megastarXMLData.aspx?RequestType=GetMovieListByCinemaID&&CinemaID={0}&&visLang=1";
        }

        public int Id
        {
            get { return 0; }
        }

        public string Name
        {
            get { return "MegaStar"; }
        }

        public string BaseUrl
        {
            get { return "http://www.megastar.vn"; }
        }

        public IEnumerable<Movie> LoadAllMovies()
        {
            IList<Movie> now = LoadNowShowingMovies().ToList();
            IList<Movie> coming = LoadComingSoonMovies().ToList();

            foreach (var movie in coming)
                now.Add(movie);

            return now;
        }

        public IEnumerable<Movie> LoadNowShowingMovies()
        {
            List<Movie> results = LoadMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/vn/nowshowing/", null), true).ToList();
            results.ForEach(movie =>
                                {
                                    if (!movie.InfoUrl.Contains(BaseUrl))
                                    {
                                        Stream s = WebConnection.GetUrl(BaseUrl + movie.InfoUrl, null);
                                        HtmlDocument xdoc = Helper.ParsingStream(s);
                                        movie.Cast = HttpUtility.HtmlDecode(xdoc.DocumentNode.SelectNodes("//span[@id='ContentPlaceHolder1_lblCast']").FirstOrDefault().InnerText);
                                        movie.Language = HttpUtility.HtmlDecode(xdoc.DocumentNode.SelectNodes("//span[@id='ContentPlaceHolder1_lblLanguage']").FirstOrDefault().InnerText);
                                        movie.TrailerUrl = GetTrailerUrl(xdoc.DocumentNode);
                                    }
                                });
            return results;
        }

        public IEnumerable<Movie> LoadComingSoonMovies()
        {
            List<Movie> results = LoadMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/vn/comingsoon/", null), false).ToList();
            results.ForEach(movie =>
                                {
                                    if (!movie.InfoUrl.Contains(BaseUrl))
                                    {
                                        Stream s = WebConnection.GetUrl(BaseUrl + movie.InfoUrl, null);
                                        HtmlDocument xdoc = Helper.ParsingStream(s);
                                        movie.Cast = HttpUtility.HtmlDecode(xdoc.DocumentNode.SelectNodes("//span[@id='ContentPlaceHolder1_lblCast']").FirstOrDefault().InnerText);
                                        movie.Language = HttpUtility.HtmlDecode(xdoc.DocumentNode.SelectNodes("//span[@id='ContentPlaceHolder1_lblLanguage']").FirstOrDefault().InnerText);
                                        movie.TrailerUrl = GetTrailerUrl(xdoc.DocumentNode);
                                        HtmlNode var = xdoc.DocumentNode.SelectNodes("//input[@id='ContentPlaceHolder1_hid_movie']").FirstOrDefault();
                                        movie.MovieWebId = var.GetAttributeValue("value", string.Empty);
                                    }
                                });
            return results;
        }

        public IEnumerable<SessionTime> LoadSessionTimes(IEnumerable<Cinema> cinemas)
        {   
            foreach (var cinema in cinemas)
            {
                Stream s = WebConnection.GetUrl(string.Format(CINEMA_TO_FILM, cinema.CinemaWebId), null);
                var movies = GetMoviesFromCinema(s);
                foreach (var movie in movies)
                {
                    s = WebConnection.GetUrl(string.Format(CINEME_FILM_TO_SESSIONTIME, cinema.CinemaWebId, movie.MovieWebId), null);
                    var dates = LoadDateTimeFromCinemaMovie(s);
                    if (dates != null)
                    {
                        foreach (ShowingDateModel date in dates)
                        {
                            foreach (ShowingTimeModel time in date.ShowingTimes)
                            {
                                yield return new SessionTime
                                                 {
                                                     //Cinema = cinema,
                                                     //Movie = movie,
                                                     Date = date.ShowingDate,
                                                     Time = time.DateTime
                                                 };
                            }
                        }
                    }
                }
            }   
        }

        public IEnumerable<SessionTime> LoadSessionTimes(ISGNMovies sgnMovies)
        {
            IEnumerable<Cinema> cinemas = sgnMovies.Cinemas.Where(c => Int32.Parse(c.CinemaWebId) > 1000);
            foreach (var cinema in cinemas)
            {
                Stream s = WebConnection.GetUrl(string.Format(CINEMA_TO_FILM, cinema.CinemaWebId), null);
                IEnumerable<Movie> tmp = GetMoviesFromCinema(s);
                IEnumerable<Movie> movies = LoadMoviesFromVars(sgnMovies.Movies, tmp);
                foreach (var movie in movies)
                {
                    s = WebConnection.GetUrl(string.Format(CINEME_FILM_TO_SESSIONTIME, cinema.CinemaWebId, movie.MovieWebId), null);
                    IEnumerable<ShowingDateModel> dates = LoadDateTimeFromCinemaMovie(s);
                    if (dates != null)
                    {
                        foreach (ShowingDateModel date in dates)
                        {
                            foreach (ShowingTimeModel time in date.ShowingTimes)
                            {
                                yield return new SessionTime
                                {
                                    //Cinema = cinema,
                                    //Movie = movie,
                                    Date = date.ShowingDate,
                                    Time = time.DateTime
                                };
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Movie> LoadMoviesFromStream(Stream s, bool isNowShowing)
        {
            HtmlDocument xdoc = Helper.ParsingStream(s);
            HtmlNodeCollection items = xdoc.DocumentNode.SelectNodes("//div[@class='landingbody_item']");
            return items.Select(item => LoadMovieFromNode(item, isNowShowing)).ToList();
        }

        private Movie LoadMovieFromNode(HtmlNode node, bool isNowShowing)
        {
            string[] info = GetMovieInfo(node);
            return new Movie
            {
                InfoUrl = GetMovieInfoUrl(node),
                ImageUrl = GetMovieImageUrl(node),
                Title = info[0],
                Director = HttpUtility.HtmlDecode(info[1]).Replace("Đạo diễn:", "").Replace("Director:", ""),
                Duration = HttpUtility.HtmlDecode(info[2]).Replace("Thời lượng:", "").Replace("Running Time:", ""),
                Genre = HttpUtility.HtmlDecode(info[3]).Replace("Thể loại:", "").Replace("Genre:", ""),
                Description = info[4],
                MovieWebId = GetMovieVarName(node),
                IsNowShowing = isNowShowing,
                Version = "",//need to arrange
            };
        }

        #region Get Property Methods

        private string[] GetMovieInfo(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[1]");
            string[] info = child.InnerText.Split('\n');
            info = Array.FindAll(info, val => val != "" && val != "\t\t").ToArray();
            return info;
        }

        private string GetMovieImageUrl(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/a[1]/img[1]");
            return child.GetAttributeValue("src", string.Empty);
        }

        private string GetMovieInfoUrl(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/a[1]");
            return child.GetAttributeValue("href", string.Empty);
        }

        private string GetMovieVarName(HtmlNode node)
        {
            try
            {
                HtmlNode child = node.SelectNodes(node.XPath + "//select[@*]").FirstOrDefault();
                if (child == null)
                    return "";
                string result = child.GetAttributeValue("onchange", string.Empty);
                result = result.Split('"')[1];
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetTrailerUrl(HtmlNode xdoc)
        {
            HtmlNodeCollection items = xdoc.SelectNodes("//script[@*]");
            foreach (var node in items)
            {
                if (!string.IsNullOrEmpty(node.InnerText))
                {
                    if (node.InnerText.Contains("ytlink"))
                    {
                        string[] strs = node.InnerText.Split('"');
                        foreach (var str in strs)
                        {
                            if (str.Contains("ytlink"))
                                return HttpUtility.UrlDecode(str.Replace("ytlink=", ""));
                        }
                    }
                }
            }
            return string.Empty;
        }

        #endregion Get Property Methods

        #region Load SessionTime Old Code
        public IEnumerable<Movie> GetMoviesFromCinema(Stream s)
        {
            XDocument xDoc = XDocument.Load(s);
            List<Movie> results = (from movie in xDoc.Descendants("movie")
                                   select new Movie
                                   {
                                       Title = Helper.GetElementValue(movie, "MovieName"),
                                       MovieWebId = Helper.GetElementValue(movie, "MovieNameVar"),
                                       TrailerUrl = Helper.GetElementValue(movie, "Trailer"),
                                       InfoUrl = Helper.GetElementValue(movie, "MovieInfoUrl"),
                                       Description = Helper.GetElementValue(movie, "ShortDesc"),
                                       Version = Helper.GetElementValue(movie, "Is3d"),
                                       IsNowShowing = Helper.ConvertFromStringToBool(Helper.GetElementValue(movie, "IsNew")),
                                   }).ToList();
            return results;
        }
        #endregion Load SessionTime Old Code

        #region Load SessionTime New Code
        public IEnumerable<Movie> LoadMoviesFromVars(IEnumerable<Movie> sgns, IEnumerable<Movie> tmps)
        {
            List<Movie> results = new List<Movie>();
            foreach (var tmp in tmps)
                if (tmp != null)
                    results.AddRange(sgns.Where(sgn => sgn.MovieWebId == tmp.MovieWebId));

            return results;
        }
        #endregion Load SessionTime New Code

        public IEnumerable<ShowingDateModel> LoadDateTimeFromCinemaMovie(Stream s)
        {
            List<ShowingDateModel> result = new List<ShowingDateModel>();
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection dates = xdoc.DocumentNode.SelectNodes("//date[@*]");
            if (dates == null)
                return null;
            foreach (HtmlNode date in dates)
            {
                ShowingDateModel dateTime = new ShowingDateModel { ShowingDate = date.GetAttributeValue("name", string.Empty) };
                HtmlNodeCollection times = date.ChildNodes;
                foreach (HtmlNode time in times)
                {
                    ShowingTimeModel sessionTime = new ShowingTimeModel
                    {
                        Id = time.SelectSingleNode(time.XPath + "/id[1]").InnerText,
                        DateTime = time.SelectSingleNode(time.XPath + "/date[1]").InnerText
                    };
                    dateTime.ShowingTimes.Add(sessionTime);
                }
                result.Add(dateTime);
            }
            return result;
        }
    }
}