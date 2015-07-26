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
    public static class ArtistDA
    {
        public static ObservableCollection<Artist> GetArtists()
        {
            ObservableCollection<Artist> artists = new ObservableCollection<Artist>();
            string sql = "SELECT * FROM artist ORDER BY Artist ASC;";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql);
            while (reader.Read())
            {
                artists.Add(Create(reader));
            }
            reader.Close();
            return artists;
        }

        public static Artist GetArtistById(int id)
        {
            string sql = "SELECT * FROM artist WHERE ID = @ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            reader.Read();
            Artist a = null;
            if (reader.HasRows)
                a=Create(reader);
            reader.Close();
            return a;
        }

        public static Artist GetArtistByName(string name)
        {
            string sql = "SELECT * FROM artist WHERE Artist Like @Name;";
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Name", name);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parName);
            reader.Read();
            Artist a = null;
            if (reader.HasRows)
                a = Create(reader);
            reader.Close();
            return a;
        }
        public static int InsertArtist(Artist a)
        {
            string sql = "INSERT INTO artist(artist) VALUES(@Artist);";
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Artist", a.ArtistName);
            return Database.InsertData(Database.GetConnection(Database.DBNAME), sql, parName);
        }

        public static int UpdateArtist(Artist a)
        {
            string sql = "UPDATE artist SET artist=@Artist, image=@Image where ID=@ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", a.ID);
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Artist", a.ArtistName);
            DbParameter parImage = Database.AddParameter(Database.DBNAME, "@Image", a.Image);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID, parName, parImage);
        }

        public static int DeleteArtist(Artist a)
        {
            //ook albums and songs implementeren
            string sql = "DELETE FROM artist WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", a.ID);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        public static int DeleteArtist(int id)
        {
            string sql = "DELETE FROM artist WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        private static Artist Create(IDataRecord record)
        {
            byte[] pic = (!DBNull.Value.Equals(record["Image"]))?(byte[])record["Image"]:null;
            return new Artist()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                ArtistName = record["Artist"].ToString(),
                Image = pic
            };
        }
    }
}
