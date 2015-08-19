using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.ViewModel
{
    public class ArtistDetailView : ObservableObject
    {

        private Artist _artist;

        public Artist ArtistDetail
        {
            get { return _artist; }
            set { _artist = value; OnPropertyChanged("ArtistDetail"); ArtistChanged(); }
        }


        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get { return _albums; }
            set { _albums = value; OnPropertyChanged("Albums"); }
        }
        




        public ArtistDetailView()
        {

        }

        private void ArtistChanged()
        {
            if (ArtistDetail == null)
                return;
            Albums = helper.AlbumDA.GetAlbumsByArtist(ArtistDetail);
        }
    }
}
