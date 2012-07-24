using System.Collections.Generic;

namespace SGNMovies.Server.Models
{
    public class ShowingDateModel
    {
        public string ShowingDate { get; set; }
        public List<ShowingTimeModel> ShowingTimes { get; set; }

        public ShowingDateModel()
        {
            ShowingTimes = new List<ShowingTimeModel>();
        }
    }
}