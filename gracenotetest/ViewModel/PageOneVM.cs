using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gracenotetest.ViewModel
{
    class PageOneVM : ObservableObject, IPage
    {

        public string Name
        {

            get { return "First page"; }

        }

        private string _demo = "demo.";
        public string Demo
        {

            get { return _demo; }
            set { _demo = value; OnPropertyChanged("Demo"); }

        }

        private string _clientID = "4178944-162EC87654B09D3D4132ADDFAAD468DD";
        private string _clientTag = "162EC87654B09D3D4132ADDFAAD468DD";

    }
}
