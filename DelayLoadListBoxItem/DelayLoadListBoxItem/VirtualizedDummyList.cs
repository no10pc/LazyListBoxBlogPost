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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using LazyListBox;

namespace DelayLoadListBoxItem
{
  /// <summary>
  /// Simple data source that uses our dummy data items
  /// </summary>
  public class VirtualizedDummyList : IList, INotifyCollectionChanged
  {
    public const int MAX_ITEMS = 10000;
    static Random r = new Random();
    Dictionary<int, SampleLazyDataItem> cache = new Dictionary<int, SampleLazyDataItem>();
    bool useLazyLoading;

    public VirtualizedDummyList(bool useLazyLoading)
    {
      this.useLazyLoading = useLazyLoading;
    }

    public int Count
    {
      get { return MAX_ITEMS; }
    }

    public int IndexOf(object value)
    {
      if (value == null)
        return -1;

      return (value as SampleLazyDataItem).Id;
    }

    public object this[int index]
    {
      get
      {
        SampleLazyDataItem item;
        if (cache.TryGetValue(index, out item))
          return item;
        
        item = new SampleLazyDataItem("Simple Text ", index, new SolidColorBrush(Color.FromArgb(255, (byte)(r.Next(180) + 20), (byte)(r.Next(180) + 20), (byte)(r.Next(180) + 20))));
        item.IsLazy = useLazyLoading;
        if (!useLazyLoading)
        {
          item.GoToState(LazyDataLoadState.Minimum);
          item.GoToState(LazyDataLoadState.Loading);
        }
        cache[index] = item;

        return item;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public void Insert(int newIndex, object value)
    {
      int[] sortedKeys = cache.Keys.ToArray();
      Array.Sort(sortedKeys);

      for (int i = sortedKeys.Length - 1; i >= 0; i--)
      {
        int key = sortedKeys[i];
        if (key < newIndex)
          break;

        cache[key + 1] = cache[key];
        cache[key].Index++;
      }

      cache[newIndex] = (SampleLazyDataItem)value;
      var handler = CollectionChanged;
      if (handler != null)
        handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, newIndex));
    }

    #region E_NOT_IMPL

    #region IList Members

    public int Add(object value)
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
    #endregion

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion
  }
}
