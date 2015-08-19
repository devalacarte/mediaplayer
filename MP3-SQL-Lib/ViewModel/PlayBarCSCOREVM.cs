using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MP3_SQL_Lib.helper;
using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace MP3_SQL_Lib.ViewModel
{
    /*
     * Shuffling Logic:
     * we want to:
     *      be able to play the next song in the list by picking a random song in the playlist. random.next(ammountOfSongsInPlayList)
     *      store the played songs so we can go back to the correct song. _playedsonglist with integer op positions in SongsInPlayList
     *      when we hit next on a previously played song (hit the previous button a few times)     
     *          Do not store the song
     *          Do not use a random number
     *      when shuffling is enabled, and disabled again, we want to continue playing music from set position in the playlist (ignoring the fact if we've already played the future or not)
    */
    public class PlayBarCSCOREVM : ObservableObject, IPage
    {
        /// <summary>
        /// Functies voor het eigenlijke afspelen van de muziek
        /// </summary>
        private readonly MusicPlayerCSCore _musicPlayer = new MusicPlayerCSCore();
        private bool _stopSliderUpdate;
        /// <summary>
        /// used to send currently playing data like (when to hit next song, current possition in song, ...)
        /// </summary>
        private static Timer PlayTimer;

        #region Properties
        private List<MMDevice> _devices = new List<MMDevice>();
        /// <summary>
        /// Bevat de geluidskaarten
        /// </summary>
        public List<MMDevice> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged("Devices"); }
        }
        

        private MMDevice _selectedSoundCard;
        /// <summary>
        /// Geluidskaart waar muziek op wordt afgespeeld
        /// </summary>
        public MMDevice SelectedSoundCard
        {
            get { return _selectedSoundCard; }
            set { _selectedSoundCard = value; OnPropertyChanged("SelectedSoundCard");}
        }

        private ObservableCollection<Song> _playedList;
        private ObservableCollection<Song> _toPlayList;
        private ObservableCollection<Song> _originalPlayList;
        /// <summary>
        /// Normal playlist to use without shuffling
        /// </summary>
        public ObservableCollection<Song> PlayList
        {
            get { return _originalPlayList; }
            set { _originalPlayList = value; _toPlayList = value; _positionInPrev = -1; OnPropertyChanged("PlayList"); }
        }

        private int _playListPos=0;
        /// <summary>
        /// position in playlist
        /// </summary>
        public int PlayListPos
        {
            get { return _playListPos; }
            set
            {
                if ((value >= 0) && (value <= PlayList.Count))
                {
                    _playListPos = value; OnPropertyChanged("PlayListPos");
                    //als posinprev >=0 wil dit zeggen dat we liedjes afspelen die al afgespeeld zijn door gebruik te maken van previous
                    //-1 = andere afspeellijst dan normaal
                    if(_positionInPrev == -1) 
                    {
                        SelectedSong = _toPlayList[value];
                    }
                    else
                        SelectedSong = _playedList[value];
                    Play();
                }
            } 
        }
        
        
        private Song _selectedSong;
        /// <summary>
        /// currently playing song
        /// </summary>
        public Song SelectedSong
        {
            get { return _selectedSong; }
            set { 
                _selectedSong = value; OnPropertyChanged("SelectedSong");
                if (SelectedSong != null)
                {
                    if (System.IO.File.Exists(value.FilePath)) { Open(); BtnPlayEnabled = true;}
                }
            }
        }
        #endregion properties

        #region BindingProperties
        private bool _btnPlayEnabled = true;
        public bool BtnPlayEnabled
        {
            get { return _btnPlayEnabled; }
            set { _btnPlayEnabled = value; OnPropertyChanged("BtnPlayEnabled"); }
        }
        
        
        private bool _btnPauseEnabled;
        public bool BtnPauseEnabled
        {
            get { return _btnPauseEnabled; }
            set { _btnPauseEnabled = value; OnPropertyChanged("BtnPauseEnabled"); }
        }
        
        
        private bool _btnStopEnabled;
        public bool BtnStopEnabled
        {
            get { return _btnStopEnabled; }
            set { _btnStopEnabled = value; OnPropertyChanged("BtnStopEnabled"); }
        }

        
        private int _sldVolume;
        public int SliderVolume
        {
            get { _sldVolume = (Properties.Settings.Default.Volume >= 0) ? (Properties.Settings.Default.Volume) : (_musicPlayer.Volume); ; return _sldVolume; }
            set { _sldVolume = Properties.Settings.Default.Volume = _musicPlayer.Volume = value; OnPropertyChanged("SliderVolume"); Properties.Settings.Default.Save(); }
        }

        
        private double _sldTime=0;
        public double SliderTime
        {
            get { return _sldTime; }
            set { _sldTime = value; OnPropertyChanged("SliderTime"); }
        }

        
        private string _timePlayed = "0:00:00";
        public string TimePlayed
        {
            get { return _timePlayed; }
            set { _timePlayed = value; OnPropertyChanged("TimePlayed"); }
        }

        
        private string _timeTotal = "0:00:00";
        public string TimeTotal
        {
            get { return _timeTotal; }
            set { _timeTotal = value; OnPropertyChanged("TimeTotal"); }
        }

        #endregion BindingProperties
        

        private bool _isShuffle = false;
        public bool IsShuffle
        {
            get { return _isShuffle; }
            set { _isShuffle = value; OnPropertyChanged("Shuffle"); }
        }
        

        public string Name
        {
            get { return "PlayBar"; }
        }


        























        public PlayBarCSCOREVM()
        {
            _musicPlayer.PlaybackStopped += (s, args) =>
            {
                BtnPlayEnabled = BtnStopEnabled = BtnPauseEnabled = false;
            };

            //zoek alle audio toestellen, en selecteer het eerst gevonden toestel indien er geen settings zijn opgeslagen
            Devices = CSAudio.GetSoundDevices();
            int sounddev = Properties.Settings.Default.SoundCard;
            SelectedSoundCard = ((sounddev <= -1) || (sounddev > Devices.Count)) ? (CSAudio.GetDefaultSoundDevice()) : (Devices[sounddev]);
            
            PlayList = new ObservableCollection<Song>();

            PlayTimer = new Timer();
            PlayTimer.Interval = 1000;
            PlayTimer.Elapsed += PlayTimer_Elapsed;

            _playedList = new ObservableCollection<Song>();
        }
        
        


        /// <summary>
        /// oproepen bij het veranderen van ieder lied, of door te klikken op een nieuw lied. Of door het afspelen naar het volgend lied in afspeellijst
        /// zet het lied in de player, en krijgt lengte en volume blablabla
        /// afspelen met player.play staat al in deze functie
        /// </summary>
        public void Open()
        {
            if (SelectedSong == null || SelectedSoundCard == null)
                return;
            if (String.IsNullOrEmpty(SelectedSong.FilePath))
                return;
            try
            {
                _musicPlayer.Open(SelectedSong.FilePath, SelectedSoundCard);
                _musicPlayer.Volume = SliderVolume;
                //_musicPlayer.Position = _musicPlayer.Length - new TimeSpan(0, 0, 10); //diende om het testen van next song (speelde enkele laatste 10 sec van liedje af)
                BtnPlayEnabled = true;
                BtnPauseEnabled = BtnStopEnabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine ("kan musicplayer niet openen:" + ex.StackTrace);
            }
        }





        #region ICOMMANDS
        public ICommand PlayCommand
        {
            get { return new RelayCommand(Play, canPlay); }
        }
        public void Play()
        {
            if (_musicPlayer.PlaybackState != PlaybackState.Playing)
            {
                _musicPlayer.Play();
                PlayTimer.Enabled = true;
                Messenger.Default.Send (new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_NEW_SONG));
                BtnPlayEnabled = false;
                BtnPauseEnabled = BtnStopEnabled = true;
            }
        }
        private bool canPlay()
        {
            if (SelectedSong == null)
                return false;
            if (!String.IsNullOrEmpty(SelectedSong.FilePath) && BtnPlayEnabled == true)
                return true;
            else
                return false;
        }

        public ICommand StopCommand
        {
            get { return new RelayCommand(stop, canStop); }
        }
        private void stop()
        {
            if (_musicPlayer.PlaybackState != PlaybackState.Stopped)
            {
                PlayTimer.Enabled = false;
                _musicPlayer.Stop();
                BtnPlayEnabled = true;
                BtnStopEnabled = BtnPauseEnabled = false;
                Messenger.Default.Send(new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_STOPPED));
            }
        }
        private bool canStop()
        {
            return BtnStopEnabled;
        }

        public ICommand PauseCommand
        {
            get { return new RelayCommand(pause, canPause); }
        }
        private void pause()
        {
            if (_musicPlayer.PlaybackState == PlaybackState.Playing)
            {
                PlayTimer.Enabled = false;
                _musicPlayer.Pause();
                Messenger.Default.Send(new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_PAUSE)); 
                BtnPauseEnabled = false;
                BtnPlayEnabled = BtnStopEnabled = true;
            }
        }
        private bool canPause()
        {
            return BtnPauseEnabled;
        }


        public ICommand PrevCommand
        {
            get { Console.WriteLine("prev clicked"); return new RelayCommand(prevCommand); }
        }

        /// <summary>
        /// when we use previous we want to save how many times we went back, and be able to use next to undo
        /// -1 means we're not playing any previous song.
        /// </summary>
        private int _positionInPrev = -1;
        /// <summary>
        /// when less than 20 % of the song is played, go to previous track, else start at the beginning of the current song
        /// </summary>
        private void prevCommand()
        {
            Console.WriteLine("entered prevcommand");
            if (_playedList == null)
                return;

            if (SliderTime < 20)
            {
                if (_playedList.Count > 0) //check if there are any played tracks yet
                {
                    if (_positionInPrev == _playedList.Count - 1) //allereerste lied is al geladen, kan niet meer terug, speel af van lied begin
                    {
                        Console.WriteLine("eerste lied is al geladen + playlistpos");
                        PlayListPos = _playListPos;
                    }
                    else
                    {
                        Console.WriteLine("ga een lied terug + playlistpos");
                        _positionInPrev += 1;
                        PlayListPos = ((_playedList.Count -1) - _positionInPrev); //ammount of tracks played - how many tracks we've already turned back -1 (list is index)
                    }
                } 
            }
            else
            {
                Console.WriteLine("lied is al te lang aan het spelen, ga terug naar start + playliststart");
                PlayListPos = _playListPos; //herstart van beginpunt van lied
            }
        }

        public ICommand NextCommand
        {
            get { Console.WriteLine("pressed next"); return new RelayCommand(nextCommand, canNext); }
        }
        private void nextCommand()
        {
            /*
             * check if we're playing a song from the previous list.
             * if not: remove current song from current list and add to previous list
             *          check if we're shuffling if not, set playpos in currentlist back to 0
             *          if we are, random song from currentlist
             * if we are playing a song from the previous list
             *          raise the previous song counter by one. Check if we still have a positive value.
             *          if we do, play the next previous song
             *          if we have a 0, play song from currentplaylist
             *          
            */
            Console.WriteLine("next command");
            if (PlayList == null)
                return;

            if (PlayListPos < _originalPlayList.Count)
            {
                if(_positionInPrev == -1)
                {
                    Console.WriteLine("beide lijsten bewerken");
                    _playedList.Add(SelectedSong);
                    _toPlayList.Remove(SelectedSong);
                    //test non shuffling
                    //Console.WriteLine("played added song: " + _playedList.Last().SongName + " / toplay next song: " + _toPlayList.First().SongName);
                    Console.WriteLine("played added song: " + _playedList.Last().SongName);
                    if (!IsShuffle)
                    {
                        Console.WriteLine("geen shuffle playlist pos gezet");
                        //PlayListPos += 1;
                        PlayListPos = 0;
                    }
                    else
                    {
                        Console.WriteLine("shuffle + playlistpos");
                        Random rnd = new Random(); 
                        PlayListPos = rnd.Next(0,_toPlayList.Count-1);
                        Console.WriteLine("random: " + SelectedSong.ArtistID + " - " + SelectedSong.SongName);
                    }
                }
                else if(_positionInPrev >= 0) //wanneer we aan het afspelen zijn van de previous playlist, de positi met 1 verlagen (speelt volgend lied af) 
                {
                    Console.WriteLine("op next geduwd terwijl we oud liedje aan het afspelen zijn + playlistpos");
                    _positionInPrev -= 1;
                    PlayListPos = ((_playedList.Count-1) - _positionInPrev);
                }
            }
            else
                stop();
        }

        private bool canNext()
        {
            if (_toPlayList != null)
            {
                if (PlayListPos < _toPlayList.Count)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public ICommand ShuffleCommand
        {
            get { Console.WriteLine("pressed shuffle"); return new RelayCommand(Shuffle); }
        }
        public void Shuffle()
        {
            IsShuffle = (IsShuffle == false) ? true : false;
        }
        #endregion ICOMMANDS

        void PlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateTime();
        }
        private void updateTime()
        {
            TimeSpan position = _musicPlayer.Position;
            TimeSpan length = _musicPlayer.Length;
            //zowel checken op playbackstate als position & length. Playbackstate is een event van de mediaplayer zelf, die normaal altijd zou moeten werken, maar gebeurd niet altijd
            //alleen position & length gebruiken is ook geen oplossing, want in sommige gevallen stopt de position enkele nanoseconden te vroeg en bijgevolg < length
            //vandaar dat we ook gebruik maken van total seconds en afronden
            if ((_musicPlayer.PlaybackState == PlaybackState.Stopped)||(position >= length)) {
                PlayTimer.Enabled = false;
                Console.WriteLine("next command oproepen omdat playback is gestopt of lied gdn is");
                nextCommand();
            }               

            //TimePlayed/Total = String.Format(@"{0:mm\:ss} / {1:mm\:ss}", position, length);
            TimePlayed = String.Format(@"{0:h\:mm\:ss}", position);
            TimeTotal = String.Format(@"{0:h\:mm\:ss}", length);

            if (!_stopSliderUpdate && length != TimeSpan.Zero && position != TimeSpan.Zero)
            {
                double perc = position.TotalMilliseconds / length.TotalMilliseconds * 100;
                SliderTime = perc;
            }
        }



        //zou worden gebruikt eens de trackbar ook dient om de juiste positie in het liedje in te geven
        private void changePosition()
        {
            //double perc = trackBar1.Value / (double)trackBar1.Maximum;
            //TimeSpan position = TimeSpan.FromMilliseconds(_musicPlayer.Length.TotalMilliseconds * perc);
            TimeSpan position = TimeSpan.FromMilliseconds(_musicPlayer.Length.TotalMilliseconds * 0);
            _musicPlayer.Position = position;
        }
    }

}
