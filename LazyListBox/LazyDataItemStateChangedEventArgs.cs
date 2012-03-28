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
  /// Event args for the <see cref="ILazyDataItem.CurrentStateChanged"/> event
  /// </summary>
  public class LazyDataItemStateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// The old state of the item
    /// </summary>
    public LazyDataLoadState OldState { get; private set; }

    /// <summary>
    /// The new state of the item
    /// </summary>
    public LazyDataLoadState NewState { get; private set; }

    /// <summary>
    /// Create a new instance of the args
    /// </summary>
    /// <param name="oldState">The old state of the item</param>
    /// <param name="newState">The new state of the item</param>
    public LazyDataItemStateChangedEventArgs(LazyDataLoadState oldState, LazyDataLoadState newState)
    {
      OldState = oldState;
      NewState = newState;
    }
  }
}
