using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionApp1.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class CalculateDatesAndAmountsFunction
    {
        [FunctionName("CalculateDatesAndAmountsFunction")]
        public async Task Run([QueueTrigger("messagetomom", Connection = "")]MessageToMom myQueueItem, 
            [Queue("outputletter")] IAsyncCollector<FormLetter> letterCollector,
            ILogger log)
        {
            log.LogInformation($"{myQueueItem.Greeting} {myQueueItem.HowMuch} {myQueueItem.HowSoon}");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            //TODO parse flattery list into comma separated string
            var str = String.Join(",", myQueueItem.Flattery);
            log.LogInformation($"{str}");
            //TODO populate Header with salutation comma separated string and "Mother"
            var newstr = myQueueItem.Greeting +(", \"Mother\"");
            //TODO calculate likelihood of receiving loan based on this decision tree
            // 100 percent likelihood (initial value) minus the probability expressed from the quotient of howmuch and the total maximum amount ($10000)
            FormLetter formLetter = new FormLetter();
            var calculateliklihood= 100 - (myQueueItem.HowMuch / 10000);
            formLetter.Likelihood = Convert.ToDouble(calculateliklihood);
            //TODO calculate approximate actual date of loan receipt based on this decision tree
            // funds will be made available 10 business days after day of submission    
            // business days are weekdays, there are no holidays that are applicable
            var Calculatedate = myQueueItem.HowSoon;
            if (myQueueItem.HowSoon.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                Calculatedate = myQueueItem.HowSoon.Value.AddDays(15);
            }
            else if (myQueueItem.HowSoon.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                Calculatedate = myQueueItem.HowSoon.Value.AddDays(16);
            }
            else
            {
                Calculatedate = myQueueItem.HowSoon.Value.AddDays(14);
            }

            //TODO use new values to populate letter values per the following:
            //Body:"Really need help: I need $5523.23 by December 12,2020"
            //ExpectedDate = calculated date
            //RequestedDate = howsoon
            //Heading=Greeting
            //Likelihood = calculated likelihood

            await letterCollector.AddAsync(new FormLetter {Body= "Really need help: I need $5523.23 by December 12,2020", ExpectedDate=(DateTime)Calculatedate,
            RequestedDate=(DateTime)myQueueItem.HowSoon,Heading=newstr,Likelihood=formLetter.Likelihood});
        
    }

        
    }


    
}
