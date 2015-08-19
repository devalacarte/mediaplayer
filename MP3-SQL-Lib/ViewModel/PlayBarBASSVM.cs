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
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace MP3_SQL_Lib.ViewModel
{
    public class PlayBarBASSVM : ObservableObject
    {
        private int _stream = 0;
        /// <summary>
        /// used to send event when streams is done
        /// </summary>
        private SYNCPROC _mySync;
        private Un4seen.Bass.BASSTimer _updateTimer = null;
        private int _updateTimerInterval = 50;
        private bool _bassInit;
        /// <summary>
        /// Is output device initialized
        /// </summary>
        public bool BassInit
        {
            get { return _bassInit; }
            set { _bassInit = value; }
        }
        

        private List<BASS_DEVICEINFO> _devices;
        /// <summary>
        /// Bevat de geluidskaarten
        /// </summary>
        public List<BASS_DEVICEINFO> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged("Devices"); }
        }
        #region properties
        private BASS_DEVICEINFO _selectedSoundCard;
        /// <summary>
        /// Geluidskaart waar muziek op wordt afgespeeld
        /// </summary>
        public BASS_DEVICEINFO SelectedSoundCard
        {
            get { return _selectedSoundCard; }
            set { _selectedSoundCard = value; OnPropertyChanged("SelectedSoundCard"); SelectedSoundCardChanged();}
        }













        private bool _stopSliderUpdate;
        /// <summary>
        /// played / skipped songs (used when going back by using previous)
        /// </summary>
        private ObservableCollection<Song> _playedList;
        private ObservableCollection<Song> _toPlayList;
        private ObservableCollection<Song> _originalPlayList;
        /// <summary>
        /// Normal playlist
        /// </summary>
        public ObservableCollection<Song> PlayList
        {
            get { return _originalPlayList; }
            set { _originalPlayList = value; _toPlayList = value; _positionInPrev = -1; OnPropertyChanged("PlayList"); }
        }

        private int _playListPos = 0;
        /// <summary>
        /// when we use previous we want to save how many times we went back, and be able to use next to undo
        /// -1 means we're not playing any previous song. 0 last played song, 1 second last played ...
        /// use with playedsonglist[count-_playedlistpos]
        /// </summary>
        private int _positionInPrev = -1;
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
                    if (_positionInPrev == -1)
                    {
                        SelectedSong = _toPlayList[value];
                    }
                    else
                        SelectedSong = _playedList[value];
                    this.PlayStream();
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
            set
            {
                _selectedSong = value; OnPropertyChanged("SelectedSong");
                if (SelectedSong != null)
                {
                    if (System.IO.File.Exists(value.FilePath)) { /*Open()*/; BtnPlayEnabled = true; }
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
        /// <summary>
        /// min:0 = mute, max 100
        /// </summary>
        public int SliderVolume
        {
            get { _sldVolume = (Properties.Settings.Default.Volume >= 0) ? (Properties.Settings.Default.Volume) : ((int)Math.Ceiling(Bass.BASS_GetVolume()))*100; ; return _sldVolume; }
            set { _sldVolume = Properties.Settings.Default.Volume = value; Bass.BASS_SetVolume(_sldVolume / 100); OnPropertyChanged("SliderVolume"); Properties.Settings.Default.Save(); }
        }


        private double _sldTime = 0;
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

        private bool _isShuffle = false;
        public bool IsShuffle
        {
            get { return _isShuffle; }
            set { _isShuffle = value; OnPropertyChanged("Shuffle"); }
        }
        #endregion BindingProperties





























        public PlayBarBASSVM()
        {
            PlayList = new ObservableCollection<Song>();
            _playedList = new ObservableCollection<Song>();
            _mySync = new SYNCPROC(EndSync);
            //MusicPlayer = new MusicPlayerBASS();
            Devices = BASSHelper.GetDeviceInfoList();

            //initalizing BASS
            InitBASS();
            _updateTimer = new Un4seen.Bass.BASSTimer(_updateTimerInterval);
            _updateTimer.Tick += new EventHandler(timerUpdate_Tick);

            //PlayStream();
            SelectedSoundCard = Devices[1];
        }

        
        private void InitBASS()
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.SoundCardName))
            {
                SelectedSoundCard = BASSHelper.GetDeviceSystemDefault();
                Properties.Settings.Default.SoundCardName = SelectedSoundCard.name; Properties.Settings.Default.Save();
            }
            else
            {
                SelectedSoundCard = BASSHelper.GetDeviceInfoByName(Properties.Settings.Default.SoundCardName);
            }

            int dev = BASSHelper.GetDevicePosInDeviceList(SelectedSoundCard);
            BassInit = Bass.BASS_Init(dev, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 200);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 20);
        }
      
        
        public void CloseBass()
        {

            _updateTimer.Tick -= new EventHandler(timerUpdate_Tick);
            Bass.BASS_Stop();
            Bass.BASS_Free();
        }


        public void PlayStream()
        {
            if(BassInit == false)
            {
                Console.WriteLine("PlayBarBASSVM: PlayStream: Bass not initialized");
                return;
            }
            
            if(SelectedSong==null)
            {
                Console.WriteLine("PlayBarBASSVM: PlayStream: No selected song");
                return;
            }

            if(!System.IO.File.Exists(SelectedSong.FilePath))
            {
                Console.WriteLine("PlayBarBASSVM: PlayStream: Songpath doesn't exists - file not found");
                return;
            }

            //indien een vorig lied nog aan het spelen is, stream vrijmaken voor volgend lied
            if(Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
                Bass.BASS_StreamFree(_stream);

            _stream = Bass.BASS_StreamCreateFile(SelectedSong.FilePath, 0, 0, BASSFlag.BASS_DEFAULT); /* BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN */
            if (_stream != 0)
            {
                Bass.BASS_ChannelPlay(_stream, false); // play the channel
                Messenger.Default.Send(new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_NEW_SONG));
                _updateTimer.Start();
                //channelinfo
                BASS_CHANNELINFO info = new BASS_CHANNELINFO();
                Bass.BASS_ChannelGetInfo(_stream, info);
                //filetags (id3v1 v2 ...)
                TAG_INFO tagInfo = new TAG_INFO(SelectedSong.FilePath);
                if (BassTags.BASS_TAG_GetFromFile(_stream, tagInfo))
                {
                    Console.WriteLine("PlayBarBASSVM: PlayStream: Playing:{0} - {1}", tagInfo.albumartist, tagInfo.title);
                }
                this.BtnPauseEnabled = true; this.BtnStopEnabled = true; this.BtnPlayEnabled = false;

                //sends event when track is finished
                Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME,0, _mySync, IntPtr.Zero);
            }
            else
            {
                Console.WriteLine("PlayBarBASSVM: PlayStream: Stream error: {0}", Bass.BASS_ErrorGetCode());  // error
                return;
            }

        }

        public void StopStream()
        {
            _updateTimer.Stop();
            Bass.BASS_StreamFree(_stream);
        }
        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            this.nextCommand();
        }

        private void SelectedSoundCardChanged()
        {
            if (_stream == 0)
            {
                Console.WriteLine("PlayBarBASSVM: PlayStream: SelectedSoundCardChanged: stream is 0");
                return;
            }
            if (Bass.BASS_ChannelSetDevice(_stream, BASSHelper.GetDevicePosInDeviceList(SelectedSoundCard)))
            {
                Properties.Settings.Default.SoundCardName = SelectedSoundCard.name;
                Properties.Settings.Default.Save();
                Console.WriteLine("PlayBarBASSVM: PlayStream: SelectedSoundCardChanged to {0}", SelectedSoundCard.name);
            }
            else
                Console.WriteLine("PlayBarBASSVM: PlayStream: SelectedSoundCardChanged: error: {0}", Bass.BASS_ErrorGetCode());
        }



        private int _tickCounter = 0;
        private void timerUpdate_Tick(object sender, System.EventArgs e)
        {
            if ( Bass.BASS_ChannelIsActive(_stream) != BASSActive.BASS_ACTIVE_PLAYING )
            {
                _updateTimer.Stop();
                BtnPauseEnabled = false;
                BtnPlayEnabled = BtnStopEnabled = true;
                return;
            }

            _tickCounter++;
            long pos = Bass.BASS_ChannelGetPosition(_stream); // position in bytes
            long len = Bass.BASS_ChannelGetLength(_stream); // length in bytes

            if (_tickCounter == 5)
            {
                _tickCounter = 0;
                double totaltime = Bass.BASS_ChannelBytes2Seconds(_stream, len); // the total time length
                double elapsedtime = Bass.BASS_ChannelBytes2Seconds(_stream, pos); // the elapsed time length
                double remainingtime = totaltime - elapsedtime;
                this.TimePlayed = Utils.FixTimespan(elapsedtime, "MMSS");
                this.TimeTotal = Utils.FixTimespan(totaltime, "MMSS");
            }
        }









        #region ICommands
        public ICommand PlayCommand
        {
            get { return new RelayCommand(Play, canPlay); }
        }
        public void Play()
        {

            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PAUSED)
            { 
                Bass.BASS_ChannelPlay(_stream, false);
                BtnPauseEnabled = BtnStopEnabled = true;
                BtnPlayEnabled = false;
                Messenger.Default.Send(new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_NEW_SONG));
                //timer starten
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



        public ICommand PauseCommand
        {
            get { return new RelayCommand(pause, canPause); }
        }
        private void pause()
        {
            //bij afspelen pauseer, bij pause speel af
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(_stream);
                Messenger.Default.Send(new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_PAUSE));
                BtnPauseEnabled = false;
                BtnPlayEnabled = BtnStopEnabled = true;
            }
        }
        private bool canPause()
        {
            return BtnPauseEnabled;
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
                if (_positionInPrev == -1)
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
                        PlayListPos = rnd.Next(0, _toPlayList.Count - 1);
                        Console.WriteLine("random: " + SelectedSong.ArtistID + " - " + SelectedSong.SongName);
                    }
                }
                else if (_positionInPrev >= 0) //wanneer we aan het afspelen zijn van de previous playlist, de positi met 1 verlagen (speelt volgend lied af) 
                {
                    Console.WriteLine("op next geduwd terwijl we oud liedje aan het afspelen zijn + playlistpos");
                    _positionInPrev -= 1;
                    PlayListPos = ((_playedList.Count - 1) - _positionInPrev);
                }
            }
            //else
                //stop();
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


        public ICommand PrevCommand
        {
            get { Console.WriteLine("prev clicked"); return new RelayCommand(prevCommand); }
        }
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
                        PlayListPos = ((_playedList.Count - 1) - _positionInPrev); //ammount of tracks played - how many tracks we've already turned back -1 (list is index)
                    }
                }
            }
            else
            {
                Console.WriteLine("lied is al te lang aan het spelen, ga terug naar start + playliststart");
                PlayListPos = _playListPos; //herstart van beginpunt van lied
            }
        }


        public ICommand ShuffleCommand
        {
            get { Console.WriteLine("pressed shuffle"); return new RelayCommand(Shuffle); }
        }
        public void Shuffle()
        {
            IsShuffle = (IsShuffle == false) ? true : false;
        }
        #endregion ICommands
    }
}
