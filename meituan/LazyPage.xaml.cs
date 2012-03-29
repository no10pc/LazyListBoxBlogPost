using Microsoft.Phone.Controls;
using LazyListBox;
using UIExtensionMethods;
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;
using meituan.Model;
using System;
using meituan.Helper;
using GalaSoft.MvvmLight.Messaging;
using System.IO.IsolatedStorage;
using System.IO;
namespace meituan
{
    /// <summary>
    /// Description for LazyPage.
    /// </summary>
    public partial class LazyPage : PhoneApplicationPage
    {
        protected City myCity;
        /// <summary>
        /// Initializes a new instance of the LazyPage class.
        /// </summary>
        public LazyPage()
        {
            InitializeComponent();
        
            PageTitle.ManipulationCompleted += delegate { ExtensionMethods.PrintDescendentsTree(myList); };

        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            myList.ItemsSource = null;
            base.OnNavigatedFrom(e);
        }

        private void myList_ScrollingStateChanged(object sender, ScrollingStateChangedEventArgs e)
        {
            //if (e.NewValue)
            //    textblock.Foreground = new SolidColorBrush(Colors.Red);
            //else
            //    textblock.ClearValue(TextBlock.ForegroundProperty);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ExecuteloadData();
            base.OnNavigatedTo(e);
        }

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
            myCity = new City() { Py = _cityid.Split('|')[0], Name = _cityid.Split('|')[1] };
            doDealList(myCity.Py);
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
            myList.ItemsSource = list;
            this.PageTitle.Text= myCity.Name+"团购";
        }

    }
}