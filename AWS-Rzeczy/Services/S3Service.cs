using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.IO;
using Amazon.Runtime;
using AWS_Rzeczy.Models;

namespace AWS_Rzeczy.Services
{
    public class S3Service : IS3Service
    {
        // Specify your bucket region (an example region is shown).
        private IAmazonS3 s3Client;

        public S3Service(IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
        }
        [Obsolete]

        #region Create Bucket
        public async Task<CustomResponse> CreateBucketAsync(string bucketName)
        {
            CustomResponse bucketLocation = new CustomResponse();
            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistAsync(s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                }
                // Retrieve the bucket location.
                bucketLocation.Response = await FindBucketLocationAsync(s3Client, bucketName);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                bucketLocation.Response = string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                bucketLocation.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return bucketLocation;
        }


        async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            string bucketLocation;
            var request = new GetBucketLocationRequest()
            {
                BucketName = bucketName
            };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            bucketLocation = response.Location.ToString();
            return bucketLocation;
        }

        #endregion

        #region Upload Object

        public async Task<CustomResponse> UploadObjects(string fileName, string fileContent, string bucketName)
        {
            CustomResponse response = new CustomResponse();
            try
            {
                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    ContentType = "text/plain",
                    ContentBody = fileContent
                };

                PutObjectResponse response1 = await s3Client.PutObjectAsync(putRequest1);
                response.Response = string.Format("Uploaded object {0}, status code: {1}", fileName, response1.HttpStatusCode.ToString());
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                response.Response = string.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                response.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            AmazonWebServiceResponse badResponse = new AmazonWebServiceResponse();
            badResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            return response;
        }

        #endregion

        #region GetList
        public async Task<string> GetList(string bucketName)
        {
            string items = "";
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await s3Client.ListObjectsV2Async(request);

                    // Process the response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        items += ("key = {0} size = {1} modified = {2}\n",
                            entry.Key, entry.Size, entry.LastModified.ToShortDateString());
                    }
                    Console.WriteLine("Next Continuation Token: {0}", response.NextContinuationToken);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);

                return items;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine("S3 error occurred. Exception: " + amazonS3Exception.ToString());
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                Console.ReadKey();
            }
            if (items == "")
                items = "No files in a bucket";
            return items;
        }
        #endregion

        #region Get Object
        public async Task<CustomResponse> GetFromBucket(string fileName, string bucketName)
        {
            string contents;
            //if using .NET Core, make sure to use await keyword and async method
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            ResponseHeaderOverrides responseHeaders = new ResponseHeaderOverrides();
            responseHeaders.CacheControl = "No-cache";
            responseHeaders.ContentDisposition = "attachment; filename=testing.txt";

            request.ResponseHeaderOverrides = responseHeaders;


            // Issue request and remember to dispose of the response
            using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
            {
                using (StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    contents = reader.ReadToEnd();
                    Console.WriteLine("Object - " + response.Key);
                    Console.WriteLine("Version Id - " + response.VersionId);
                    Console.WriteLine("Contents - " + contents);
                }
            }
            return new CustomResponse { Response = contents };
        }
        #endregion

        #region Delete Object
        public async Task<CustomResponse> DeleteObject(string fileName, string bucketName)
        {
            CustomResponse response = new CustomResponse();
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                Console.WriteLine("Deleting an object");
                DeleteObjectResponse result = await s3Client.DeleteObjectAsync(deleteObjectRequest);

                response.Response = string.Format("Deleted object {0} (if existed), status code: {1}", fileName, result.HttpStatusCode.ToString());
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                response.Response = string.Format("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                response.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return response;
        }
        #endregion
    }
}
