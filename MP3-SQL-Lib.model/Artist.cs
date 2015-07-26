using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.model
{
    public class Artist : ObservableObject
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }


        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set { _artistName = value; OnPropertyChanged("ArtistName"); }
        }

        private string _imageURL;

        public string ImageURL
        {
            get { return _imageURL; }
            set { _imageURL = value; OnPropertyChanged("ImageURL"); }
        }

        private byte[] _image;

        public byte[] Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }
        

        private bool _isFavorite;

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { _isFavorite = value; OnPropertyChanged("IsFavorite"); }
        }
        
        

        public Artist() { }
        public Artist(string name)
        {
            this.ArtistName = name;
        }
        public Artist(int id, string name)
        {
            this.ID = id;
            this.ArtistName = name;
        }
    }
}
