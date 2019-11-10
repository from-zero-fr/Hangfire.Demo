using Hangfire.Demo.Job;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Hangfire.Demo.Client.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : ApiController
    {
        private static Logger logger;

        public TaskController()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        [HttpGet]
        [Route("execute")]
        public IHttpActionResult Execute()
        {
            logger.Info("Start setting up delayed jobs, execute after 1 minute");

            // Create a delayed job
            var jobId = BackgroundJob.Schedule<IDemoJob>(t => t.Execute(DateTime.Now, null), TimeSpan.FromMinutes(1));

            // Find if a periodic job exists, and if it does not exist, create a job
            var list = JobStorage.Current.GetConnection().GetAllEntriesFromHash("recurring-job:Client_Recurring");
            if (list == null)
            {
                logger.Warn("job not found, create 1 minute periodic job");
                Hangfire.RecurringJob.AddOrUpdate<IDemoJob>("Client_Recurring", t => t.Execute(DateTime.Now, null), Hangfire.Cron.Minutely, TimeZoneInfo.Local);
            }
            else
            {
                logger.Warn("Periodic job already exists");
            }
            return Ok();
        }
    }
}
