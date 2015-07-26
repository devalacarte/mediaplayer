using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MP3_SQL_Lib.helper;
using MP3_SQL_Lib.model;
using System.Collections.ObjectModel;
namespace MP3_SQL_Lib.View
{
    /// <summary>
    /// Interaction logic for PageOne.xaml
    /// </summary>
    public partial class PageOne : UserControl
    {
        //private ObservableCollection<CSCore.CoreAudioAPI.MMDevice> _devices = new ObservableCollection<CSCore.CoreAudioAPI.MMDevice>();
        //private MusicPlayer mp = new MusicPlayer();
        public PageOne()
        {
            InitializeComponent();
            TestDatabaseAccessInsertsAndGet();
           
        }

        private void TestDatabaseAccessInsertsAndGet()
        {
            //code to test data access
            /*MP3_SQL_Lib.helper.ArtistDA.InsertArtist(new Artist("whatafaak"));
            Artist a = ArtistDA.GetArtistByName("whatafaak");
            Album alNew = new Album();
            alNew.ArtistID = a.ID;
            //alNew.Cover = new Byte[0];
            alNew.Name = "hiy123456";
            AlbumDA.InsertAlbum(alNew);
            ObservableCollection<Album> al = AlbumDA.GetAlbumsByArtist(a);
            Song s = new Song(); s.AlbumID = 1; s.ArtistID = 2; s.FilePath = "hahahah.mp3"; s.PlayedAmmount = 0; s.SongLength = 10000; s.SongName = "kakaka"; s.Track = 1;
            SongDA.InsertSong(s);
            Song ss = SongDA.GetSongById(1);*/

            /*string[] files = FileOperations.GetFilesInDirectoryByPatern("H:\\Alle Muziek\\Music\\3 doors down","*.mp3",true);
            foreach (string file in files)
            {
                TagLib.File f = TagLib.File.Create(file);
                Console.WriteLine(f.Tag.FirstAlbumArtist + " - " + f.Tag.Album + " - " + f.Tag.Title);
            }*/

            
            //FileOperations.Test();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }


    }
}
