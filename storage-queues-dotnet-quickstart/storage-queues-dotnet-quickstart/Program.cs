 //------------------------------------------------------------------------------
//MIT License

//Copyright(c) 2017 Microsoft Corporation. All rights reserved.

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------


namespace storage_queues_dotnet_quickstart
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Queues - .NET Quickstart sample");
            Console.WriteLine();
            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            CloudStorageAccount storageAccount = null;
            CloudQueue queue = null;

            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable called storageconnectionstring, on the machine where the application is running.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
            string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudQueueClient that represents the queue endpoint for the storage account.
                    CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();

                    // Create a queue called 'quickstartqueues' and append a GUID value so that the queue name 
                    // is unique in your storage account. 
                    queue = cloudQueueClient.GetQueueReference("quickstartqueues-" + Guid.NewGuid().ToString());
                    await queue.CreateAsync();
                    Console.WriteLine("Created queue '{0}'", queue.Name);
                    Console.WriteLine();

                    // Create a message and add it to the queue. Set expiration time to 7 days.
                    CloudQueueMessage message = new CloudQueueMessage("Hello, World");
                    await queue.AddMessageAsync(message, new TimeSpan(7,0,0,0), null, null, null);
                    Console.WriteLine("Added message '{0}' to queue '{1}'", message.Id, queue.Name);
                    Console.WriteLine("Message insertion time: {0}", message.InsertionTime.ToString());
                    Console.WriteLine("Message expiration time: {0}", message.ExpirationTime.ToString());
                    Console.WriteLine();

                    // Peek at the message at the front of the queue. Peeking does not alter the message's 
                    // visibility, so that another client can still retrieve and process it. 
                    CloudQueueMessage peekedMessage = await queue.PeekMessageAsync();

                    // Display the ID and contents of the peeked message.
                    Console.WriteLine("Contents of peeked message '{0}': {1}", peekedMessage.Id, peekedMessage.AsString);
                    Console.WriteLine();

                    // Retrieve the message at the front of the queue. The message becomes invisible for 
                    // a specified interval, during which the client attempts to process it.
                    CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();

                    // Display the time at which the message will become visible again if it is not deleted.
                    Console.WriteLine("Message '{0}' becomes visible again at {1}", retrievedMessage.Id, retrievedMessage.NextVisibleTime);
                    Console.WriteLine();

                    //Process and delete the message within the period of invisibility.
                    await queue.DeleteMessageAsync(retrievedMessage);
                    Console.WriteLine("Processed and deleted message '{0}'", retrievedMessage.Id);
                    Console.WriteLine();
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from Azure Storage: {0}", ex.Message);
                }
                finally
                {
                    Console.WriteLine("Press any key to delete the sample queue.");
                    Console.ReadLine();
                    Console.WriteLine("Deleting the queue and any messages it contains...");
                    Console.WriteLine();
                    if (queue != null)
                    {
                        await queue.DeleteIfExistsAsync();
                    }
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }
    }
}
