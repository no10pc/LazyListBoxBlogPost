using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Collections;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using NetflixBrowserTest.ViewModels;
using System.Collections.Specialized;

namespace NetflixBrowserTest
{
  public class MainViewModel : IList, INotifyPropertyChanged, INotifyCollectionChanged
  {
    // The maximum number of things we want from Netflix (and actually the max it returns, at least today)
    private const int MAX_COUNT = 500;

    /// <summary>
    /// Event for notifying clients of when the download stats chages
    /// </summary>
    public event EventHandler<DownloadStatusChangedEventArgs> DownloadStatusChanged;

    /// <summary>
    /// Standard event for letting someone know our properties have changed
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Standard event for letting someone know our collection has changed
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Whether or not data has been downloaded
    /// </summary>
    public bool IsDataLoaded { get; private set; }

    /// <summary>
    /// Namespace used by Netflix
    /// </summary>
    private readonly XNamespace ATOM_NAMESPACE = "http://www.w3.org/2005/Atom";

    /// <summary>
    /// Namespace used by Netflix
    /// </summary>
    private readonly XNamespace ODATA_NAMESPACE = "http://schemas.microsoft.com/ado/2007/08/dataservices";

    /// <summary>
    /// Raises the DownloadStatusChanged Event
    /// </summary>
    /// <param name="message">Message to go along with the event</param>
    /// <param name="isComplete">Whether the download is complete or not</param>
    private void RaiseDownloadStatusChanged(string message, bool isComplete)
    {
      var handler = DownloadStatusChanged;
      if (handler != null)
        handler(this, new DownloadStatusChangedEventArgs(message, isComplete));
    }

    /// <summary>
    /// Kicks off the asynchronous load of data from Netflix
    /// </summary>
    public void LoadData()
    {
      if (IsDataLoaded)
        return;

      // First we ask how many items will be returned if we ask for MAX_COUNT
      HttpWebRequest request = WebRequest.CreateHttp("http://odata.netflix.com/v1/Catalog/Titles/$count?$top=" + MAX_COUNT);
      request.BeginGetResponse(DownloadItemCountComplete, request);
      RaiseDownloadStatusChanged("connecting...", false);
    }

    /// <summary>
    /// Handler for when the initial count is returne from Netflix
    /// </summary>
    /// <param name="result">The IAsyncResult of the WebRequest</param>
    void DownloadItemCountComplete(IAsyncResult result)
    {
      HttpWebRequest request = (HttpWebRequest)result.AsyncState;
      WebResponse response = request.EndGetResponse(result);
      string value = new StreamReader(response.GetResponseStream()).ReadToEnd();

      // This will cause the UI to show dummy items
      Count = int.Parse(value);

      // Now download the full items
      BeginDownloadOfItems();
    }

    /// <summary>
    /// Download the items from Netflix
    /// </summary>
    void BeginDownloadOfItems()
    {
      // Ask for as many items as Netflix said it could return
      HttpWebRequest request = WebRequest.CreateHttp("http://odata.netflix.com/v1/Catalog/Titles/?$top=" + Count);
      request.BeginGetResponse(DownloadItemListComplete, request);

      RaiseDownloadStatusChanged("loading...", false);
    }

    /// <summary>
    /// Handler for when the full download has completed
    /// </summary>
    /// <param name="result">The IAsyncResult of the WebRequest</param>
    void DownloadItemListComplete(IAsyncResult result)
    {
      RaiseDownloadStatusChanged("parsing...", false);

      // Get the response stream, and perform a Linq query against it
      HttpWebRequest request = (HttpWebRequest)result.AsyncState;
      WebResponse response = request.EndGetResponse(result);
      XDocument doc = XDocument.Load(response.GetResponseStream());
      var items = from entry in doc.Descendants(ATOM_NAMESPACE + "entry")
                  select new NetflixData
                    (
                      entry.Element(ATOM_NAMESPACE + "id").Value,
                      entry.Element(ATOM_NAMESPACE + "title").Value,
                      entry.Element(ATOM_NAMESPACE + "summary").Value,
                      (from img in entry.Descendants(ODATA_NAMESPACE + "MediumUrl")
                       select img).FirstOrDefault().Value
                    );

      // Get the enumerator for later use (we will NOT be foreach-ing over the enumeration)
      downloadedItems = items.GetEnumerator();

      // Update any of the dummy items we already created
      PopulatePreloadedItems();

      // Done!
      IsDataLoaded = true;
      RaiseDownloadStatusChanged("done!", true);
    }

