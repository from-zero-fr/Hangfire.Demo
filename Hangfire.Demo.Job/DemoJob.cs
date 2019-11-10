using Hangfire.Server;
using NLog;
using System;
using System.Threading;

namespace Hangfire.Demo.Job
{
    public interface IDemoJob
    {
        void Execute(DateTime datetime, PerformContext context);
    }

    public class DemoJob : IDemoJob
    {
        private static Logger logger;

        public DemoJob() => logger = LogManager.GetCurrentClassLogger();

        public void Execute(DateTime datetime, PerformContext context)
        {
            using (NLog.NestedDiagnosticsContext.Push(context.BackgroundJob.Id))
            {
                logger.Info($"Creation time:{datetime.ToString()}");
                for (int i = 0; i <= 100; i += 5)
                {
                    logger.Info($"Background job progress: {i}");
                    Thread.Sleep(100);
                }
            }
        }
    }
}