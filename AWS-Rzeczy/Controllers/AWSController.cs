using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using AWS_Rzeczy.Models;
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
        private readonly IS3Service _s3Service;


        // -- SERVICES -- //
        private DynamoDBService _dynamoService;


        public AWSController(ILogger<AWSController> logger, IAmazonDynamoDB amazonDynamoDBClient, IS3Service s3Service)
        {
            _logger = logger;
            _dynamoClient = amazonDynamoDBClient;
            _s3Service = s3Service;
            _dynamoService = new DynamoDBService(_dynamoClient);
        }

        #region Ola D
        // Script 1 - Druciak
        [HttpPost]
        [Route("clients/createtable")]
        public async Task<ActionResult> CreateClientTable()
        {
            var res = await _dynamoService.createClientTableAsync();
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            else if (res.ErrorMsg.Contains("repeat"))
                return await CreateClientTable();
            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpDelete]
        [Route("clients/deletetable")]
        public async Task<ActionResult> DeleteClientTable()
        {
            var res = await _dynamoService.deleteClientTableIfExistAsync();
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpPost]
        [Route("clients/createclients")]
        public async Task<ActionResult> CreateClients([FromBody] IEnumerable<Client> clients)
        {
            var res = await _dynamoService.createClientList(clients);
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpGet]
        [Route("clients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            var res = await _dynamoService.getAllClients();
            if (res.WasSuccessful)
                return Ok(res.Value);
            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpPost]
        [Route("clients")]
        public async Task<ActionResult> CreateClient([FromBody] Client client)
        {
            var res = await _dynamoService.addClient(client);
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpPost]
        [Route("clients/{login}")]
        public async Task<ActionResult<Client>> UpdateClient(string login, [FromBody] Client client)
        {
            var res = await _dynamoService.editClient(login, client);
            if (res.WasSuccessful)
                return Ok(res.Value);

            return BadRequest(new { msg = res.ErrorMsg });
        }

        // Script 1 - Druciak
        [HttpDelete]
        [Route("clients/{login}")]
        public async Task<ActionResult<Client>> DeleteClient(string login)
        {
            var res = await _dynamoService.deleteClient(login);
            if (res.WasSuccessful)
                return Ok(res.Value);

            return BadRequest(new { msg = res.ErrorMsg });
        }
        #endregion

        #region Tomek
        [HttpPost]
        [Route("s3/create")]
        public async Task<CustomResponse> CreateBucket([FromBody] S3RquestBody body)
        {
            return await _s3Service.CreateBucketAsync(body.bucketName);
        }


        [HttpPost]
        [Route("s3/delete")]
        public async Task<CustomResponse> DeleteFromBucket([FromBody] S3RquestBody body)
        {
            return await _s3Service.DeleteObject(body.fileName, body.bucketName);
        }

        [HttpPost]
        [Route("s3/getlist")]
        public async Task<CustomResponse> GetListFromBucket([FromBody] S3RquestBody body)
        {
            return new CustomResponse() { Response = await _s3Service.GetList(body.bucketName) };
        }

        [HttpPost]
        [Route("s3/get")]
        public async Task<CustomResponse> GetFromBucket([FromBody] S3RquestBody body)
        {
            return await _s3Service.GetFromBucket(body.fileName, body.bucketName);
        }
        [HttpPost]
        [Route("s3/upload")]
        public async Task<CustomResponse> UploadToBucket([FromBody] S3RquestBody body)
        {
            return await _s3Service.UploadObjects(body.fileName, body.fileContent, body.bucketName);
        }
        #endregion

        #region Ola G

        #endregion
    }
}
