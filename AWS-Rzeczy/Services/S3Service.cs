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
        public async Task<string> CreateBucketAsync(string bucketName)
        {
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
                string bucketLocation = await FindBucketLocationAsync(s3Client, bucketName);
                return bucketLocation;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return "Error";
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

        public async Task<AmazonWebServiceResponse> UploadObjects(string key1, string key2, string bucketName)
        {
            try
            {
                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key1,
                    ContentBody = "Hello cruel world"
                };

                PutObjectResponse response1 = await s3Client.PutObjectAsync(putRequest1);
                // 2. Put the object-set ContentType and add metadata.
                var putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key2,
                    FilePath = "/",
                    ContentType = "text/plain"
                };
                putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");

                return response1;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
            AmazonWebServiceResponse badResponse = new AmazonWebServiceResponse();
            badResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            return badResponse;
        }

        #endregion

        #region Get Object
        public async Task<CustomResponse> GetFromBucket(string key, string bucketName)
        {
            string contents;
            //if using .NET Core, make sure to use await keyword and async method
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
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
        public async Task<AmazonWebServiceResponse> DeleteObject(string key, string bucketName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                Console.WriteLine("Deleting an object");
                var result = await s3Client.DeleteObjectAsync(deleteObjectRequest);
                return result;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            AmazonWebServiceResponse badResponse = new AmazonWebServiceResponse();
            badResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            return badResponse;
        }
        #endregion
    }
}
