using Amazon.CloudWatch.Model;
using AWS_Rzeczy.Models;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public interface ICloudWatchService
    {
        Task<CustomResponse> DeleteAlarm(WatchRequestBody requestBody);
        Task<CustomResponse> DescribeAlarm(WatchRequestBody requestBody);
        Task<CustomResponse> PutMetricAlarm(WatchRequestBody requestBody);
    }
}