using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
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
        private readonly AmazonDynamoDBConfig _clientConfig;

        // -- SERVICES -- //
        private DynamoDBService _dynamoService;

        public AWSController(ILogger<AWSController> logger)
        {
            _logger = logger;
            _clientConfig = new AmazonDynamoDBConfig();
            _clientConfig.ServiceURL = "http://localhost:8000";
        }

        // Script 1 - Druciak
        [HttpGet]
        [Route("createtable")]
        public async Task<string> CreateClientTable()
        {
            SessionAWSCredentials tempCredentials = await GetTemporaryCredentialsAsync();
            _dynamoService = new DynamoDBService(new AmazonDynamoDBClient(tempCredentials, _clientConfig));

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

        private static async Task<SessionAWSCredentials> GetTemporaryCredentialsAsync()
        {
            using (var stsClient = new AmazonSecurityTokenServiceClient())
            {
                var getSessionTokenRequest = new GetSessionTokenRequest
                {
                    DurationSeconds = 7200 // seconds
                };

                GetSessionTokenResponse sessionTokenResponse =
                              await stsClient.GetSessionTokenAsync(getSessionTokenRequest);

                Credentials credentials = sessionTokenResponse.Credentials;

                var sessionCredentials =
                    new SessionAWSCredentials(credentials.AccessKeyId,
                                              credentials.SecretAccessKey,
                                              credentials.SessionToken);
                return sessionCredentials;
            }
        }
    }
}
