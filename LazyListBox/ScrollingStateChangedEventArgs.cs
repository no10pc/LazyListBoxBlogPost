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

namespace LazyListBox
{
  /// <summary>
  /// Event args for the <see cref="LazyListBox.ScrollingStateChanged"/> event
  /// </summary>
  public class ScrollingStateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Old scrolling value
    /// </summary>
    public bool OldValue { get; private set; }

    /// <summary>
    /// New scrolling value
    /// </summary>
    public bool NewValue { get; private set; }

    /// <summary>
    /// Create a new instance of the event args
    /// </summary>
    /// <param name="oldValue">Old scrolling value</param>
    /// <param name="newValue">New scrolling value</param>
    public ScrollingStateChangedEventArgs(bool oldValue, bool newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}
