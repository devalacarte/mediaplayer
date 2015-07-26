using GalaSoft.MvvmLight.Messaging;
using MP3_SQL_Lib.model;
using MP3_SQL_Lib.MVVMMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MP3_SQL_Lib.View
{
    /// <summary>
    /// Interaction logic for PlayBar.xaml
    /// </summary>
    public partial class PlayBar : UserControl
    {
        private Storyboard _sbSlideAnimation;
        public PlayBar()
        {
            //Messenger.Default.Register<NotificationMessage<Song>>(this, (message) => NotificationMessageHandlerSong(message));
            //Messenger.Default.Register<NotificationMessage>(this, (message) => NotificationMessageHandler(message));
            //Unloaded += PlayBarView_Unloaded;
            InitializeComponent();
            //_sbSlideAnimation = (Storyboard)this.FindResource("SlideAnimation");
        }


        private void BeginStoryBoard(Storyboard sb)
        {
            
            sb.Begin(this, true);
        }

        private void SetStoryBoardActivity(Storyboard sb, bool play)
        {
            if (play)
            {
                sb.Resume(this);
            }
            else
            {
                sb.Pause(this);
            }
        }

        private void NotificationMessageHandler(NotificationMessage m)
        {
            switch (m.Notification)
            {
                case Messages.MUSIC_PAUSE:
                    SetStoryBoardActivity(_sbSlideAnimation, false);
                    break;
                case Messages.MUSIC_RESUME:
                    SetStoryBoardActivity(_sbSlideAnimation, true);
                    break;
                default:
                    break;
            }
        }
        private void NotificationMessageHandlerSong(NotificationMessage<Song> msg)
        {
            Song s = msg.Content;
            // Checks the actual content of the message.
            switch (msg.Notification)
            {
                case Messages.MUSIC_NEW_SONG:
                    //MessageBox.Show(s.SongName);
                    _sbSlideAnimation.Begin(this, true);
                    break;
                default:
                    break;
            }
        }
        void PlayBarView_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }
    }
}
