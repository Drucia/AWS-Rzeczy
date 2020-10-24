using Amazon.Runtime;
using AWS_Rzeczy.Models;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public interface IS3Service
    {
        Task<string> CreateBucketAsync(string bucketName);
        Task<AmazonWebServiceResponse> DeleteObject(string key, string bucketName);
        Task<CustomResponse> GetFromBucket(string key, string bucketName);
        Task<AmazonWebServiceResponse> UploadObjects(string key1, string key2, string bucketName);
    }
}