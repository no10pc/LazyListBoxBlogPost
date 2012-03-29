using GalaSoft.MvvmLight;
using System.Collections.Generic;
using meituan.Model;
using System.Xml.Linq;
using System.Net;
using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using meituan.Helper;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.IO;

namespace meituan.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }
        public const string MyPropertyPropertyName = "cityList";

        private List<City> _citylist;

        /// <summary>
        /// Sets and gets the MyProperty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<City> cityList
        {
            get
            {
                return _citylist;
            }

            set
            {
                if (_citylist == value)
                {
                    return;
                }

                _citylist = value;
                RaisePropertyChanged(MyPropertyPropertyName);
            }
        }



      
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                    gotoPage = new RelayCommand<City>((x) => ExecutegotoPage(x));
                    WelcomeTitle = item.Title;
                    cityList = new DataItem().getCityList();

                });
        }
       


        public RelayCommand<City> gotoPage { get; private set; }

        public object ExecutegotoPage(City _city)
        {
            var msg = new PageMessage() { city = _city };
            var appStoreage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = appStoreage.OpenFile("city.txt", FileMode.Create, FileAccess.ReadWrite))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(_city.Py+"|"+_city.Name);
                }
            }
            var rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(new System.Uri("/LazyPage.xaml", System.UriKind.Relative));
            return null;
        }


        private void sendMessage(string _cityid)
        {
           
        }



        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }

}