using GalaSoft.MvvmLight.CommandWpf;
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
       
        
        
        private ApplicationVM _appvm;
        public BrowseArtistsVM()
        {

        }


    }
}
