using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WcfPubSubClientBase;

namespace TestPubSub
{
    class PubSubViewModel : ViewModelBase<PubSubViewModel>, IDisposable
    {
        public ObservableCollection<SubscriberExample> Subscribers { get; set; } = new ObservableCollection<SubscriberExample>();
        public ObservableCollection<ImagePublisherExample> Publishers { get; set; } = new ObservableCollection<ImagePublisherExample>();

        private PublicationPump serviceCalls;

        public bool SynchronizeOnUiThread
        {
            get { return Properties.Settings.Default.SynchronizeOnUiThread; }
            set
            {
                SubscriberBase.UseSyncContext =
                Properties.Settings.Default.SynchronizeOnUiThread = value;
                OnPropertyChanged(x => x.SynchronizeOnUiThread);
            }
        }

        public bool SubscribeAsynchronously
        {
            get { return Properties.Settings.Default.SubscribeAsynchronously; }
            set
            {
                SubscriberBase.AsyncPublications = 
                Properties.Settings.Default.SubscribeAsynchronously = value;
                OnPropertyChanged(x => x.SubscribeAsynchronously);
            }
        }

        public ICommand PublishCommand { get; set; }
        public ICommand AddSubscriberCommand { get; set; }
        public ICommand AddPublisherCommand { get; set; }

        public PubSubViewModel()
        {

            PublishCommand = new ActionCommand()
            {
                CanExecuteFunction = e => { return Publishers.Count > 0; },
                ExecuteAction = e => {
                    if (serviceCalls != null)
                    {
                        serviceCalls.Terminator.Cancel();
                    }
                    serviceCalls = new PublicationPump(() => {
                        Parallel.ForEach(Publishers, x => x.Publish());
                    }, new TimeSpan(0, 0, 0, 0, 200));
                    Task.Run(() => serviceCalls.Prime());
                }
            };

            AddSubscriberCommand = new ActionCommand()
            {
                CanExecuteFunction = e => { return true; },
                ExecuteAction = e =>
                {
                    List<string> channels = new List<string>();
                    Publishers.ToList().ForEach(x => channels.Add(x.Channel));
                    Subscribers.Add(new SubscriberExample(channels.ToArray()));
                    OnPropertyChanged(x => x.Subscribers);
                }
            };

            AddPublisherCommand = new ActionCommand()
            {
                CanExecuteFunction = e => { return true; },
                ExecuteAction = e =>
                {
                    Publishers.Add(new ImagePublisherExample() { Channel = $"channel{Publishers.Count}" });
                    OnPropertyChanged(x => x.Publishers);
                }
            };

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
                serviceCalls?.Terminator.Cancel();
                Subscribers.ToList().ForEach(x => x.Subscriber.Client.Unsubscribe());
                //client.Unsubscribe();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PubSubViewModel() {
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
    
    public class ViewModelBase<T> : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged<T2>(Expression<Func<T, T2>> accessor) =>
            OnPropertyChanged(PropertyName(accessor));

        public virtual string PropertyName<T2>(Expression<Func<T, T2>> accessor) =>
            ((MemberExpression)accessor.Body).Member.Name;

        public virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}