using Lastfm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.helper
{
    public static class LastFM
    {
        private static string _apikey = "18ac9fefbe1cc3895caeabe2c2197b63";
        private static string _apisecret = "0ab52b644280c47678a64d0b04b485c1";

        private static string _username = "hairydruidy";
        private static string _password = "luna1991";
        private static Session _session;
        private static Session GetSession()
        {
            if (_session == null)
            {
                _session = new Session(_apikey, _apisecret);
                //_session.Authenticate(_username, Lastfm.Utilities.md5(_password));
            }
            return _session;
        }
        
        public static async Task<string> GetArtistImageURL(string artistname)
        {
            Artist artist = new Artist(artistname, GetSession());
            string url = string.Empty;
            try
            {
                url = await Task.Run(() => artist.GetImageURL(ImageSize.Large));
            }
            catch (Exception ex)
            {
                Console.WriteLine("lastfm: couldn't fetch large image: " + ex.StackTrace);
            }

            if (string.IsNullOrEmpty(url))
            {
                try
                {
                    url = await Task.Run(() => artist.GetImageURL(ImageSize.Medium));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("lastfm: couldn't fetch medium image: " + ex.StackTrace);
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                try
                {
                    url = await Task.Run(() => artist.GetImageURL(ImageSize.Small));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("lastfm: couldn't fetch small image: " + ex.StackTrace);
                }
            }
            return url;
            
        }
    }
}
