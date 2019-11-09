using Autofac;
using Hangfire.Demo.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire.Demo.Server
{
    public class Service
    {
        private BackgroundJobServer backgroundJobServer;

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            // Setup Sql Server Storage
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDemo", new SqlServer.SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            });
            GlobalConfiguration.Configuration.UseNLogLogProvider();
            GlobalConfiguration.Configuration.UseAutofacActivator(ConfigureContainer());

            // Run Hangfire
            backgroundJobServer = new BackgroundJobServer(new BackgroundJobServerOptions
            {
                ServerName = Guid.NewGuid().ToString(),
                WorkerCount = 4,
                Queues = new string[] { "default"}
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            backgroundJobServer?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            RegisterApplicationComponents(builder);
            return builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterApplicationComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DemoJob>().As<IDemoJob>().InstancePerLifetimeScope();
        }
    }
}
