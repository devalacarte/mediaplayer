using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.helper
{
    public static class AlbumDA
    {
        public static ObservableCollection<Album> GetAlbums()
        {
            ObservableCollection<Album> albums = new ObservableCollection<Album>();
            string sql = "SELECT * FROM album ORDER BY ArtistID ASC, Album ASC;";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql);
            while (reader.Read())
            {
                albums.Add(Create(reader));
            }
            reader.Close();
            return albums;
        }

        public static Album GetAlbumById(int id)
        {
            string sql = "SELECT * FROM album WHERE ID = @ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            reader.Read();
            Album a = null;
            if (reader.HasRows)
                a = Create(reader);
            reader.Close();
            return a;
        }

        public static Album GetAlbumByArtistIDAndAlbumName(int id, string name)
        {
            string sql = "SELECT * FROM album WHERE ArtistID = @ID AND Album LIKE @NAME ORDER BY ID ASC;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@NAME", name);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID, parName);
            reader.Read();
            Album a = null;
            if (reader.HasRows)
                a = Create(reader);
            reader.Close();
            return a;
        }

        public static ObservableCollection<Album> GetAlbumsByArtist(Artist ar)
        {
            ObservableCollection<Album> albums = new ObservableCollection<Album>();
            string sql = "SELECT * FROM album WHERE ArtistID = @ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", ar.ID);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            while (reader.Read())
            {
                albums.Add(Create(reader));
            }
            reader.Close();
            return albums;
        }

        public static int InsertAlbum(Album a)
        {
            string sql = "INSERT INTO album(Album, ArtistID, Cover) VALUES(@Album,@ArtistID,@Cover);";
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Album", a.Name);
            DbParameter parArtistID = Database.AddParameter(Database.DBNAME, "@ArtistID", a.ArtistID);
            DbParameter parCover = Database.AddParameter(Database.DBNAME, "@Cover", a.Cover);
            return Database.InsertData(Database.GetConnection(Database.DBNAME), sql, parName, parArtistID, parCover);
        }

        public static int UpdateAlbum(Album a)
        {
            string sql = "UPDATE album SET Album=@Album, ArtistID=@ArtistID, Cover=@Cover where ID=@ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", a.ID);
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Album", a.Name);
            DbParameter parArtistID = Database.AddParameter(Database.DBNAME, "@ArtistID", a.ArtistID);
            DbParameter parCover = Database.AddParameter(Database.DBNAME, "@Cover", a.Cover);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID, parName, parArtistID, parCover);
        }

        public static int DeleteAlbum(Album a)
        {
            //ook albums and songs implementeren
            string sql = "DELETE FROM album WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", a.ID);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        public static int DeleteAlbum(int id)
        {
            string sql = "DELETE FROM album WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        private static Album Create(IDataRecord record)
        {
            //als opgehaalde foto in database != null, haal de foto op, maak anders een nieuwe array aan
            byte[] cover = (!DBNull.Value.Equals(record["Cover"])) ? (byte[])record["Cover"] : new byte[0];
            
            return new Album()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                Name = record["Album"].ToString(),
                ArtistID = Int32.Parse(record["ArtistID"].ToString()),
                Cover = cover
            };
        }
    }
}

