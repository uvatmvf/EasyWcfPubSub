using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace WcfPubSubClientBase
{

    public class PublisherBase : IPubSubServiceCallback
    {
        public PubSubServiceClient ServerClient { get; set; }
        public string Channel { get; set; } = "Sample";

        public PublisherBase() =>
            ServerClient = new PubSubServiceClient(new InstanceContext(this));

        public void OnPublished(string channel, string publish) { }

        public void Publish(string s)
        {
            ServerClient.Publish(Channel, s);
        }
    }

    public class SubscriberBase : IPubSubServiceCallback
    {
        public SynchronizationContext syncContext { get; set; }
        public static bool UseSyncContext { get; set; } = false;
        public static bool AsyncPublications { get; set; } = true;

        public void OnPublished(string channel, string publish)
        {
            if (!AsyncPublications)
            {
                Publish?.Invoke(channel, publish);
            }
            else if (AsyncPublications && UseSyncContext)
            {
                Task.Run(() =>
                    syncContext.Send(s =>
                    {
                        Publish?.Invoke(channel, publish);
                    }, publish));
            }
            else
            {
                Task.Run(() => Publish?.Invoke(channel, publish));
            }
        }
        public PubSubServiceClient Client { get; set; }
        public string Channel { get; set; } = string.Empty;

        public SubscriberBase(string[] channels)
        {
            Client = new PubSubServiceClient(new InstanceContext(this));
            Client.Subscribe(channels);
        }

        public SubscriberBase(string channel)
           : this(string.IsNullOrEmpty(channel) ?
                 new string[0] :
               new string[1] { channel })
        { }

        public SubscriberBase()
            : this("")
        { }

        public Action<string, string> Publish { get; set; }

    }
}
