using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Hangfire.Demo.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                // Set Topshelf Configuration
                x.Service<Service>(h =>
                {
                    h.ConstructUsing(n => new Service());
                    h.WhenStarted(s => s.Start());
                    h.WhenStopped(s => s.Stop());
                });

                // Enable Nlog Integration
                x.UseNLog();

                // Set Service Start Mode
                x.StartAutomaticallyDelayed();

                // Set Service Description
                x.SetDescription("Hangfire Service");
                x.SetDisplayName("Hangfire Service");
                x.SetServiceName("HangfireService");
            });
        }
    }
}
