using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using AWS_Rzeczy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AWS_Rzeczy.Controllers
{
    [ApiController]
    [Route("api")]
    public class AWSController : ControllerBase
    {
        private readonly ILogger<AWSController> _logger;
        private readonly IAmazonDynamoDB _dynamoClient;

        // -- SERVICES -- //
        private DynamoDBService _dynamoService;

        public AWSController(ILogger<AWSController> logger, IAmazonDynamoDB amazonDynamoDBClient)
        {
            _logger = logger;
            _dynamoClient = amazonDynamoDBClient;
            _dynamoService = new DynamoDBService(_dynamoClient);
        }

        // Script 1 - Druciak
        [HttpGet]
        [Route("createtable")]
        public async Task<string> CreateClientTable()
        {
            return await _dynamoService.createClientTableAsync();
        }

        //[HttpGet]
        //[Route("druciak/dynamodbcrud")]
        //public IEnumerable<Client> GetAll()
        //{

        //    //return Enumerable.Range(1, 5).Select(index => new Client
        //    //{
        //    //    Date = DateTime.Now.AddDays(index),
        //    //    TemperatureC = rng.Next(-20, 55),
        //    //    Summary = Summaries[rng.Next(Summaries.Length)]
        //    //})
        //    //.ToArray();
        //}

        //private static async Task<SessionAWSCredentials> GetTemporaryCredentialsAsync()
        //{
        //    using (var stsClient = new AmazonSecurityTokenServiceClient())
        //    {
        //        var getSessionTokenRequest = new GetSessionTokenRequest
        //        {
        //            DurationSeconds = 7200 // seconds
        //        };

        //        GetSessionTokenResponse sessionTokenResponse =
        //                      await stsClient.GetSessionTokenAsync(getSessionTokenRequest);

        //        Credentials credentials = sessionTokenResponse.Credentials;

        //        var sessionCredentials =
        //            new SessionAWSCredentials(credentials.AccessKeyId,
        //                                      credentials.SecretAccessKey,
        //                                      credentials.SessionToken);
        //        return sessionCredentials;
        //    }
        //}
    }
}
