using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.model
{
   public class Album
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }


        private int _artistID;
        public int ArtistID
        {
            get { return _artistID; }
            set { _artistID = value; }
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private byte[] _cover;
        public byte[] Cover
        {
            get { return _cover; }
            set { _cover = value; }
        }
        
        public Album() { }
        public Album(string name, int artistID)
        {
            this.Name = name;
            this.ArtistID = artistID;
        }
       public Album(int id, string name, int artistID)
        {
            this.ID = id;
            this.Name = name;
            this.ArtistID = artistID;
        }
    }
}
