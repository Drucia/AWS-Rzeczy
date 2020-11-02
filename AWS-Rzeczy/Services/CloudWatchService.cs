using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using AWS_Rzeczy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public class CloudWatchService : ICloudWatchService
    {

        private IAmazonCloudWatch _cloudWatchClient;

        public CloudWatchService(IAmazonCloudWatch cloudWatchClient)
        {
            _cloudWatchClient = cloudWatchClient;
        }

        public async Task<CustomResponse> PutMetricAlarm(WatchRequestBody requestBody)
        {
            CustomResponse result = new CustomResponse(); ;
            try
            {
                await _cloudWatchClient.PutMetricAlarmAsync
                        (new PutMetricAlarmRequest
                        {
                            AlarmName = requestBody.name,
                            ComparisonOperator = ComparisonOperator.GreaterThanThreshold,
                            EvaluationPeriods = 1,
                            MetricName = "CPUUtilization",
                            Namespace = "AWS/EC2",
                            Period = 60,
                            Statistic = Statistic.Average,
                            Threshold = 20.0,
                            ActionsEnabled = true,
                            AlarmActions = new List<string> { "arn:aws:swf:us-east-1:" + "customerAccount" + ":action/actions/AWS_EC2.InstanceId.Reboot/1.0" },
                            AlarmDescription = "Alarm when server CPU exceeds 20%",
                            Dimensions = new List<Dimension>
                                {
                                        new Dimension { Name = "InstanceId", Value = "INSTANCE_ID" }
                                },
                            Unit = StandardUnit.Seconds
                        }
                        );
                result.Response = "Alarm created";
            }
            catch (AmazonCloudWatchException e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return result;
        }

        public async Task<CustomResponse> DeleteAlarm(WatchRequestBody requestBody)
        {
            CustomResponse result = new CustomResponse();
            try
            {
                await _cloudWatchClient.DeleteAlarmsAsync
                    (new DeleteAlarmsRequest 
                        { 
                            AlarmNames = new List<string> { requestBody.name } 
                        }
                    );
                    result.Response = "Alarm deleted";
            }
            catch (AmazonCloudWatchException e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return result;
        }

        //limiting the alarms that are returned to those with a state of INSUFFICIENT_DATA
        public async Task<CustomResponse> DescribeAlarm(WatchRequestBody requestBody)
        {
            CustomResponse result = new CustomResponse(); ;
            try
            {
                var request = new DescribeAlarmsRequest();
                request.StateValue = "INSUFFICIENT_DATA";
                request.AlarmNames = new List<string> { requestBody.name };
                do
                {
                    var response = await _cloudWatchClient.DescribeAlarmsAsync(request);
                    foreach (var alarm in response.MetricAlarms)
                    {
                        Console.WriteLine(alarm.AlarmName);
                    }
                    request.NextToken = response.NextToken;
                } while (request.NextToken != null);
                result.Response = "Alarm described";
            }
            catch (AmazonCloudWatchException e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                result.Response = string.Format("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return result;
        }


    }
}

