using System;
using System.Linq;
using System.Threading.Tasks;
using FunctionApp1.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApp1
{
    public class LogFormLetterToStorage
    {
        [FunctionName("LogFormLetterToStorage")]
        public async Task Run([QueueTrigger("outputletter", Connection = "")]FormLetter myQueueItem,
            [Table("letters")] IAsyncCollector<LetterEntity> letterTableCollector,
            ILogger log)
        {

            //TODO map FormLetter message to LetterEntity type and save to table storage
            await letterTableCollector.AddAsync(new LetterEntity 
            {
                PartitionKey = "US",
                RowKey = Guid.NewGuid().ToString(),
                Heading = myQueueItem.Heading,
                Likelihood = myQueueItem.Likelihood,
                ExpectedDate = myQueueItem.ExpectedDate,
                RequestedDate = myQueueItem.RequestedDate,
                Body = myQueueItem.Body
            });
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            //SaveToDatabase(letterTableCollector);
            log.LogInformation($"{myQueueItem.Heading}");
            
        }
    }

    public class LetterEntity : TableEntity {
        public string Heading { get; set; }
        public double Likelihood { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Body { get; set; }
    }
}
