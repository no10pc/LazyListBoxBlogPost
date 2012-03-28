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

namespace UIExtensionMethods
{
  /// <summary>
  /// Interface for listboxes that want to know when their offset has changed
  /// </summary>
  /// <remarks>
  /// This is really a hack for supporting re-creation of LazyListBox along with 
  /// saved scroll offsets...
  /// </remarks>
  public interface ISupportOffsetChanges
  {
    /// <summary>
    /// The horizontal offset has been changed
    /// </summary>
    /// <param name="offset">The new offset</param>
    void HorizontalOffsetChanged(double offset);

    /// <summary>
    /// The vertical offset has been changed
    /// </summary>
    /// <param name="offset">The new offset</param>
    void VerticalOffsetChanged(double offset);
  }
}
