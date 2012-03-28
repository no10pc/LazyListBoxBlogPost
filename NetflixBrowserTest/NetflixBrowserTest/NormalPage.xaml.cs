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
using NetflixBrowserTest.ViewModels;

namespace NetflixBrowserTest
{
  public partial class NormalPage : PhoneApplicationPage
  {
    // Constructor
    public NormalPage()
    {
      InitializeComponent();
    }

    // NOTE: The code in all three pages is identical.

    /// <summary>
    /// Handler for the DownloadStatusChanged event
    /// </summary>
    /// <param name="sender">The main ViewModel</param>
    /// <param name="e">Args</param>
    void ViewModel_DownloadStatusChanged(object sender, DownloadStatusChangedEventArgs e)
    {
      // This comes in on a background thread...
      Dispatcher.BeginInvoke(delegate
      {
        if (e.IsComplete)
        {
          // Stop loading animation, hide the text, and make the list fully opaque
          loadingAnimation.Stop();
          loadingText.Visibility = Visibility.Collapsed;
          MainListBox.Opacity = 1;
        }
        else
        {
          loadingText.Text = e.Message;
        }
      });
    }

    /// <summary>
    /// Handler for when page is navigated to. We start loading the viewmodel and update the UI
    /// </summary>
    /// <param name="e">Args</param>
    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      loadingAnimation.Begin();
      loadingText.Visibility = Visibility.Visible;
      MainListBox.Opacity = 0.5;
      MainListBox.ItemsSource = App.ViewModel;
      App.ViewModel.DownloadStatusChanged += ViewModel_DownloadStatusChanged;
      App.ViewModel.LoadData();
    }

    /// <summary>
    /// Handler for when page is navigated away from. We unload all the data and unhook the events
    /// </summary>
    /// <param name="e">Args</param>
    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
      MainListBox.ItemsSource = null;
      App.ViewModel.ClearData();
      App.ViewModel.DownloadStatusChanged -= ViewModel_DownloadStatusChanged;
    }
  }
}