using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.model
{
    public class MP3Tag
    {
        private Album _album;

        public Album Album
        {
            get { return _album; }
            set { _album = value; }
        }

        private Artist _artist;

        public Artist Artist
        {
            get { return _artist; }
            set { _artist = value; }
        }

        private Song _song;

        public Song Song
        {
            get { return _song; }
            set { _song = value; }
        }

        public MP3Tag()
        {

        }
        
        public MP3Tag(Artist ar, Album al, Song s)
        {
            this.Artist = ar;
            this.Album = al;
            this.Song = s;
        }

        
    }

    
}
