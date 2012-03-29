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
using System.Text;
using System.Windows;
using meituan.ViewModel;
using Microsoft.Phone.Shell;
namespace meituan
{
    /// <summary>
    /// Description for LazyPage.
    /// </summary>
    public partial class LazyPage : PhoneApplicationPage
    {
        protected City myCity;
        protected XElement xml;
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
            var appStoreage = IsolatedStorageFile.GetUserStoreForApplication();
            var filename = string.Format("deals_{0}.xml", cityid);
          
            if (appStoreage.FileExists(filename))
            {

                using (var file = appStoreage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                {

                    StreamReader sr = new StreamReader(file);


                    xml = XElement.Load(sr);
                    myList.ItemsSource = praseXML(xml);

                    this.cacheTime.Text = "[缓存时间:" + appStoreage.GetCreationTime(filename).ToString() + "]";
                }

                this.PageTitle.Text = myCity.Name + "团购";
            }
            else
            {
                WebClient client = new WebClient();
                Uri uri = new Uri(String.Format("http://www.meituan.com/api/v2/{0}/deals", cityid), UriKind.Absolute);
                client.OpenReadAsync(uri, cityid);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            }
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            xml = XElement.Load(e.Result);
            var appStoreage = IsolatedStorageFile.GetUserStoreForApplication();
            var filename = string.Format("deals_{0}.xml", e.UserState);
            appStoreage.DeleteFile(filename);
            using (var file = appStoreage.OpenFile(filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    Stream s = e.Result as Stream;
                    s.Position = 0;
                    Byte[] info = new Byte[s.Length];
                    s.Read(info, 0, (int)s.Length);
                    sw.BaseStream.Write(info, 0, info.Length);
                    sw.Flush();
                    sw.Close();
                }
                file.Close();

            }
            myList.ItemsSource = praseXML(xml);
            this.PageTitle.Text = myCity.Name + "团购[实时]";
            this.cacheTime.Text = "[数据已被缓存:" + appStoreage.GetCreationTime(filename).ToString() + "]";
        }
        private List<Deal> praseXML(XElement xml)
        {
            List<Deal> list = new List<Deal>();
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
            return list;
        }

        private void myList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (myList.SelectedItem!=null)
            {
                Deal deal = myList.SelectedItem as Deal;
                PhoneApplicationService.Current.State["deal"] = deal;
                NavigationService.Navigate(new Uri("/Detail.xaml", UriKind.RelativeOrAbsolute));
            }



        }
    

    }
}