using System;
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

            if (uploadToSharePoint)
            {
                uploadedFile = await _graphClient.Sites["root"].Drive.Root.ItemWithPath(fileToUpload).Content.Request().PutAsync<DriveItem>(fileStream);
            }
            else
            {
                uploadedFile = (await _graphClient.Me.Drive.Root.ItemWithPath(fileToUpload).Content.Request().PutAsync<DriveItem>(fileStream));
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
                var fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, fileStream, maxSizeChunk);

                // Create a callback that is invoked after each slice is uploaded
                IProgress<long> progress = new Progress<long>(prog =>
                {
                    Console.WriteLine($"Uploaded {prog} bytes of {fileStream.Length} bytes");
                });

                try
                {
                    // Upload the file
                    var uploadResult = await fileUploadTask.UploadAsync(progress);

                    if (uploadResult.UploadSucceeded)
                    {
                        uploadedFile = uploadResult.ItemResponse;
                    }
                    else
                    {
                        Console.WriteLine("Upload failed");
                    }
                }
                catch (ServiceException ex)
                {
                    Console.WriteLine($"Error uploading: {ex.ToString()}");
                }
            }

            return uploadedFile;
        }
    }
}