using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.model
{
    public class Song
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }




        private int _albumID;
        public int AlbumID
        {
            get { return _albumID; }
            set { _albumID = value; }
        }




        private int _artistID;
        public int ArtistID
        {
            get { return _artistID; }
            set { _artistID = value; }
        }





        private string _song;
        public string SongName
        {
            get { return _song; }
            set { _song = value; }
        }



        private int _track;

        public int Track
        {
            get { return _track; }
            set { _track = value; }
        }
        


        private int _playedAmmount;
        public int PlayedAmmount
        {
            get { return _playedAmmount; }
            set { _playedAmmount = value; }
        }




        private int _songLength;
        public int SongLength
        {
            get { return _songLength; }
            set { _songLength = value; }
        }



        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }



        public Song() { }
        public Song(int albumID, int artistID, string song)
        {
            this.AlbumID = albumID;
            this.ArtistID = artistID;
            this.SongName = song;
        }
        public Song(int id, int albumID, int artistID, string song)
        {
            this.ID = id;
            this.AlbumID = albumID;
            this.ArtistID = artistID;
            this.SongName = song;
        }
    }
}
