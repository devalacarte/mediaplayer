using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3_SQL_Lib.ViewModel
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

    }
}
