using GalaSoft.MvvmLight;
using System.Collections.Generic;
using meituan.Model;
using System.Xml.Linq;
using System.Net;
using System;

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

                    WelcomeTitle = item.Title;

                   
                   // cityList =  new DataItem().getCityList();


                    doDealList("beijing");


                });
        }
        public void doDealList(string cityid)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri(String.Format("http://www.meituan.com/api/v2/{0}/deals", cityid), UriKind.Absolute);
            client.DownloadStringAsync(uri);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            List<Deal> list = new List<Deal>();

            XElement xml = XElement.Parse(e.Result);

            foreach (XElement element1 in xml.Element("deals").Elements("data"))
            {
                foreach (XElement element2 in element1.Elements("deal"))
                {

                    list.Add(new Deal()
                    {
                        City_Name = element2.Element("city_name").Value,
                        Deal_Id = element2.Element("deal_id").Value,
                        Deal_img = element2.Element("deal_img").Value.Replace("275.168", "150.90"),
                        Deal_Price = element2.Element("price").Value,
                        Deal_title = element2.Element("deal_title").Value,
                        Deal_Url = element2.Element("deal_url").Value,
                        Value = "￥" + element2.Element("value").Value
                    });
                }
            }

            dealList = list;
        }
        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
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



        private List<Deal> _deallist;

        /// <summary>
        /// Sets and gets the MyProperty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<Deal> dealList
        {
            get
            {
                return _deallist;
            }

            set
            {
                if (_deallist == value)
                {
                    return;
                }

                _deallist = value;
                RaisePropertyChanged("dealList");
            }
        }
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}