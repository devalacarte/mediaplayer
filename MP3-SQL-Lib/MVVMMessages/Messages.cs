using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.MVVMMessages
{
    public static class Messages
    {
        public const string MUSIC_NEW_SONG = "MessageForMVVMLightMessengerNewSongLoaded";
        public const string MUSIC_PAUSE = "MessageForMVVMLightMessengerMusicPausedStopTRackbar";
        public const string MUSIC_RESUME = "MessageForMVVMLightMessengerMusicResumedStartTRackbar";
        public const string MUSIC_STOPPED = "MessageForMVVMLightMessengerMusicStoppedForWhateverReason";
        public const string MUSIC_PLAY_ARTIST = "MessageForMVVMLightMessengerMusicPlayArtistFromArtistID";
    }
}
