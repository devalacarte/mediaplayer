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
    public class PlayBarVM : ObservableObject, IPage
    {
        #region Properties
        
        /// <summary>
        /// Functies voor het eigenlijke afspelen van de muziek
        /// </summary>
        private readonly MusicPlayer _musicPlayer = new MusicPlayer();
        private bool _stopSliderUpdate;
        /// <summary>
        /// Bevat de geluidskaarten
        /// </summary>
        private ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();
        public ObservableCollection<MMDevice> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged("Devices"); }
        }
        /// <summary>
        /// Geluidskaart waar muziek op wordt afgespeeld
        /// </summary>
        private MMDevice _selectedSoundCard;
        public MMDevice SelectedSoundCard
        {
            get { return _selectedSoundCard; }
            set { _selectedSoundCard = value; OnPropertyChanged("SelectedSoundCard");}
        }

        private ObservableCollection<Song> _playList;
        public ObservableCollection<Song> PlayList
        {
            get { return _playList; }
            set { _playList = value; OnPropertyChanged("PlayList"); }
        }

        private int _playListPos=0;
        public int PlayListPos
        {
            get { return _playListPos; }
            set
            {
                if ((value >= 0) && (value <= PlayList.Count))
                {
                    _playListPos = value; OnPropertyChanged("PlayListPos");
                    SelectedSong = PlayList[value];
                    Play();
                }
            } 
        }
        
        
        private Song _selectedSong;
        public Song SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                OnPropertyChanged("SelectedSong");
                if (SelectedSong != null)
                {
                    if (System.IO.File.Exists(value.FilePath)) { Open(); BtnPlayEnabled = true; Properties.Settings.Default.LastPlayedSong = value; Properties.Settings.Default.Save(); } else { nextCommand(); }
                }
                
            }
        }
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
        public static Timer PlayTimer;

        private bool _shuffle = false;
        public bool Shuffle
        {
            get { return _shuffle = false; }
            set { _shuffle = value; OnPropertyChanged("Shuffle"); }
        }

        public string Name
        {
            get { return "PlayBar"; }
        }


        #endregion Properties























        public PlayBarVM()
        {
            _musicPlayer.PlaybackStopped += (s, args) =>
            {
                BtnPlayEnabled = BtnStopEnabled = BtnPauseEnabled = false;
            };

            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (
                    var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        Devices.Add(device);
                    }
                }


                //Devices[Properties.Settings.Default.SoundCard]; //moet van settings komen, indien niet opgeslagen neem de eerste soundcard
                SelectedSoundCard = (Properties.Settings.Default.SoundCard == -1) ? (Devices[0]) : (Devices[Properties.Settings.Default.SoundCard]);
                PlayList = new ObservableCollection<Song>();

                PlayTimer = new Timer();
                PlayTimer.Interval = 1000;
                PlayTimer.Elapsed += PlayTimer_Elapsed;
            }
        }

        void PlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            updateTime();
        }

        /*public ICommand OpenCommand
        {
            get { return new RelayCommand(Open); }
        }*/
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
                _musicPlayer.Position = _musicPlayer.Length - new TimeSpan(0, 0, 10); //diende om het testen van next song (speelde enkele laatste 10 sec van liedje af)
                BtnPlayEnabled = true;
                BtnPauseEnabled = BtnStopEnabled = false;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
                Messenger.Default.Send (new NotificationMessage<Song>(SelectedSong, MVVMMessages.Messages.MUSIC_NEW_SONG)); //diende voor een slider animation op de trackbar, omdat timer niet werkte ma fixed
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
            get { return new RelayCommand(prevCommand); }
        }
        /// <summary>
        /// indien den tracker op 20% sta, naar het begin van het lied, anders vorig lied
        /// </summary>
        private void prevCommand()
        {
            if (PlayList == null)
                return;
            if(SliderTime < 20)
            {
                if (PlayListPos > 0)
                    PlayListPos -= 1;
            }
            else
            {
                PlayListPos = _playListPos;
            }
            
        }
        public ICommand NextCommand
        {
            get { return new RelayCommand(nextCommand, canNext); }
        }
        private void nextCommand()
        {
            if (PlayList == null)
                return;

            if (!Shuffle)
            {
                if (PlayListPos < PlayList.Count)
                    PlayListPos += 1;
                else
                    stop();
            }
            
        }
        private bool canNext()
        {
            if (PlayList != null)
            {
                if (PlayListPos < PlayList.Count)
                    return true;
                else
                    return false;
            }
            else
                return false;
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
                nextCommand();
            }
           // }
               

            //TimePlayed/Total = String.Format(@"{0:mm\:ss} / {1:mm\:ss}", position, length);
            TimePlayed = String.Format(@"{0:h\:mm\:ss}", position);
            TimeTotal = String.Format(@"{0:h\:mm\:ss}", length);

            if (!_stopSliderUpdate && length != TimeSpan.Zero && position != TimeSpan.Zero)
            {
                double perc = position.TotalMilliseconds / length.TotalMilliseconds * 100;
                SliderTime = perc;
            }
        }


        

        private void changePosition()
        {
            //double perc = trackBar1.Value / (double)trackBar1.Maximum;
            //TimeSpan position = TimeSpan.FromMilliseconds(_musicPlayer.Length.TotalMilliseconds * perc);
            TimeSpan position = TimeSpan.FromMilliseconds(_musicPlayer.Length.TotalMilliseconds * 0);
            _musicPlayer.Position = position;
        }
    }

}
