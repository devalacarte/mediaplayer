using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.helper
{
    public static class CSAudio
    {
        public static List<MMDevice> GetSoundDevices()
        {
            List<MMDevice> devices = new List<MMDevice>();
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        devices.Add(device);
                    }
                }
            }
            return devices;
        }

        public static MMDevice GetDefaultSoundDevice()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
        }
    }

    public class MusicPlayerCSCore : Component
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped
        {
            add
            {
                if (_soundOut != null)
                    _soundOut.Stopped += value;
            }  
            remove
            {
                if (_soundOut != null)
                    _soundOut.Stopped -= value;
            }
        }

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                    return _soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
            }
            set
            {
                if (_waveSource != null)
                    _waveSource.SetPosition(value);
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                    return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                }
            }
        }

        public void Open(string filename, MMDevice device)
        {
            CleanupPlayback();

            _waveSource =
                CodecFactory.Instance.GetCodec(filename)
                    .ToSampleSource()
                    .ToMono()
                    .ToWaveSource();
            _soundOut = new WasapiOut() {Latency = 100, Device = device};
            _soundOut.Initialize(_waveSource);
        }

        public void Play()
        {
            if (_soundOut != null)
                _soundOut.Play();
        }

        public void Pause()
        {
            if (_soundOut != null)
                _soundOut.Pause();
        }

        public void Stop()
        {
            if (_soundOut != null)
                _soundOut.Stop();
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                try
                {
                    _soundOut.Dispose();
                    _soundOut = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mediaplayer: Dispose soundout: " + ex.Message);
                    Console.WriteLine("vervolg stacktrace: " + ex.StackTrace);
                }
            }
            if (_waveSource != null)
            {
                try
                {
                    _waveSource.Dispose();
                    _waveSource = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mediaplayer: Dispose wavesource: " + ex.Message);
                    Console.WriteLine("vervolg stacktrace: " + ex.StackTrace);                    
                } 
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}
