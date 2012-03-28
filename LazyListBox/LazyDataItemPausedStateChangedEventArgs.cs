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
  /// Arguments for the <see cref="ILazyDataItem.PauseStateChanged"/> event
  /// </summary>
  public class LazyDataItemPausedStateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Whether the item is paused or not
    /// </summary>
    public bool IsPaused { get; private set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="isPaused">The paused state of the item</param>
    public LazyDataItemPausedStateChangedEventArgs(bool isPaused)
    {
      IsPaused = isPaused;
    }
  }
}
