

using GalaSoft.MvvmLight;
using System.Collections.Generic;
using meituan.Model;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using meituan.Helper;
using System.Net;
using System;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Input;
namespace meituan.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class LazyViewModel : ViewModelBase
    {
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

  
        private string _welcomeTitle = string.Empty;


        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {

                    Messenger.Default.Send<DialogMessage>(new DialogMessage("保存成功", result => { }));
                        
                });
                    
            }
        }

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

        private readonly IDataService _dataService;

        public LazyViewModel()
        {
            loadData = new RelayCommand(() => ExecuteloadData());
            WelcomeTitle = "Loading...";
        }






        public RelayCommand loadData { get; private set; }
        private object ExecuteloadData()
        {
            string _cityid = string.Empty;
            var appStoreage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = appStoreage.OpenFile("city.txt", FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(file))
                {
                    _cityid = sr.ReadToEnd();
                }
               
            }

            doDealList(_cityid.Split('|')[0]);
            return null;
        }

        public void doDealList(string cityid)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri(String.Format("http://www.meituan.com/api/v2/{0}/deals", cityid), UriKind.Absolute);
            client.OpenReadAsync(uri);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            List<Deal> list = new List<Deal>();

            XElement xml = XElement.Load(e.Result);

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
            WelcomeTitle = "异步加载";
        }


    }
}