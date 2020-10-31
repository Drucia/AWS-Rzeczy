using Amazon.Runtime;
using AWS_Rzeczy.Models;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public interface IS3Service
    {
        Task<CustomResponse> CreateBucketAsync(string bucketName);
        Task<CustomResponse> DeleteObject(string key, string bucketName);
        Task<string> GetList(string bucketName);
        Task<CustomResponse> GetFromBucket(string key, string bucketName);
        Task<CustomResponse> UploadObjects(string key1, string key2, string bucketName);
    }
}