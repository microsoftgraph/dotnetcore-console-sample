using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class OneDriveHelper
    {
        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        public OneDriveHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public OneDriveHelper(HttpClient httpClient)
        {
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }


        /// <summary>
        /// Take a file less than 4MB and upload it to the service
        /// </summary>
        /// <param name="fileToUpload">The file that we want to upload</param>
        /// <param name="uploadToSharePoint">Should we upload to SharePoint or OneDrive?</param>
        public async Task<DriveItem> UploadSmallFile(string fileToUpload, bool uploadToSharePoint)
        {
            DriveItem uploadedFile = null;
            FileStream fileStream = new FileStream(fileToUpload, FileMode.Open);

            if(uploadToSharePoint)
            {
                uploadedFile = await _graphClient.Sites["root"].Drive.Root.ItemWithPath(fileToUpload).Content.Request().PutAsync<DriveItem>(fileStream);
            }
            else
            {
                uploadedFile = (await _graphClient.Me.Drive.Root.ItemWithPath(fileToUpload).Content.Request().PutAsync<DriveItem>(fileStream ));
            } 
 
            return uploadedFile;
        }

        /// <summary>
        /// Take a file greater than 4MB and upload it to the service
        /// </summary>
        /// <param name="fileToUpload">The file that we want to upload</param>
        /// <param name="uploadToSharePoint">Should we upload to SharePoint or OneDrive?</param>
        public async Task<DriveItem> UploadLargeFile(string fileToUpload, bool uploadToSharePoint)
        {
            DriveItem uploadedFile = null;
            FileStream fileStream = new FileStream(fileToUpload, FileMode.Open);

            UploadSession uploadSession = null;

            // Do we want OneDrive for Business/Consumer or do we want a SharePoint Site?
            if (uploadToSharePoint)
            {
                uploadSession = await _graphClient.Sites["root"].Drive.Root.ItemWithPath(fileToUpload).CreateUploadSession().Request().PostAsync();
            }
            else
            {
                uploadSession = await _graphClient.Me.Drive.Root.ItemWithPath(fileToUpload).CreateUploadSession().Request().PostAsync();
            }

            if (uploadSession != null)
            {
                // Chunk size must be divisible by 320KiB, our chunk size will be slightly more than 1MB
                int maxSizeChunk = (320 * 1024) * 4;
                ChunkedUploadProvider uploadProvider = new ChunkedUploadProvider(uploadSession, _graphClient, fileStream, maxSizeChunk);
                var chunkRequests = uploadProvider.GetUploadChunkRequests();
                var exceptions = new List<Exception>();
                var readBuffer = new byte[maxSizeChunk];
                foreach (var request in chunkRequests)
                {
                    var result = await uploadProvider.GetChunkRequestResponseAsync(request, exceptions);

                    if (result.UploadSucceeded)
                    {
                        uploadedFile = result.ItemResponse;
                    }
                }
            }

            return uploadedFile;
        }
    }
}