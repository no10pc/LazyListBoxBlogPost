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
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using LazyListBox;

namespace DelayLoadListBoxItem
{
  /// <summary>
  /// This is a dummy data class that has various types of data in it, designed to show
  /// how to build a lazy data item
  /// </summary>
  /// <remarks>
  /// This class is more complicated than an ordinary class for two reasons:
  /// 1. It is artificially trying to illustrate how to deal with all sorts of different
  ///    data characteristics (slow/fast, large/small) which ordinarily you wouldn't 
  ///    need to do in most cases
  /// 2. It artificially supports both a "lazy" and a "non-lazy" operation in order
  ///    to illustrate the difference in responsiveness between the two modes
  /// </remarks>
  public class SampleLazyDataItem : ILazyDataItem, INotifyPropertyChanged
  {
    static int nextId = 0;
    public int Index { get; internal set; }
    public int Id { get; private set; }
    string text;
    Brush brush;

    string smallAndFast;
    Brush smallAndSlow;
    byte[] largeAndFast;
    byte[] largeAndSlow;

    internal bool IsLazy { get; set; }

    public string SmallAndFastData
    {
      get
      {
        if (smallAndFast != null)
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Getting small and fast data for " + Id);
#endif
          return smallAndFast;
        }
        else
        {
          throw new InvalidOperationException("Unexpected call to SmallAndFastData before it was loaded");
        }
      }
      private set
      {
        smallAndFast = value;
      }
    }

