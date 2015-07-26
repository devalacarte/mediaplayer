using Lastfm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lastfm_sharp_test.ViewModel
{
    class PageOneVM : ObservableObject, IPage
    {

        public string Name
        {

            get { return "First page"; }

        }

        private string _demo = "demo.";
        public string Demo
        {

            get { return _demo; }
            set { _demo = value; OnPropertyChanged("Demo"); }

        }

        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set { _artistName = value; OnPropertyChanged("ArtistName"); GetArtist(); }
        }


        private string _artistImage;
        public string ArtistImage
        {
            get { return _artistImage; }
            set { _artistImage = value; OnPropertyChanged("ArtistImage"); }
        }



        string apikey = "18ac9fefbe1cc3895caeabe2c2197b63";
        string apisecret = "0ab52b644280c47678a64d0b04b485c1";
        Session session;

        public PageOneVM()
        {
            session = new Session(apikey, apisecret);
            //string url = session.GetWebAuthenticationURL();
        }

        private async void GetArtist()
        {
            Artist artist = new Artist(ArtistName, session);
            ArtistImage = string.Empty;
            try
            {
                ArtistImage = await Task.Run(() => artist.GetImageURL(ImageSize.Large));
            }
            catch (Exception ex)
            {
                Console.WriteLine("lastfm: couldn't fetch large image: " + ex.StackTrace);
            }

            if (string.IsNullOrEmpty(ArtistImage))
            {
                try
                {
                    ArtistImage = await Task.Run(() => artist.GetImageURL(ImageSize.Medium));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("lastfm: couldn't fetch medium image: " + ex.StackTrace);
                }
            }

            if (string.IsNullOrEmpty(ArtistImage))
            {
                try
                {
                    ArtistImage = await Task.Run(() => artist.GetImageURL(ImageSize.Small));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("lastfm: couldn't fetch small image: " + ex.StackTrace);
                }
            }
            
        }
        
        public void GetImage()
        {
            //ArtistImage = MP3_SQL_Lib.helper.FileOperations.GetBytesFromUrlDownloadData("http://home.deds.nl/~zoheb/Week4/Img/Linkin-Park-linkin-park-776343_1280_1024.jpg");
            //ArtistImage = "http://home.deds.nl/~zoheb/Week4/Img/Linkin-Park-linkin-park-776343_1280_1024.jpg";
        }
    }
}
