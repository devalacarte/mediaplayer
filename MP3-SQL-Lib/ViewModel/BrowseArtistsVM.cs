using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MP3_SQL_Lib.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.ViewModel
{
    public class BrowseArtistsVM:ObservableObject
    {
        public string Name
        {
            get { return "Browse Artists page"; }
        }


        private ObservableCollection<Artist> _artists;

        public ObservableCollection<Artist> Artists
        {
            get { return _artists; }
            set { _artists = value; OnPropertyChanged("Artists"); }
        }
        
        
        
        public BrowseArtistsVM(ObservableCollection<Artist> artists)
        {
            this.Artists = artists;
        }


        private RelayCommand _viewLoaded;
        public RelayCommand ViewLoaded { get { return _viewLoaded ?? (_viewLoaded = new RelayCommand(ControlLoaded)); } }
        private void ControlLoaded()
        { _appvm = App.Current.MainWindow.DataContext as ApplicationVM; }

        private RelayCommand<Artist> _playArtistCommand;
        public RelayCommand<Artist> PlayArtistCommand { get { return _playArtistCommand ?? (_playArtistCommand = new RelayCommand<Artist>((ar) => PlayArtistClicked(ar))); } }
        
        private RelayCommand<Artist> _viewArtistDetailCommand;
        public RelayCommand<Artist> ViewArtistDetailCommand { get { return _viewArtistDetailCommand ?? (_viewArtistDetailCommand = new RelayCommand<Artist>((ar) => ViewArtistDetailClicked(ar))); } }
        
        private ApplicationVM _appvm;
        public BrowseArtistsVM()
        {

        }

        private void PlayArtistClicked(Artist ar)
        {
            Messenger.Default.Send(new NotificationMessage<Artist>(ar, MVVMMessages.Messages.ARTIST_PLAY));
        }

        private void ViewArtistDetailClicked(Artist ar)
        {
            Messenger.Default.Send(new NotificationMessage<Artist>(ar, MVVMMessages.Messages.ARTIST_VIEW));
        }

    }
}
