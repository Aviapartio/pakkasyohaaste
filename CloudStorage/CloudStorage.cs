using System;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;


namespace Pakkasyöhaaste.CloudStorage
{
    // With thanks to Sena Kılıçarslan
    // See https://medium.com/net-core/using-google-cloud-storage-in-asp-net-core-74f9c5ee55f5
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            var confval = configuration.GetValue<string>("GoogleCredentialsFile");
            googleCredential = GoogleCredential.FromFile(confval);

            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Seek(0,SeekOrigin.Begin);
                Console.WriteLine($"Pituus {memoryStream.Length}");
                var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage,null, memoryStream);
                return dataObject.MediaLink;
            }
        }

        public async Task<string> UploadStringAsync(String content, string fileNameForStorage)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, stream);
            return dataObject.MediaLink;
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }
    }
}
