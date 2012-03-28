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
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetflixBrowserTest.ViewModels
{
  public class NetflixData : INotifyPropertyChanged
  {
    /// <summary>
    /// Amount of memory to copy from streams at a time
    /// </summary>
    const int MAX_COPY_CHUNK_SIZE = 5000;

    /// <summary>
    /// Create an empty item, to be filled in later by Initialize
    /// </summary>
    public NetflixData()
    {
      this.title = "loading...";
    }

    /// <summary>
    /// Create a new NetflixData object
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <param name="title">The title of the item</param>
    /// <param name="description">The description of the item</param>
    /// <param name="boxArtPath">The path to the box art</param>
    public NetflixData(string id, string title, string description, string boxArtPath)
    {
      Initialize(id, title, description, boxArtPath, false);
    }

    /// <summary>
    /// Initialize an existing object
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <param name="title">The title of the item</param>
    /// <param name="description">The description of the item</param>
    /// <param name="boxArtPath">The path to the box art</param>
    public void Initialize(string id, string title, string description, string boxArtPath)
    {
      Initialize(id, title, description, boxArtPath, true);
    }

    public void Initialize(NetflixData data)
    {
      string path = (data.imageUri != null ? data.imageUri.AbsoluteUri : null);
      Initialize(data.id.AbsoluteUri, data.title, data.description, path, true);
    }

    /// <summary>
    /// Initialize the object and optionally raise the change notification
    /// </summary>
    /// <param name="id">The id of the item</param>
    /// <param name="title">The title of the item</param>
    /// <param name="description">The description of the item</param>
    /// <param name="boxArtPath">The path to the box art</param>
    /// <param name="raiseNotification">Whether or not to raise the PropertyChanged event</param>
    private void Initialize(string id, string title, string description, string boxArtPath, bool raiseNotification)
    {
      this.id = new Uri(id);
      this.title = title;
      this.description = description;
      Uri uri;
      if (Uri.TryCreate(boxArtPath, UriKind.Absolute, out uri))
        this.imageUri = uri;

      if (raiseNotification)
        OnPropertyChanged(null);
    }

    Uri id;
    /// <summary>
    /// The ID of the item
    /// </summary>
    public Uri Id
    {
      get { return id; }
      set { if (id == value) return; id = value; OnPropertyChanged("Id"); }
    }

    string title;
    /// <summary>
    /// The title of the item
    /// </summary>
    public string Title
    {
      get { return title; }
      set { if (title == value) return; title = value; OnPropertyChanged("Title"); }
    }

    string description;
    /// <summary>
    /// The description of the item
    /// </summary>
    public string Description
    {
      get { return description; }
      set { if (description == value) return; description = value; OnPropertyChanged("Description"); }
    }

    Uri imageUri;
    /// <summary>
    /// The URI of the box art image
    /// </summary>
    public Uri ImageUri
    {
      get { return imageUri; }
      set { if (imageUri == value) return; imageUri = value; bitmapImage = null; OnPropertyChanged("ImageUri"); OnPropertyChanged("ImageSource"); }
    }

    WeakReference<BitmapImage> bitmapImage;
    /// <summary>
    /// The actual image object representing the box art, which is created on-demand
    /// </summary>
    /// <remarks>
    /// When this property is retrieved, it immediately returns the image if already loaded; otherwise it
    /// downloads it on a background thread and will raise the PropertyChanged event when it has been loaded
    /// </remarks>
    public ImageSource ImageSource
    {
      get
      {
        if (bitmapImage != null)
        {
          if (bitmapImage.IsAlive)
            return bitmapImage.Target;
          else
          {
            Debug.WriteLine("Bitmap source has been GCed; creating new one...");
          }
        }

        // Begin downloading the image off-thread
        if (imageUri != null)
          ThreadPool.QueueUserWorkItem(DownloadImage, imageUri);

        return null;
      }
    }

    /// <summary>
    /// Introduce some randomness in our threads to avoid "boot storms"
    /// </summary>
    Random random = new Random();

    /// <summary>
    /// Begins downloading the box art
    /// </summary>
    /// <param name="state"></param>
    void DownloadImage(object state)
    {
      // Create simple web request...
      HttpWebRequest request = WebRequest.CreateHttp(state as Uri);
      request.BeginGetResponse(DownloadImageComplete, request);
    }

    /// <summary>
    /// Completes the download of the image, and notifies the UI
    /// </summary>
    /// <param name="result"></param>
    void DownloadImageComplete(IAsyncResult result)
    {
      // Insert random sleep so that all images don't fire at once, impacting performance
      Thread.Sleep(random.Next(1000));

      // Get the stream
      HttpWebRequest request = result.AsyncState as HttpWebRequest;
      HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
      Stream stream = response.GetResponseStream();

      // We need to copy the stream into our own memory stream because the BitmapImage
      // can only be modified from the UI thread, but the WebResponse can only be
      // accessed on a non-UI thread...
      Stream copy = CopyStream(stream, (int) response.ContentLength);

      // Create the bitmap and raise the notification event on the UI thread
      Deployment.Current.Dispatcher.BeginInvoke(delegate
      {
        BitmapImage bm = new BitmapImage();
        bm.SetSource(copy);

        if (bitmapImage == null)
          bitmapImage = new WeakReference<BitmapImage>(bm);
        else
          bitmapImage.Target = bm;

        OnPropertyChanged("ImageSource");
      });
    }

    /// <summary>
    /// Copies a stream into a new memory stream, and returns the copy seeked to the origin
    /// </summary>
    /// <param name="stream">The stream to copy</param>
    /// <returns>The copied stream. The stream pointer will be at the origin</returns>
    static Stream CopyStream(Stream stream, int length)
    {
      Stream copy = new MemoryStream(length);

      int chunkSize = Math.Min(length, MAX_COPY_CHUNK_SIZE);
      byte[] buffer = new byte[chunkSize];
      int amountRead = 0;

      do
      {
        amountRead = stream.Read(buffer, 0, chunkSize);
        copy.Write(buffer, 0, amountRead);
      } while (amountRead == chunkSize);

      copy.Seek(0, SeekOrigin.Begin);
      return copy;
    }

    /// <summary>
    /// Helper to raise the PropertyChanged event
    /// </summary>
    /// <param name="property">The property name</param>
    void OnPropertyChanged(string property)
    {
      var handler = PropertyChanged;
      if (handler != null)
        Deployment.Current.Dispatcher.BeginInvoke(() => handler(this, new PropertyChangedEventArgs(property)));
    }

    // Standard event from INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
  }

  /// <summary>
  /// This is a dummy class so we can get design-time data generated
  /// </summary>
  public class NetflixDataList
  {
    public List<NetflixData> Data { get; private set;  }
  }
}
