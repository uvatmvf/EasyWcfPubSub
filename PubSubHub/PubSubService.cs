using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;

namespace PubSubService2
{
    [ServiceContract(CallbackContract = typeof(IPubSubSubscriber), SessionMode= SessionMode.Required)]
    public interface IPubSubService
    {
        [OperationContract]
        void Subscribe(string[] channels);

        [OperationContract]
        void Unsubscribe();

        [OperationContract]
        void Publish(string channel, string publish);

        [OperationContract]
        void Ping();
        // TODO: Add your service operations here
    }

    [ServiceContract]
    public interface IPubSubSubscriber
    {
        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IPubSubService/OnPublished")]
        void OnPublished(string channel, string publish);

        [OperationContract(IsOneWay = true, AsyncPattern = true, Action = "http://tempuri.org/IPubSubService/OnPublished")]
        IAsyncResult BeginOnPublished(string channel, string publish, AsyncCallback callback, object state);

        void EndOnPublished(IAsyncResult result);
                        
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PubSubService : ServiceBase, IPubSubService
    {        
        private static ConcurrentDictionary<IPubSubSubscriber, List<string>> _callbackChannels = new ConcurrentDictionary<IPubSubSubscriber, List<string>>();
        private static object _syncLock = new object();
        private IPubSubSubscriber _mySubscriberCallback;
        private ServiceHost _serviceHost;
        protected override void OnStart(string[] args)
        {
            try
            {
                _serviceHost = new ServiceHost(typeof(PubSubService));
                _serviceHost.Open();
            }
            catch (Exception e)
            {
                ConsoleExtension.WriteError(e);
            }
        }

        public void Publish(string channel, string publish)
        {
            for (int i = _callbackChannels.Keys.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (_callbackChannels[_callbackChannels.Keys.ElementAt(i)].Count == 0 ||
                        _callbackChannels[_callbackChannels.Keys.ElementAt(i)].Contains(channel))
                    {
                        if (Properties.Settings.Default.PublishAsync)
                        {
                            _callbackChannels.Keys.ElementAt(i).BeginOnPublished(channel, publish, new AsyncCallback(x => { }), null);
                        }
                        else
                        {
                            _callbackChannels.Keys.ElementAt(i).OnPublished(channel, publish);
                        }
                    }
                }
                catch (Exception e)
                {
                    ConsoleExtension.WriteError(e);
                    _callbackChannels.TryRemove(_callbackChannels.Keys.ElementAt(i), value: out List<string> channels);
                    ConsoleExtension.WriteDrop();
                }
            }
        }
        
        public void Subscribe(string[] channels)
        {
            var callback = _mySubscriberCallback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();
            lock (_syncLock)
            {
                if (!_callbackChannels.ContainsKey(callback))
                {
                    // empty is subscribe all
                    var channelList = new List<string>();
                    channelList.AddRange(channels);
                    _callbackChannels.TryAdd(callback, channelList);
                    ConsoleExtension.WriteAdd();
                    
                }
            }   
        }

        public void Unsubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IPubSubSubscriber>();
            lock (_syncLock)
            {
                if (_callbackChannels.ContainsKey(callback))
                {
                    _callbackChannels.TryRemove(callback, value: out List<string> channels);
                    ConsoleExtension.WriteDrop();
                }
            }
        }

        protected override void OnStop() => _serviceHost.Close();

        public void Ping() { }

        public void Start() => OnStart(null);

        public void Stop() => OnStop();
    }
}
