using Microsoft.WindowsAzure.Storage.Blob;

namespace Scoreboards.Data
{
    public interface IUpload
    {
        CloudBlobContainer GetStorageContainer(string blobStorageConnectionString);
    }
}
