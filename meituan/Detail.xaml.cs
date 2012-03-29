using Microsoft.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;
using meituan.Helper;
using System.Windows;
using meituan.Model;
using Microsoft.Phone.Shell;

namespace meituan
{
    /// <summary>
    /// Description for Detail.
    /// </summary>
    public partial class Detail : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the Detail class.
        /// </summary>
        public Detail()
        {
            InitializeComponent();

           
        }
      

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            Deal deal = PhoneApplicationService.Current.State["deal"] as Deal;
            
           
        }


    }
}