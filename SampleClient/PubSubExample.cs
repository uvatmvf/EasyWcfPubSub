using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media.Imaging;
using WcfPubSubClientBase;

namespace TestPubSub
{
    public class ImagePublisherExample : PublisherBase
    {
        public bool PublishImages { get; set; }
        private long counter;
        public void Publish()
        {
            if (PublishImages)
            {
                base.Publish(SampleBitmapSerializer.GetSerializedBitmap($"Image publication No. {counter++}\n on {Channel}"));
            }
            else
            {
                base.Publish($"Text publication No. {counter++} on {Channel}");
            }
        }        
    }

    public class SubscriberExample : ViewModelBase<SubscriberExample>,        
        IDisposable
    {
        public SubscriberBase Subscriber { get; set; }       
        public string Buffer { get; set; } = string.Empty;
        public BitmapSource Image { get; set; }
        public bool OnDelay { get; set; }

        public SubscriberExample(string[] channels)
        {
            Subscriber = new SubscriberBase(channels)
            {
                syncContext = SynchronizationContext.Current,
                Publish = (channel, publication) => { TypePublication(channel, publication); },                
            };
        }       

        public List<string> Channels { get; set; } = new List<string>();

        private void TypePublication(string c, string p)
        {
            if (OnDelay)
            {
                Random rdm = new Random(DateTime.Now.Millisecond);
                DateTime stamp = DateTime.Now;
                Thread.Sleep(rdm.Next(1000, 3500));
            }
            try
            {
                if (!Channels.Contains(c))
                {
                    Channels.Add(c);
                    OnPropertyChanged(x => x.Channels);
                }
                Image = SampleBitmapSerializer.DeserializeBitmap(p);
                OnPropertyChanged(x => x.Image);
            }
            catch
            {
                Buffer = p;
                OnPropertyChanged(x => x.Buffer);
            }
        }        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                Subscriber.Client.Unsubscribe();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Subscriber() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }    
}