# mediaplayer

I only learned to work with c# and wpf last year, and haven't used it for more than half a year since.
To get back into it before looking for a job, I wanted to work on my skills a bit.
As I don't really have much big projects to show for, I decided to make a media player in my free time.

At first this will just have to work as a basic media player
(see PlayBar)

As this program is still in a test phase I do not advice anyone to use it. It works, but all data is localized, you'll have to use your own mysql/mssql servers

# Working functions:
import music:
+ Scan specific directories for MP3s
+ Get mp3 tags for said MP3s (title, artist, song, albums, track, length, year, genre, ...)
+ Import into database

Media functions:
+ Play, Pause, Resume, stop
+ set volume 
+ set position in track (not possible yet, it's read only)
+ next, previous (start of song if more than >20% played)
+ which audiodevice to play on? (list of available audio devices)
+ now playing in title bar

Library functions
+ Load and actually play songs (dbugging and testing, can play every song loaded in database)
+ Get all data from database (artists, albums, songs)
+ create allartist overview
+ use last.fm to get images of artists and save image to database if there is none (no need to download all the time)
+ change normal wpf app to metro app (working on gui elements)




# TODO:
+ gui change play buttons
+ icon for taskbar
+ mini next prev pause/play button for mini window on taskbar (see spotify)
+ click on artist image -> play artist
+ click on artist name -> detail view (list every album + art + each song) + biography (+ social media)?
+ Shuffle / repeat
+ list all albums
+ download missing album arts + save in db
+ click on album art to play whole album
+ click on album name to list album details
+ create and Save playlist (either file or db)
+ possibility for more than one library / database (for example, library for party music, normal music, ...)
+ set track to listened on lastfm when played for more than 30 seconds

When the music library is working as planned, other things to do
+ add serverlistener (listen for commands)
+ create webbased api or android application to control mediaplayer

When remote is finished
+ add equalizer
+ add music visuals
+ add music lyric
+ posibility to play youtubesongs?
+ create webapi to access database
+ stream to android app
