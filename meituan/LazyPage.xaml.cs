using Microsoft.Phone.Controls;
using LazyListBox;
using UIExtensionMethods;
namespace meituan
{
    /// <summary>
    /// Description for LazyPage.
    /// </summary>
    public partial class LazyPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the LazyPage class.
        /// </summary>
        public LazyPage()
        {
            InitializeComponent();
            
            PageTitle.ManipulationCompleted += delegate { ExtensionMethods.PrintDescendentsTree(myList); };
        }
        private void myList_ScrollingStateChanged(object sender, ScrollingStateChangedEventArgs e)
        {
            //if (e.NewValue)
            //    textblock.Foreground = new SolidColorBrush(Colors.Red);
            //else
            //    textblock.ClearValue(TextBlock.ForegroundProperty);
        }
    }
}