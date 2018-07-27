using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    class LockScreenStorage
    {
        CloudQueue queue;

        public LockScreenStorage()
        {
            this.Init();
        }

        private async void Init()
        {
            // SA
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=lockscreen;AccountKey=+mlPG0MQCAqPF2SczMqWdSRJP9Kk9GTzEPACl008QoSI3dgXNtLtnuxWQI8MX8P4FioYfhLxoHRTTMzkpU38qA==";
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Queue
            var qClient = storageAccount.CreateCloudQueueClient();
            queue = qClient.GetQueueReference("destination");
            await queue.CreateIfNotExistsAsync();
        }

        public async Task<CloudQueueMessage> ReadFromQueue()
        {
            return await queue.GetMessageAsync();
        }

        public async void DeleteMessage(CloudQueueMessage message)
        {
            await queue.DeleteMessageAsync(message);
        }
    }
}
