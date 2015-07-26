using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;

namespace MP3_SQL_Lib.helper
{
    public static class TagLibFunctions
    {
        
        public static void ImportSingleMP3IntoDB(string path)
        {
            TagLib.File f = TagLib.File.Create(path);
            
            //checken of artist al in db zit, zoja artist ophalen; zo nee, nieuwe aanmaken
            Artist ar = null;
            if (!string.IsNullOrEmpty(f.Tag.FirstAlbumArtist))
                ar = ArtistDA.GetArtistByName(f.Tag.FirstAlbumArtist);
            else
            {
                ar = (f.Tag.Artists.Length > 0) ? (ArtistDA.GetArtistByName(f.Tag.Artists[0])):(null);
            }

            if (ar == null)
            {
                if (!string.IsNullOrEmpty(f.Tag.FirstAlbumArtist))
                    ar = new Artist(f.Tag.FirstAlbumArtist);
                else if (f.Tag.Artists.Length > 0)
                    ar = new Artist(f.Tag.Artists[0]);
                else
                    ar = new Artist("Andere");


                ar.ID = ArtistDA.InsertArtist(ar);
                Console.WriteLine("DB: created new Artist - " + ar.ArtistName);
            }

            //checken of de album al in db zit aan de hand van albumnaam en artist id, zoja, album ophalen. Zo nee, nieuwe aanmaken
            Album al =null;
            if (!string.IsNullOrEmpty(f.Tag.Album))
                al = AlbumDA.GetAlbumByArtistIDAndAlbumName(ar.ID, f.Tag.Album);
            if(al == null)
            {
                string albumname = (!String.IsNullOrEmpty(f.Tag.Album)) ? (f.Tag.Album) : ("Andere");
                al = new Album(albumname, ar.ID);
                al.ID = AlbumDA.InsertAlbum(al);
                Console.WriteLine("DB: created new Album - " + al.Name);
            }


            //checken of da et liedje al in de database zit adhb artist / album id en songname, anders in db steken
            Song s = SongDA.GetSongByArtistAlbumSong(ar.ID, al.ID, f.Tag.Title);
            if (s==null)
            {
                s = new Song();
                s.AlbumID = al.ID;
                s.ArtistID = ar.ID;
                s.SongName = f.Tag.Title;
                s.Track = (int)f.Tag.Track;

                ShellFile so = ShellFile.FromFilePath(path);
                double nanoseconds;
                double.TryParse(so.Properties.System.Media.Duration.Value.ToString(),
                out nanoseconds);
                s.SongLength = (int)Math.Round((nanoseconds / 10000000), 0);
                s.PlayedAmmount = 0;
                s.FilePath = path;
                int songID = SongDA.InsertSong(s);
                s.ID = songID;
                Console.WriteLine("DB: created new Song - " + s.SongName);
            }
            
        }

        public static int ImportMultipleMP3IntoDB(string[] paths)
        {
            int ammountImported = 0;
            foreach (string path in paths)
            {
                try
                {
                    ImportSingleMP3IntoDB(path);
                    ammountImported += 1;
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    //throw;
                }
                
            }
            return ammountImported;
        }
        
        /*public static MP3Tag GetMP3TagFromFilePath(string path)
        {
            TagLib.File f = TagLib.File.Create(path);
            MP3Tag m = new MP3Tag();
            Artist ar = new Artist(f.Tag.FirstAlbumArtist);
            Album al = new Album()
            
               
            return m;
        }*/
    }
}
