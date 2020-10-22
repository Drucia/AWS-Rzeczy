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
        private IAmazonDynamoDB _dynamoClient;

        public DynamoDBService(IAmazonDynamoDB dynamoClient)
        {
            _dynamoClient = dynamoClient;
        }

        public IAmazonDynamoDB DynamoClient { get; }

        public async Task<string> createClientTableAsync()
        {
            var attributes = new List<AttributeDefinition>() { 
                new AttributeDefinition { AttributeName = "login", AttributeType = "S" }
            };
            var req = new CreateTableRequest
            {
                TableName = "Client",
                KeySchema = new List<KeySchemaElement>() {
                    new KeySchemaElement { AttributeName = "login", KeyType = KeyType.HASH }},
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
