using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudMigrator.Azure
{
    public class BlobMigrationService
    {
        delegate void BeginUploadFinished(IAsyncResult result);

        public void UploadZipFileAsync(string containerName, Stream zipStream)
        {
            string connString = "UseDevelopmentStorage=true";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            //CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            //// Create the container if it doesn't already exist.
            //container.CreateIfNotExistsAsync();

            //container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            /* BeginUploadFinished beginUploadFinished = delegate (IAsyncResult result)
             {
                 blob.EndUploadFromStream(result);
                 Trace.WriteLine("EndUploadFromStream", "Information");
                 stream.Close();
                 stream.Dispose();
             };*/
            using (var reader = new StreamReader(zipStream))
            {
                var stream = reader.BaseStream;
                stream.Seek(0, SeekOrigin.Begin);
            //using (var ms = new MemoryStream())
            //{

                //zipStream.CopyTo(ms);
                //var data = ms.ToArray();
                var collection = new FileProcessor.ZipFileService().ExtractToStreamCollection(stream).Result;
                containerName = collection.CollectionName.ToLower();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                // Create the container if it doesn't already exist.
                var containerRes = container.CreateIfNotExistsAsync();
                containerRes.Wait();

                container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                foreach (var item in collection)
                {
                    CloudBlockBlob blob = container.GetBlockBlobReference(item.FileName);
                    var strm = new MemoryStream(item.Content);

                    var res = blob.UploadFromByteArrayAsync(item.Content, 0, item.Content.Length);
                    res.Wait();
                }
            }
        }
    }
}
