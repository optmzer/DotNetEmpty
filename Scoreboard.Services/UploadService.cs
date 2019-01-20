using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Scoreboards.Data;

/**
 * Manages upload of User Profile Picture to Azure Blob Sotrage
 * 
 */ 

namespace Scoreboards.Services
{
    public class UploadService : IUpload
    {
        public CloudBlobContainer GetStorageContainer(string blobStorageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            return blobClient.GetContainerReference("userimages");
        }
    }
}
