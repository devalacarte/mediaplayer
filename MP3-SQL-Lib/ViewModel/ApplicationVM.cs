using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MP3_SQL_Lib.helper;
using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

//last fm info
//API Key: 18ac9fefbe1cc3895caeabe2c2197b63
//Secret: is 0ab52b644280c47678a64d0b04b485c1

namespace MP3_SQL_Lib.ViewModel
{
    class ApplicationVM : ObservableObject
    {

        private const string permatitle= "My Music Library";
        private string _title = permatitle;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }
        
        public ApplicationVM()
        {
            Messenger.Default.Register<NotificationMessage<Song>>(this, (message) => NotificationMessageHandlerString(message));
            
            Pages.Add(new PageOneVM());
            // Add other pages
            /*Pages.Add(new PlayBarVM());*/
            PlayBar = new PlayBarVM();
            BrowseArtists = new BrowseArtistsVM();
            CurrentPage = Pages[0];
            
            
            //ImportSQL("H:\\Alle Muziek\\Music", "*.mp3", true);
            GetAllData();

        }


        #region messages
        //message opvangen wanneer er muziek wordt afgespeeld, en de titlebar song + artist wijzigen
        private void NotificationMessageHandlerString(NotificationMessage<Song> message)
        {
            //throw new NotImplementedException();
            string song = message.Content.SongName;
            Artist artist = Artists.First(x => x.ID == message.Content.ArtistID);
            
            switch (message.Notification)
            {
                case MVVMMessages.Messages.MUSIC_NEW_SONG:
                    Title = "Now playing: " + song;
                    if (artist!=null)
                        Title += " By " + artist.ArtistName;
                    break;
                case MVVMMessages.Messages.MUSIC_PAUSE:
                    Title = "Pauzed: " + song;
                    if (artist!=null)
                        Title += " By " + artist.ArtistName;
                    break;
                case MVVMMessages.Messages.MUSIC_RESUME:
                    Title = "Now playing: " + song;
                    if (artist!=null)
                        Title += " By " + artist.ArtistName;
                    break;
                case MVVMMessages.Messages.MUSIC_STOPPED:
                    Title = permatitle;
                    break;
                default:
                    break;
            }
        }
        #endregion messages





        #region ViewModels
        private object currentPage;
        public object CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged("CurrentPage"); }
        }
        private PlayBarVM _playBar;
        public PlayBarVM PlayBar
        {
            get { return _playBar; }
            set { _playBar = value; OnPropertyChanged("PlayBar"); }
        }
        private BrowseArtistsVM _browseArtists;

        public BrowseArtistsVM BrowseArtists
        {
            get { return _browseArtists; }
            set { _browseArtists = value; OnPropertyChanged("BrowseArtists"); }
        }
        

        private List<IPage> pages;
        public List<IPage> Pages
        {
            get
            {
                if (pages == null)
                    pages = new List<IPage>();
                return pages;
            }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }

        private void ChangePage(IPage page)
        {
            CurrentPage = page;
        }


        #endregion ViewModels



















        #region CollectionProperties AND GetData

        private ObservableCollection<Song> _songs;
        public ObservableCollection<Song> Songs
        {
            get { return _songs; }
            set { _songs = value; OnPropertyChanged("Songs");}
        }

        private ObservableCollection<Song> _favorites;
        public ObservableCollection<Song> Favorites
        {
            get { return _favorites; }
            set { _favorites = value; OnPropertyChanged("Favorites");}
        }

        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get { return _albums; }
            set { _albums = value; OnPropertyChanged("Albums"); }
        }

        private ObservableCollection<Artist> _artists;

        public ObservableCollection<Artist> Artists
        {
            get { return _artists; }
            set { _artists = value; OnPropertyChanged("Artists"); }
        }
        
        private async void GetAllData()
        {

            Task taskData = GetAllDataTask();
            await taskData;

            Task taskDataLoaded = taskData.ContinueWith(t => GetAllDataLoadedTask());
            taskDataLoaded.Wait();
        }

        private async Task GetAllDataTask()
        {
            Artists = await Task.Run(() => ArtistDA.GetArtists());
            Albums = await Task.Run(() => AlbumDA.GetAlbums());
            Favorites = await Task.Run(() => SongDA.GetSongsStarred());
            Songs = await Task.Run(() => SongDA.GetSongs());
        }

        private async Task GetAllDataLoadedTask()
        {
            PlayBar.PlayList = Songs;
            PlayBar.PlayListPos = 0;
            BrowseArtists.Artists = await Task.Run(() => ArtistDA.GetArtists());
            //int res = await CheckArtistImages();
        }


        #endregion CollectionProperties AND GetData


        private async Task<int> CheckArtistImages()
        {
            int picsfound = 0;
            foreach (Artist artist in Artists)
            {
                if(artist.Image == null)
                {
                    artist.ImageURL = await MP3_SQL_Lib.helper.LastFM.GetArtistImageURL(artist.ArtistName);
                    if (string.IsNullOrEmpty(artist.ImageURL))
                        continue;
                    artist.Image = await MP3_SQL_Lib.helper.FileOperations.GetBytesFromUrlDownloadData(artist.ImageURL);
                    if(Properties.Settings.Default.SaveImages==true)
                        ArtistDA.UpdateArtist(artist);
                    picsfound += 1;
                }
            }
            return picsfound;
        }














        private async void ImportSQL(string path, string pattern, bool subdirs)
        {
            //haalt alle files op aan de hand van het path, pattern, en of hij subdirs moe doorlopen met een string[] als result. 
            //Deze wordt gebruikt om van ieder gevonden path de song gegevens op te halen en in de db te steken
            await Task.Run(() => MP3_SQL_Lib.helper.TagLibFunctions.ImportMultipleMP3IntoDB(MP3_SQL_Lib.helper.FileOperations.GetFilesInDirectoryByPatern(path, pattern, subdirs)));
        }
    }
}
