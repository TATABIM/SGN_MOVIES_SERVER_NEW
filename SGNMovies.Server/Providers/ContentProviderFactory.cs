using System;
using SGNMovies.Server.Providers;

namespace SGNMovies.Server.Providers
{
    public static class ContentProviderFactory
    {
        public static IContentProvider Create(string id)
        {
            switch (Int32.Parse(id))
            {
                case 1:
                    return new GalaxyProvider();
                case 2:
                    return new MegaStarProvider();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
