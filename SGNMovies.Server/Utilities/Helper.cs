using System.IO;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using SGNMovies.Server.Models;

namespace SGNMovies.Server.Utilities
{
    public static class Helper
    {
        //public static string GetAttributeValue(XElement element, string attributeName)
        //{
        //    string result = string.Empty;
        //    if (element != null)
        //    {
        //        var attribute = element.Attribute(attributeName);
        //        if (attribute != null)
        //        {
        //            result = attribute.Value;
        //        }
        //    }
        //    return result;
        //}

        public static string GetElementValue(XElement element, string elementName)
        {
            string result = string.Empty;

            if (element != null)
            {
                var childElement = element.Element(elementName);
                if (childElement != null)
                {
                    return childElement.Value;
                }
            }
            return result;
        }

        public static bool ConvertFromStringToBool(string value)
        {
            return !value.Equals("0");
        }

        public static HtmlDocument ParsingStream(Stream s)
        {
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            return xdoc;
        }

        public static Movie GetMovieObject(Movie obj)
        {
            return new Movie
                       {
                           Id = obj.Id,
                           MovieWebId = obj.MovieWebId,
                           Title = obj.Title,
                           Director = obj.Director,
                           Duration = obj.Duration,
                           Description = obj.Description,
                           Genre = obj.Genre,
                           Cast = obj.Cast,
                           Language = obj.Language,
                           Producer = obj.Producer,
                           Version = obj.Version,
                           IsNowShowing = obj.IsNowShowing,
                           InfoUrl = obj.InfoUrl,
                           ImageUrl = obj.ImageUrl,
                           TrailerUrl = obj.TrailerUrl,
                       };
        }

        public static Cinema GetCinemaObject(Cinema obj)
        {
            return new Cinema
                       {
                           Id = obj.Id,
                           CinemaWebId = obj.CinemaWebId,
                           Name = obj.Name,
                           Address = obj.Address,
                           Phone = obj.Phone,
                           Latitude = obj.Latitude,
                           Longitude = obj.Longitude,
                           ImageUrl = obj.ImageUrl,
                           MapUrl = obj.MapUrl,
                       };
        }

        public static SessionTime GetSessionTimeObject(SessionTime obj)
        {
            return new SessionTime
            {
                Id = obj.Id,
                //ProviderCinema = obj.ProviderCinema,
                //Movie = obj.Movie,
                Date = obj.Date,
                Time = obj.Time,
            };
        }
    }
}