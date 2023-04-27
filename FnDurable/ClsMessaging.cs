using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnDurable
{
    public class ClsMessaging
    {
        public async Task AddMessageAsync(string data)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=msitstorageaccount;AccountKey=HCB2Oc6LHBMA7FAiSu9JAyAorap+nFYh7lzfxPUXyL8jWcPZa84YY1eeI0hETp5fYWUGS4yXL/DA+AStyqbTDA==;EndpointSuffix=core.windows.net");
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("durable-message");
            CloudQueueMessage queueMessage = new CloudQueueMessage(data);
            await cloudQueue.AddMessageAsync(queueMessage);
            Console.WriteLine("Message sent");

        }
    }
}