    /// <summary>
    /// Populates the already-loaded data items with the downloaded data
    /// </summary>
    private void PopulatePreloadedItems()
    {
      int index = 0;
      while (lastEnumeratedItem < loadedItems.Count - 1)
      {
        if (downloadedItems.MoveNext())
        {
          loadedItems[index].Initialize(downloadedItems.Current);
          lastEnumeratedItem++;
          index++;
        }
        else
          throw new Exception("No more data!");
      }
    }

    /// <summary>
    /// Helper to raise the PropertyChanged event
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed</param>
    /// <param name="dispatch">Whether to dispatch on the UI thread or not</param>
    private void OnPropertyChanged(String propertyName, bool dispatch)
    {
      var handler = PropertyChanged;
      if (null != handler)
      {
        if (dispatch)
          Deployment.Current.Dispatcher.BeginInvoke(() => handler(this, new PropertyChangedEventArgs(propertyName)));
        else
          handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Helper to raise the CollectionChanged event
    /// </summary>
    /// <param name="action">The action that has occurred</param>
    /// <param name="dispatch">Whether to dispatch on the UI thread or not</param>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, bool dispatch)
    {
      var handler = CollectionChanged;
      if (null != handler)
      {
        if (dispatch)
          Deployment.Current.Dispatcher.BeginInvoke(() => handler(this, new NotifyCollectionChangedEventArgs(action)));
        else
          handler(this, new NotifyCollectionChangedEventArgs(action));
      }
    }

    /// <summary>
    /// Index of the last item that was enumerated from the downloaded list of items
    /// </summary>
    int lastEnumeratedItem = -1;

    /// <summary>
    /// Enumerator of data items that will be the result of a LINQ query
    /// </summary>
    IEnumerator<NetflixData> downloadedItems = null;

    /// <summary>
    /// List of actual loaded items
    /// </summary>
    List<NetflixData> loadedItems = new List<NetflixData>();

    /// <summary>
    /// The number of items in the list
    /// </summary>
    int count = 0;

    /// <summary>
    /// The total number of items in the list
    /// </summary>
    public int Count
    {
      get 
      { 
        return count; 
      }

      private set
      {
        if (count == value) 
          return; 

        count = value; 
        OnPropertyChanged("Count", true); 
        OnCollectionChanged(NotifyCollectionChangedAction.Reset, true);
      }
    }

    /// <summary>
    /// Clear everything out
    /// </summary>
    public void ClearData()
    {
      lastEnumeratedItem = -1;
      downloadedItems = null;
      loadedItems.Clear();
      IsDataLoaded = false;
      Count = 0;
    }

    /// <summary>
    /// Indexer for the Netflix items. Will return dummy items if the list is not yet loaded,
    /// but they will be filled in with data asynchronously
    /// </summary>
    /// <param name="index">The index of the item to get</param>
    /// <returns>The item</returns>
    public object this[int index]
    {
      get
      {
        // Return empty values at first
        if (IsDataLoaded == false)
        {
          while (loadedItems.Count <= (index + 1))
          {
            loadedItems.Add(new NetflixData());
          }
        }
        else
        {
          // Load the correct items
          while (lastEnumeratedItem < index)
          {
            if (downloadedItems.MoveNext())
            {
              loadedItems.Add(downloadedItems.Current);
              lastEnumeratedItem++;
            }
            else
              throw new Exception("No more data!");
          }
        }

        return loadedItems[index];
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    #region IList Members

    public int Add(object value)
    {
      throw new NotImplementedException();
    }

    public int IndexOf(object item)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public bool Contains(object value)
    {
      throw new NotImplementedException();
    }

    public void Insert(int index, object value)
    {
      throw new NotImplementedException();
    }

    public bool IsFixedSize
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public void Remove(object value)
    {
      throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region ICollection Members

    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    #endregion

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}