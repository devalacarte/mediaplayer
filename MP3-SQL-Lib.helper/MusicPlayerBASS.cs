using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace MP3_SQL_Lib.helper
{
    public static class BASSHelper
    {
       /* private int _stream = 0;
        private string _fileName = String.Empty;
        private int _tickCounter = 0;
        private DSPPROC _myDSPAddr = null;
        private SYNCPROC _sync = null;
        private int _deviceLatencyMS = 0; // device latency in milliseconds
        private int _deviceLatencyBytes = 0; // device latency in bytes
        private int _updateInterval = 50; // 50ms
        private Un4seen.Bass.BASSTimer _updateTimer = null;*/



        public static BASS_DEVICEINFO[] GetDeviceInfoArray()
        {
            BASS_DEVICEINFO[] devices;
            //int count = Bass.BASS_GetDeviceCount();
            devices = Bass.BASS_GetDeviceInfos();
            Console.WriteLine("listing all available sound devices");
            int pos = 0;
            foreach (BASS_DEVICEINFO info in devices)
	        {
		        Console.WriteLine("pos: {0} + name:{1} + type:{2}", pos, info.name, info.type);
                pos += 1;
	        }
            return devices;
        }

        public static List<BASS_DEVICEINFO> GetDeviceInfoList()
        {
            List<BASS_DEVICEINFO> list = new List<BASS_DEVICEINFO>();
            foreach (BASS_DEVICEINFO dev in GetDeviceInfoArray())
            {
                list.Add(dev);
            }
            return list;
        }

        public static BASS_DEVICEINFO GetDeviceInfoByName(string name)
        {
            foreach (BASS_DEVICEINFO dev in GetDeviceInfoArray())
            {
                if (String.Equals(dev.name, name))
                    return dev;
            }
            return null;
        }

        public static int GetDevicePosInDeviceList(string name)
        {
            List<BASS_DEVICEINFO> list = GetDeviceInfoList();
            foreach (BASS_DEVICEINFO dev in list)
            {
                if (String.Equals(dev.name, name))
                    return list.IndexOf(dev);
            }
            return 0;
        }

        public static int GetDevicePosInDeviceList(BASS_DEVICEINFO dev)
        {
            return GetDevicePosInDeviceList(dev.name);
        }

        public static BASS_DEVICEINFO GetDeviceSystemDefault()
        {
            foreach (BASS_DEVICEINFO dev in GetDeviceInfoArray())
            {
                if (dev.IsDefault)
                    return dev;
            }
            return null;
        }





/*

        public void InitMusicPLayer(int soundoutput)
        {
            if (Bass.BASS_Init(soundoutput, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY, System.IntPtr.Zero))
			{
				BASS_INFO info = new BASS_INFO();
				Bass.BASS_GetInfo( info );
				Console.WriteLine("MusicPlayerBass Init getinfo: {0}", info.ToString() );
				_deviceLatencyMS = info.latency;
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 200);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 20);
			}
			else
                Console.WriteLine("MusicPlayerBass Init: Bass_Init error!");

			// create a secure timer
			_updateTimer = new Un4seen.Bass.BASSTimer(_updateInterval);
			_updateTimer.Tick += new EventHandler(timerUpdate_Tick);

			_sync = new SYNCPROC(EndPosition);
        }

        public void EndPosition(int handle, int channel, int data, IntPtr user)
        {
            Bass.BASS_ChannelStop(channel);
        }
 
 */

    }
}
