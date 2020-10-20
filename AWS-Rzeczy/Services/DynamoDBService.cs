using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public class DynamoDBService
    {
        private AmazonDynamoDBClient _dynamoClient;

        public DynamoDBService(AmazonDynamoDBClient amazonDynamoDBClient)
        {
            _dynamoClient = amazonDynamoDBClient;
        }

        public async Task<string> createClientTableAsync()
        {
            var attributes = new List<AttributeDefinition>() { 
                new AttributeDefinition { AttributeName = "login", AttributeType = "String" },
                new AttributeDefinition { AttributeName = "password", AttributeType = "String" },
                new AttributeDefinition { AttributeName = "name", AttributeType = "String" },
                new AttributeDefinition { AttributeName = "age", AttributeType = "Number" },
            };
            var req = new CreateTableRequest
            {
                TableName = "Client",
                AttributeDefinitions = attributes,
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10000L, WriteCapacityUnits = 10000L }
            };

            try
            {
                var response = await _dynamoClient.CreateTableAsync(req);

                return response.TableDescription.ToString();
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
