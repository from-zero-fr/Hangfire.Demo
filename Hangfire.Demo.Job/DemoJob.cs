using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire.Demo.Job
{
    public class DemoJob : IDemoJob
    {
        private static Logger logger;

        public DemoJob()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute(DateTime datetime)
        {
            logger.Warn($"Creation time:{datetime.ToString()}");
        }
    }
}
