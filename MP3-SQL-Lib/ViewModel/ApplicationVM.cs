using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
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
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
            Messenger.Default.Register<NotificationMessage<Song>>(this, (message) => NotificationMessageHandlerSong(message));
            Messenger.Default.Register<NotificationMessage<Artist>>(this, (message) => NotificationMessageHandlerArtist(message));
            
            PlayBarBASS = new PlayBarBASSVM();
            BrowseArtists = new BrowseArtistsVM();
            CurrentPage = BrowseArtists;

            //ImportSQL("H:\\Alle Muziek\\Music", "*.mp3", true);
            //GetAllData();
            //GetDataFromDB();

            //test();
        }

        /*private void test()
        {
            Task sqlTask = ImportSQL("H:\\muziek\\Alle Muziek", "*.mp3", true);
            sqlTask.Wait();

            sqlTask.ContinueWith(t => Task.Run(() => GetAllData()));
        }*/

        public ICommand LoadedCommand
        {
            get { return new RelayCommand(Loaded); }
        }
        private async void Loaded()
        {
            Task[] tasks = new Task[3];
            //Task[] tasks = new Task[4];
            tasks[0] = Task.Run(() => { Artists = ArtistDA.GetArtists(); });
            tasks[1] = Task.Run(() => { Albums = AlbumDA.GetAlbums(); });
            tasks[2] = Task.Run(() => { Songs = SongDA.GetSongs(); });
            //tasks[3] = Task.Run(() => { System.Threading.Thread.Sleep(15000); });
            await Task.WhenAll(tasks);
            //PlayBarBASS.PlayList = Songs;
            //PlayBarBASS.PlayListPos = 0;
        }

        #region messages
        //message opvangen wanneer er muziek wordt afgespeeld, en de titlebar song + artist wijzigen
        private async void NotificationMessageHandlerSong(NotificationMessage<Song> message)
        {
            //throw new NotImplementedException();
            string song = message.Content.SongName;
            if (Artists == null)
                return;
            Artist artist = null;
            try
            {
                artist = await Task<Artist>.Run(() => {return Artists.First(x => x.ID == message.Content.ArtistID);  });  //koppel song ArtistID aan artist name uit artistcollection
            }
            catch (Exception)
            {
                Console.WriteLine("AppVM: NotificationMessageHandler: Can't find ArtistName from Song.ArtistID");                
            }
            
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
        private async void NotificationMessageHandlerArtist(NotificationMessage<Artist> message)
        {
            if (message.Notification == MVVMMessages.Messages.ARTIST_PLAY)
            {
                Artist ar = message.Content;
                ObservableCollection<Song> songs = await Task<ObservableCollection<Song>>.Run(() => { return helper.SongDA.GetSongsByArtistID(ar.ID); });
                PlayBarBASS.StopStream();
                PlayBarBASS.PlayList = songs;
                PlayBarBASS.PlayListPos = 0;
            }
            if(message.Notification == MVVMMessages.Messages.ARTIST_VIEW)
            {

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
        private PlayBarBASSVM _playBarBASS;
        public PlayBarBASSVM PlayBarBASS
        {
            get { return _playBarBASS; }
            set { _playBarBASS = value; OnPropertyChanged("PlayBarBASSVM"); }
        }
        private BrowseArtistsVM _browseArtists;

        public BrowseArtistsVM BrowseArtists
        {
            get { return _browseArtists; }
            set { _browseArtists = value; OnPropertyChanged("BrowseArtists"); }
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
        
        /*private async void GetAllData()
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
            PlayBarBASS.PlayList = Songs;
            PlayBarBASS.PlayListPos = 0;
            BrowseArtists.Artists = await Task.Run(() => ArtistDA.GetArtists());
            //int res = await CheckArtistImages();
        }*/
        private void GetDataFromDB()
        {
            Task task = GetDataFromDBTask();
        }
        private async Task GetDataFromDBTask()
        {
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() => {Artists = ArtistDA.GetArtists();});
            tasks[1] = Task.Run(() => {Albums = AlbumDA.GetAlbums();});
            tasks[2] = Task.Run(() => {Songs = SongDA.GetSongs();});
            await Task.WhenAll(tasks);
        }
        private async Task GetDataFromDBTaskTWo()
        {
            await Task.Run(() =>
            {
                Artists = ArtistDA.GetArtists();
            });
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
