using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using UIExtensionMethods;
using LazyListBox;

namespace DelayLoadListBoxItem
{
  public partial class LazyPage : PhoneApplicationPage
  {
    public LazyPage()
    {
      InitializeComponent();
      myList.ItemsSource = new VirtualizedDummyList(true);
      PageTitle.ManipulationCompleted += delegate { ExtensionMethods.PrintDescendentsTree(myList); };
    }

    /// <summary>
    /// Example of using the scroll-state changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void myList_ScrollingStateChanged(object sender, ScrollingStateChangedEventArgs e)
    {
      if (e.NewValue)
        textblock.Foreground = new SolidColorBrush(Colors.Red);
      else
        textblock.ClearValue(TextBlock.ForegroundProperty);
    }

    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
      State["scrollOffset"] = myList.GetVerticalScrollOffset();
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      object dummy;
      if (State.TryGetValue("scrollOffset", out dummy))
        myList.SetVerticalScrollOffset((double)dummy);
    }

    // Navigate away...
    private void NavigateSomewhere(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/RandomPage.xaml", UriKind.Relative));
    }
  }
}