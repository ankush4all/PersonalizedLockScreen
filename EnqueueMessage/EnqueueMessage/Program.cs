using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace EnqueueMessage
{
    class Program
    {
        static CloudQueue cloudQueue;

        static void Main(string[] args)
        {
            // SA
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=lockscreen;AccountKey=+mlPG0MQCAqPF2SczMqWdSRJP9Kk9GTzEPACl008QoSI3dgXNtLtnuxWQI8MX8P4FioYfhLxoHRTTMzkpU38qA==";
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Queue
            var qClient = storageAccount.CreateCloudQueueClient();
            cloudQueue = qClient.GetQueueReference("destination");
            cloudQueue.CreateIfNotExists();
            Scheduler();
        }

        public static void Scheduler()
        {
            int i = 0;
            while(true)
            {
                if(i==5)
                {
                    i = 0;
                }

                // Sleep for 1 minute
                Thread.Sleep(1 * 10 * 1000);

                switch(i)
                {
                    case 0:
                        WritetoQueue( "Amsterdam");
                        break;
                    case 1:
                        WritetoQueue("Bali");
                        break;
                    case 2:
                        WritetoQueue("London");
                        break;
                    case 3:
                        WritetoQueue("Paris");
                        break;
                    case 4:
                        WritetoQueue("Switzerland");
                        break;
                    default:
                        WritetoQueue("Amsterdam");
                        break;
                }

                i++;

            }
        }

        public static void WritetoQueue(string destination)
        {
            //var message = new ImageMessage(destination);
            var cloudQMessage = new CloudQueueMessage(destination);
            cloudQueue.AddMessage(cloudQMessage);
        }

        private static void BlobWriteTest()
        {
            //var connectionString = "DefaultEndpointsProtocol=https;AccountName=flightpricedata;AccountKey=AdNw3+GXlo1rX7621IEhapTT3NkQcHmwnX6xZxTiCd8lLws9ijc4nfW8ENo0uWsJEqfBxKpD+4kypL4/gP243Q==";
            var connectionString = "UseDevelopmentStorage=true";
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("test"); //("flightprices");

            CloudBlockBlob blob = container.GetBlockBlobReference("test.txt");
            using (var fileStream = System.IO.File.OpenRead(@"C:\Users\ankja\Desktop\testsabre.txt"))
            {
                blob.UploadFromStream(fileStream);
            }

            var text = blob.DownloadText();
            Console.WriteLine(text);
            Console.ReadLine();
        }

        private static async Task<bool> UnitTest()
        {
            var connectionString = "UseDevelopmentStorage=true";
            if (string.IsNullOrWhiteSpace(connectionString) == false)
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(new TimeSpan(0, 0, 2), 3);
                var workspaceTable = tableClient.GetTableReference("testtable");
                var tableExists = await workspaceTable.ExistsAsync();
                if (tableExists)
                {
                    try
                    {                        
                        var tableResultTask = await workspaceTable.ExecuteAsync(TableOperation.Retrieve<TableEntity>("abg", "abc"));
                        var isEnabled = tableResultTask.Result != null;
                        return isEnabled;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {

                    }
                }
            }

            return false;
        }
    }
}
