using System.Collections;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using AWS_Rzeczy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
        [HttpPost]
        [Route("createtable/{name}")]
        public async Task<ActionResult> CreateClientTable(string name)
        {
            var res = await _dynamoService.createClientTableAsync(name);
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            return BadRequest(new {msg = res.ErrorMsg});
        }

        // Script 1 - Druciak
        [HttpDelete]
        [Route("deletetable/{name}")]
        public async Task<ActionResult> DeleteClientTable(string name)
        {
            var res = await _dynamoService.deleteClientTableIfExistAsync(name);
            if (res.WasSuccessful)
                return Ok(new { msg = res.Value });
            return BadRequest(new { msg = res.ErrorMsg });
        }

        [HttpPost]
        [Route("createclients")]
        public async Task<ActionResult> CreateClients([FromBody] IEnumerable<Client> clients)
        {
            //var res = await _dynamoService.createClientList(clients);
            //if (res.WasSuccessful)
            //    return Ok(res.Value);
            return BadRequest(new { msg = "a" });
        }

        [HttpGet]
        [Route("clients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            //var res = await _dynamoService.deleteClientTableIfExistAsync(name);
            //if (res.WasSuccessful)
            //    return Ok(res.Value);
            return BadRequest(new { msg = res.ErrorMsg });
        }

        [HttpPost]
        [Route("clients")]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            //var res = await _dynamoService.deleteClientTableIfExistAsync(name);
            //if (res.WasSuccessful)
            //    return Ok(res.Value);
            return BadRequest(new { msg = res.ErrorMsg });
        }

        [HttpPost]
        [Route("clients/{login}")]
        public async Task<ActionResult<Client>> UpdateClient(string login, [FromBody] Client client)
        {
            //var res = await _dynamoService.deleteClientTableIfExistAsync(name);
            //if (res.WasSuccessful)
            //    return Ok(res.Value);
            client.Age += 1;
            return BadRequest(new { msg = res.ErrorMsg });
        }

        [HttpDelete]
        [Route("clients/{login}")]
        public async Task<ActionResult<string>> UpdateClient(string login)
        {
            //var res = await _dynamoService.deleteClientTableIfExistAsync(name);
            //if (res.WasSuccessful)
            //    return Ok(res.Value);
            return BadRequest(new { msg = res.ErrorMsg });
        }
    }
}
