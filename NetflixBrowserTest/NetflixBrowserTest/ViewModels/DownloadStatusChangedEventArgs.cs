using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NetflixBrowserTest.ViewModels
{
  /// <summary>
  /// Arguments for the DownloadStatusChanged event
  /// </summary>
  public class DownloadStatusChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Construct a new instance
    /// </summary>
    /// <param name="message">The message to show to the user</param>
    /// <param name="isComplete">Whether download is complete or not</param>
    internal DownloadStatusChangedEventArgs(string message, bool isComplete)
    {
      Message = message;
      IsComplete = isComplete;
    }

    /// <summary>
    /// The message to show to the user
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Whether the download is complete or not
    /// </summary>
    public bool IsComplete { get; private set; }
  }
}