    public string LargeAndFastData
    {
      get
      {
        if (largeAndFast != null)
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Getting large and fast data for " + Id);
#endif
          return "large and fast data loaded; length=" + largeAndFast.Length;
        }
        else
        {
          throw new InvalidOperationException("Unexpected call to LargeAndFastData before it was loaded");
        }
      }
    }

    public Brush SmallAndSlowData
    {
      get
      {
        Debug.Assert(currentState != LazyDataLoadState.Unloaded && currentState != LazyDataLoadState.Minimum);

        if (smallAndSlow != null)
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Getting small and slow data for " + Id);
#endif
          return smallAndSlow;
        }
        else
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Request for not-yet-loaded small and slow data for " + Id);
#endif
          return new SolidColorBrush(Colors.Transparent);
        }
      }
      private set
      {
        smallAndSlow = value;
      }
    }

    LazyDataLoadState currentState;

    public SampleLazyDataItem(string text, int index, SolidColorBrush brush)
    {
      Id = nextId++;
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Creating object for " + index + ", unique Id=" + Id);
#endif
      Index = index;
      this.text = text;
      this.brush = brush;
    }

    public string LargeAndSlowData
    {
      get
      {
        Debug.Assert(currentState == LazyDataLoadState.Loading || currentState == LazyDataLoadState.Loaded || currentState == LazyDataLoadState.Reloading);

        if (largeAndSlow != null)
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Getting large and slow data for " + Id);
#endif
          return "Loaded slow, large data for " + Id + "; size=" + largeAndSlow.Length;
        }
        else
        {
#if DEBUG_SAMPLE_LAZY_ACTIONS
          Debug.WriteLine("Request for not-yet-loaded small and slow data for " + Id);
#endif
          return "Loading large and slow...";
        }
      }
    }

    public override string ToString()
    {
      return text + " " + Id;
    }

    static Random random = new Random();

    void LoadLargeAndFastData()
    {
      // between 10 and 100 k
      int size = random.Next(1, 10) * 10000;
      largeAndFast = new byte[size];
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs("LargeAndFastData"));
    }

    void BeginLoadLargeAndSlowData()
    {
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Starting thread to load large and slow data for " + Id);
#endif
      Thread t = new Thread(LoadWorker);
      t.Start();
    }

    void LoadWorker()
    {
      // Sleep for 0.5 to 2.5 seconds
      Thread.Sleep(random.Next(2000) + 500);
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Finished loading additional data for " + Id);
#endif
      Deployment.Current.Dispatcher.BeginInvoke(LoadLargeAndSlowOnUiThread);
    }

    bool pendingWorkLargeAndSlowOnUiThread = false;

    void LoadLargeAndSlowOnUiThread()
    {
      // Paused, so don't use UI thread
      if (IsPaused)
      {
        pendingWorkLargeAndSlowOnUiThread = true;
        return;
      }

      pendingWorkLargeAndSlowOnUiThread = false;

      // State has changed while we were asleep
      if (currentState != LazyDataLoadState.Loading && currentState != LazyDataLoadState.Reloading)
        return;

      // Sleep on UI thread for ~2 frames (@30fps), just for effect.
      Thread.Sleep(67);


      int size = random.Next(1, 10) * 10000;
      largeAndSlow = new byte[size];
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs("LargeAndSlowData"));

      if (smallAndSlow == null)
      {
        smallAndSlow = brush;
        if (handler != null)
          handler(this, new PropertyChangedEventArgs("SmallAndSlowData"));
      }

      EndLoadSlowData();
    }

    void EndLoadSlowData()
    {
      // State has changed while we were asleep
      if (currentState != LazyDataLoadState.Loading && currentState != LazyDataLoadState.Reloading)
        return;

      GoToState(LazyDataLoadState.Loaded);
    }

    void BeginLoadSmallAndSlowData()
    {
      // no-op for us; it is part of the LoadLargeAndSlow
    }

    void UnloadLargeData()
    {
      if (IsLazy != true)
      {
#if DEBUG_SAMPLE_LAZY_ACTIONS
        Debug.WriteLine("Ignoring unload large data for non-lazy item " + Id);
#endif
        return;
      }
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Unloading large data for " + Id);
#endif
      largeAndSlow = null;
      largeAndFast = null;
      pendingWorkLargeAndSlowOnUiThread = false;
      Unpause();

      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs("LargeAndSlowData"));
        handler(this, new PropertyChangedEventArgs("SmallAndSlowData"));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    void LoadSmallAndFastData()
    {
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Loading small and fast data for " + Id);
#endif
      smallAndFast = text + " " + Index + " (unique ID " + Id + ")";
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs("SmallAndFastData"));
      }
    }

    void UnloadAllData()
    {
      if (IsLazy != true)
      {
#if DEBUG_SAMPLE_LAZY_ACTIONS
        Debug.WriteLine("Ignoring unload data for non-lazy item " + Id);
#endif
        return;
      }
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Unloading all data for " + Id);
#endif

      smallAndFast = null;
      smallAndSlow = null;
      largeAndFast = null;
      largeAndSlow = null;
      pendingWorkLargeAndSlowOnUiThread = false;
    }

    public LazyDataLoadState CurrentState
    {
      get { return currentState; }
    }

    public void GoToState(LazyDataLoadState state)
    {
      LazyDataItemStateManagerHelper.CheckTransition(currentState, state);

      switch (state)
      {
        case LazyDataLoadState.Unloaded:
          UnloadAllData();
          break;

        case LazyDataLoadState.Minimum:
          LoadSmallAndFastData();
          break;

        case LazyDataLoadState.Loading:
          LoadLargeAndFastData();
          if (IsPaused)
            Unpause();
          BeginLoadLargeAndSlowData();
          BeginLoadSmallAndSlowData();
          break;

        case LazyDataLoadState.Loaded:
          // nothing;
          break;

        case LazyDataLoadState.Cached:
          UnloadLargeData();
          break;

        case LazyDataLoadState.Reloading:
          LoadLargeAndFastData();
          if (IsPaused)
            Unpause();
          BeginLoadLargeAndSlowData();
          break;

        default:
          throw new InvalidOperationException("Unknown current state " + currentState.ToString());
      }

      EventHandler<LazyDataItemStateChangedEventArgs> handler = CurrentStateChanged;
      if (handler != null)
        handler(this, new LazyDataItemStateChangedEventArgs(currentState, state));

      currentState = state;
    }

    public void Pause()
    {
      if (isPaused == true)
      {
#if DEBUG_SAMPLE_LAZY_ACTIONS
        Debug.WriteLine("Request to pause already paused load for " + Id);
#endif
        return;
      }
#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Pausing data load for " + Id);
#endif
      isPaused = true;

      EventHandler<LazyDataItemPausedStateChangedEventArgs> handler = PauseStateChanged;
      if (handler != null)
        handler(this, new LazyDataItemPausedStateChangedEventArgs(true));
    }

    public void Unpause()
    {
      if (isPaused == false)
      {
#if DEBUG_SAMPLE_LAZY_ACTIONS
        Debug.WriteLine("Request to unpause already paused load for " + Id);
#endif
        return;
      }

#if DEBUG_SAMPLE_LAZY_ACTIONS
      Debug.WriteLine("Unpausing data load for " + Id);
#endif
      isPaused = false;
      if (pendingWorkLargeAndSlowOnUiThread)
      {
#if DEBUG_SAMPLE_LAZY_ACTIONS
        Debug.WriteLine("Resuming pending work for " + Id);
#endif
        LoadLargeAndSlowOnUiThread();
      }

      if (PauseStateChanged != null)
        PauseStateChanged(this, new LazyDataItemPausedStateChangedEventArgs(true));
    }

    bool isPaused = false;
    public bool IsPaused
    {
      get { return isPaused; }
    }

    public event EventHandler<LazyDataItemStateChangedEventArgs> CurrentStateChanged;
    public event EventHandler<LazyDataItemPausedStateChangedEventArgs> PauseStateChanged;
  }
}
