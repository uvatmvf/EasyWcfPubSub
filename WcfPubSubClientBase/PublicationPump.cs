using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestPubSub
{
    public class PublicationPump
    {
        public CancellationTokenSource Terminator { get; set; } = new CancellationTokenSource();
        public Task Pump { get; set; }
        public TimeSpan PumpDelay { get; set; }
        public Action Function { get; set; }

        public PublicationPump(Action a, TimeSpan aWaits)
        {
            Function = a;
            PumpDelay = aWaits;
            Pump = Task.Factory.StartNew(Function, Terminator.Token);
        }

        public void Prime()
        {
            while (!Terminator.IsCancellationRequested)
            {
                Function();
                Terminator.Token.WaitHandle.WaitOne(PumpDelay);
            }
        }
    }
}
