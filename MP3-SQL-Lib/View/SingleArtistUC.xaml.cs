using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MP3_SQL_Lib.model;
using MP3_SQL_Lib.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MP3_SQL_Lib.View
{
    /// <summary>
    /// Interaction logic for SingleArtist.xaml
    /// </summary>
    public partial class SingleArtistUC : UserControl
    {

        public bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }

        public SingleArtistUC()
        {
            InitializeComponent();
        }

        public Artist Artist
        {
            get { return (Artist)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ArtistName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register("Artist", typeof(Artist), typeof(SingleArtistUC));
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ArtistName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SingleArtistUC));

        public string ImageURL
        {
            get { return (string)GetValue(ImageURLProperty); }
            set { SetValue(ImageURLProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ArtistName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageURLProperty =
            DependencyProperty.Register("ImageURL", typeof(string), typeof(SingleArtistUC));


        public byte[] ImageByte
        {
            get { return (byte[])GetValue(ImageByteProperty); }
            set { SetValue(ImageByteProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ArtistName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageByteProperty =
            DependencyProperty.Register("ImageByte", typeof(byte[]), typeof(SingleArtistUC));


        public ICommand CMDPlayArtist
        {
            get { return new RelayCommand(Play); }
        }
        public void Play()
        {
            Messenger.Default.Send(new NotificationMessage<Artist>(this.Artist, MVVMMessages.Messages.ARTIST_PLAY));
        }

        void SetValue(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
