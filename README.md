# mediaplayer

I only learned to work with c# and wpf last year, and haven't used it for more than half a year since.
To get back into it before looking for a job, I wanted to work on my skills a bit.
As I don't really have much big projects to show for, I decided to make a media player in my free time.

At first this will just have to work as a basic media player
(see PlayBar)

As this program is still in a test phase I do not advice anyone to use it. It works, but all data is localized, you'll have to use your own mysql/mssql servers

# Working functions:
import music: (11/7 - 12/7)
+ Scan specific directories for MP3s (11/7 - 12/7)
+ Get mp3 tags for said MP3s (title, artist, song, albums, track, length, year, genre, ...) (11/7 - 12/7)
+ Import into database (11/7 - 12/7)

Media functions:
+ Play, Pause, Resume, stop (18/7 - 19/7)
+ set volume (18/7 - 19/7)
+ set position in track (not possible yet, it's read only) (18/7 - 19/7)
+ next, previous (start of song if more than >20% played) (18/7 - 19/7)
+ which audiodevice to play on? (list of available audio devices) (18/7 - 19/7)
+ now playing in title bar (26/7)
+ save last used audio device and volume in settings for next startup (18/7 - 19/7) (recheck for Bass.NET)
+ click on artist image -> play artist (18/08)
+ Shuffle (18/08)

Library functions
+ Load and actually play songs (dbugging and testing, can play every song loaded in database) (18/7 - 19/7)
+ Get all data from database (artists, albums, songs) (11/7 - 12/7)
+ create allartist overview (25/7 - 26/7)
+ use last.fm to get images of artists and save image to database if there is none (no need to download all the time) (25/7)
+ change normal wpf app to metro app (working on gui elements) (25/7 - 26/7)


unplanned work:
+ change CSCore audio (beta) to BASS audio library (18/08)


# TODO:
+ drag trackbar to play from x seconds
+ settings menu :for lastfm (api key & secret, user info), database info, audio device, default volume on startup
+ script to create database if none exists (26/7 sql done, implement in c#)
+ sql (create new table: artist id + library id)
+ search field + filters
+ gui change play buttons
+ icon for taskbar
+ mini next prev pause/play button for mini window on taskbar (see spotify)
+ click on artist name -> detail view (list every album + art + each song) + biography (+ social media)?
+ repeat
+ list all albums
+ download missing album arts + save in db
+ click on album art to play whole album
+ click on album name to list album details
+ favoritize artist/album/song and auto add to playlist
+ option to only load favorites
+ most played
+ create and Save playlist (either file or db)
+ possibility for more than one library / database (for example, library for party music, normal music, ...)
+ set track to listened on lastfm when played for more than 30 seconds




#When the music library is working as planned, other things to do: 
+ add equalizer
+ add music visuals
+ add music lyric
+ posibility to play youtubesongs?
+ create webapi to access database
+ stream to android app
+ add serverlistener (listen for commands)
+ create webbased api or android application to control mediaplayer