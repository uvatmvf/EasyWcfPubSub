# Easy Wcf PubSub Service and Client
A publisher subscriber service implemented in C#/WCF

Simple asynchronous pub sub service implemented in .Net WCF.
<h1>Why?</h1>
Many pub-sub implementations couple publishers and subscribers using a static event callback. This can cause delays when serving publications to slow clients. These do not scale, and fail on intermittency or clients that take a long time to process/accept the subscription.

This implementation decouples publishers and subscribers into asynchronous, fire and forget callbacks.

Most pub-sub implementations are library implementations.

This implementation publishes through a WCF service, using a generic interface. 

Most wcf services require the clients to adopt the typed library of the service contract.

The publisher and subscribers are free to serialize objects through the pipe with no changes to the data contract by allowing them to serialize to JSON/Xml/whatever before serializing publish and deserializing after subscription is received.
<h1>At a glance</h1>
<ul>
<li>Wcf Pub Sub Service</li>
<li>Wcf Pub Sub Console Application</li>
<li>Wcf Pub Sub Publish-Subscriber Interface library</li>
  <li>Wpf Sample Application</li>
</ul>
<h1>Getting Started with the Pub Sub Service</h1>
<ul>
  <li><h3>To Publish:</h3>
    
    {        
        var publisher = new PublisherBase();    
        publisher.Publish("channel", "foo");
    }
    
  </li>
  <li><h3>To Subscribe:</h3>
    
    {
        
        var subscriber = new SubscriberBase(new string[] { "channel1", ... })
        // if you want to subscribe to all channels, pass empty or null string array for channels to constructor.
        {
           Publish = (channel, publication) => { 
              Console.WriteLine($"Listening on {Channel} to receive {publication}");              
              }
          },                                                      
          //  syncContext = SynchronizationContext.Current, // uncomment if you want to set a synchronization context
        }       
                
        // SubscriberBase.AsyncPublications = false; // uncomment if you want to handle all subscriptions synchronously
        
        // SubscriberBase.UseSyncContext = true; // uncomment if you want to use the synchronoization context 
        
    }
    
  </li>
</ul>

<h1>Nuget</h1>
<h1>Quick Start Examples</h1>
1. Download code from repository. 

2. Open solution file. (Restore nuget packages)

3. Build all.

4. Open WcfPubSubConsoleServer bin\debug folder

5. Run WcfPubSubConsoleServer.exe as administrator

6. Open WcfSampleClient bin\debug folder

7. Run TestPubSub.exe application.

The console server should look like:

server:
<img src="https://github.com/uvatmvf/EasyWcfPubSub/blob/master/server.PNG" />

client:
<img src="https://github.com/uvatmvf/EasyWcfPubSub/blob/master/clientSample.png" />

The sample client and server applications provide demonstration for the pub sub service.
The sample application has a proxy class generated from the WCF service and configuration files. 
All publications are serialized to string (JSON, XML) before publishing to the service.
The sample application demonstrates two types of message publication, string and jpeg image. (And you can define any 
data you want).

