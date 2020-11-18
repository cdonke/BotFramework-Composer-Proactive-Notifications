using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace IdleFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //Query Table Storage using time filters for the last 5 minutes
            //    Timestamp ge datetime'2020-11-18T16:43:33.754Z'
            // Foreach-Loop on each result
            //    Post asking whether is there or not
        }
    }
}
