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
    public static class SongDA
    {
        public static ObservableCollection<Song> GetSongs()
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            string sql = "SELECT * FROM song ORDER BY ArtistID ASC, AlbumID ASC, ID ASC;";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql);
            while (reader.Read())
            {
                songs.Add(Create(reader));
            }
            reader.Close();
            return songs;
        }

        public static ObservableCollection<Song> GetSongsStarred()
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            string sql = "SELECT * FROM song WHERE Starred = 1 ORDER BY ArtistID ASC, AlbumID ASC, ID ASC;";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql);
            while (reader.Read())
            {
                songs.Add(Create(reader));
            }
            reader.Close();
            return songs;
        }

        public static ObservableCollection<Song> GetSongsByArtistID(int id)
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            string sql = "SELECT * FROM song WHERE ArtistID = @ID ORDER BY AlbumID ASC, ID ASC;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            while (reader.Read())
            {
                songs.Add(Create(reader));
            }
            reader.Close();
            return songs;
        }

        public static ObservableCollection<Song> GetSongsByAlbumID(int id)
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            string sql = "SELECT * FROM song WHERE AlbumID = @ID ORDER BY ID ASC;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            while (reader.Read())
            {
                songs.Add(Create(reader));
            }
            reader.Close();
            return songs;
        }

        public static Song GetSongByArtistAlbumSong(int artistID, int albumID, string name)
        {
            string sql = "SELECT * FROM song WHERE AlbumID = @ALBUM AND ArtistID = @ARTIST AND Song LIKE @NAME;";
            DbParameter parArtist = Database.AddParameter(Database.DBNAME, "@ALBUM", artistID);
            DbParameter parAlbum = Database.AddParameter(Database.DBNAME, "@ARTIST", albumID);
            DbParameter parSong = Database.AddParameter(Database.DBNAME, "@NAME", name);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parArtist, parAlbum, parSong);
            Song s = null;
            reader.Read();
            if (reader.HasRows)
                s = Create(reader);
            reader.Close();
            return s;
        }

        public static Song GetSongById(int id)
        {
            string sql = "SELECT * FROM song WHERE ID = @ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.DBNAME), sql, parID);
            reader.Read();
            Song s = null;
            if (reader.HasRows)
                s = Create(reader);
            reader.Close();
            return s;
        }

        public static int InsertSong(Song s)
        {
            string sql = "INSERT INTO song(AlbumID,ArtistID,Song,Track,Length,Played,Path) VALUES(@AlbumID,@ArtistID,@Name,@Track,@Length,@Played,@Path);";
            DbParameter parAlbum = Database.AddParameter(Database.DBNAME, "@AlbumID", s.AlbumID);
            DbParameter parArtist = Database.AddParameter(Database.DBNAME, "@ArtistID", s.ArtistID);
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Name", s.SongName);
            DbParameter parTrack = Database.AddParameter(Database.DBNAME, "@Track", s.Track);
            DbParameter parLength = Database.AddParameter(Database.DBNAME, "@Length", s.SongLength);
            DbParameter parPlayed = Database.AddParameter(Database.DBNAME, "@Played", s.PlayedAmmount);
            DbParameter parFile = Database.AddParameter(Database.DBNAME, "@Path", s.FilePath);
            return Database.InsertData(Database.GetConnection(Database.DBNAME), sql, parAlbum, parArtist, parName, parTrack, parLength, parPlayed, parFile);
        }

        public static int UpdateSong(Song s)
        {
            string sql = "UPDATE song SET AlbumID=@AlbumID, ArtistID=@ArtistID, Song=@Name, Track=@Track, Length=@Length, Played=@Played, Path=@Path where ID=@ID;";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", s.ID);
            DbParameter parAlbum = Database.AddParameter(Database.DBNAME, "@AlbumID", s.AlbumID);
            DbParameter parArtist = Database.AddParameter(Database.DBNAME, "@ArtistID", s.ArtistID);
            DbParameter parName = Database.AddParameter(Database.DBNAME, "@Name", s.SongName);
            DbParameter parTrack = Database.AddParameter(Database.DBNAME, "@Track", s.Track);
            DbParameter parLength = Database.AddParameter(Database.DBNAME, "@Length", s.SongLength);
            DbParameter parPlayed = Database.AddParameter(Database.DBNAME, "@Played", s.PlayedAmmount);
            DbParameter parFile = Database.AddParameter(Database.DBNAME, "@Path", s.FilePath);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID, parAlbum, parArtist, parName, parTrack, parLength, parPlayed, parFile);
        }

        public static int DeleteSongByID(Song a)
        {
            //ook albums and songs implementeren
            string sql = "DELETE FROM song WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", a.ID);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }

        public static int DeleteSongByArtistID(int id)
        {
            //ook albums and songs implementeren
            string sql = "DELETE FROM song WHERE ArtistID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }

        public static int DeleteSongByAlbumID(int id)
        {
            //ook albums and songs implementeren
            string sql = "DELETE FROM song WHERE AlbumID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        public static int DeleteSong(int id)
        {
            string sql = "DELETE FROM song WHERE ID=@ID";
            DbParameter parID = Database.AddParameter(Database.DBNAME, "@ID", id);
            return Database.ModifyData(Database.GetConnection(Database.DBNAME), sql, parID);
        }


        private static Song Create(IDataRecord record)
        {
            return new Song()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                AlbumID = Int32.Parse(record["AlbumID"].ToString()),
                ArtistID = Int32.Parse(record["ArtistID"].ToString()),
                SongName = record["Song"].ToString(),
                Track = Int32.Parse(record["Track"].ToString()),
                SongLength = Int32.Parse(record["Length"].ToString()),
                PlayedAmmount = Int32.Parse(record["Played"].ToString()),
                FilePath = record["Path"].ToString()
                //Albums = Albums.GetAlbumsBySong(record["ID"].ToString())
            };
        }
    }
}
