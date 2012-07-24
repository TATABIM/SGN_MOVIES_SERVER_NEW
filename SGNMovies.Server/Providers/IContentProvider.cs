using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using SGNMovies.Server.Models;

namespace SGNMovies.Server.Providers
{
    public interface IContentProvider
    {
        //int Id { get; }
        string Name { get; }
        string BaseUrl { get; }

        IEnumerable<Movie> LoadAllMovies();
        ////IEnumerable<Movie> LoadNowShowingMovies();
        ////IEnumerable<Movie> LoadComingSoonMovies();
        //IEnumerable<SessionTime> LoadSessionTimes(IEnumerable<Cinema> cinemas);
        IEnumerable<SessionTime> LoadSessionTimes(ISGNMovies sgnMovies);
    }

    public static class WebConnection
    {
        private static readonly Random _rand = new Random();

        public static Stream PostUrl(string url, CookieContainer cookies, string content)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookies != null)
                request.CookieContainer = cookies;
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0) Gecko/20100101 Firefox/6.0";
            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = content.Length;

            Thread.Sleep((_rand.Next(5) + 2) * 50);
            using (StreamWriter stOut = new StreamWriter(request.GetRequestStream(), Encoding.ASCII))
            {
                stOut.Write(content);
                stOut.Close();
            }

            WebResponse response = request.GetResponse();
            if (response == null)
                throw new ApplicationException("Could not read programme list from source url -" + request.Address);
            return response.GetResponseStream();
        }

        public static Stream GetUrl(string url, CookieContainer cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookies != null)
                request.CookieContainer = cookies;
            Thread.Sleep((_rand.Next(5) + 2) * 50);
            WebResponse response = request.GetResponse();
            if (response == null)
                throw new ApplicationException("Could not read programme list from source url -" + request.Address);

            return response.GetResponseStream();
        }
    }
}
