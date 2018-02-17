using CommandLine;
using PubSubService2;
using System;

namespace WcfPubSubConsoleServer
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var options = CommandLine.Parser.Default.ParseArguments<PubSubOptions>(args);
                options.WithParsed<PubSubOptions>(o => {
                    PubSubService2.Properties.Settings.Default.PublishAsync = !o.PublishSync;
                    PubSubService2.Properties.Settings.Default.WritePublicationsToConsole = !o.WritePublicationsToConsole;
                    });
                options.WithNotParsed<PubSubOptions>(e =>
                {
                    Console.WriteLine("No arguments for console. Using settings file options.");
                });
            }
            var svc = new PubSubService();
            svc.Start();
            Console.WriteLine("Running pub sub service in console.");
            Console.WriteLine($" >> Publish {(PubSubService2.Properties.Settings.Default.PublishAsync ? "asynchronously.  (use -s on console to publish synchronously)" : "synchronously" )}.");
            Console.WriteLine($" >> Print publication(s) to console{(PubSubService2.Properties.Settings.Default.WritePublicationsToConsole ? "." : " is suppressed. (use '-p' on console to print.)")}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            svc.Stop();
        }

        class PubSubOptions
        {
            [Option(shortName:'s',                 
                longName: "Publish Synchronously",
                Default = false)]
            public bool PublishSync { get; set; }

            [Option(shortName: 'p',
                longName: "Write publications to console.",
                Default = false, 
                HelpText = "Write publications to console." )]
            public bool WritePublicationsToConsole { set; get; }
        }
    }
}
