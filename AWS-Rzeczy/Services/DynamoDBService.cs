using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWS_Rzeczy.Models;
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

        public async Task<Holder<string>> createClientTableAsync(string name)
        {
            var attributes = new List<AttributeDefinition>() { 
                new AttributeDefinition { AttributeName = "login", AttributeType = "S" }
            };
            var req = new CreateTableRequest
            {
                TableName = name.ToUpper(),
                KeySchema = new List<KeySchemaElement>() {
                    new KeySchemaElement { AttributeName = "login", KeyType = KeyType.HASH }},
                AttributeDefinitions = attributes,
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10000L, WriteCapacityUnits = 10000L }
            };

            try
            {
                var response = await _dynamoClient.CreateTableAsync(req);

                return Holder<string>.Success(response.TableDescription.ToString());

            } catch (Exception ex)
            {
                return Holder<string>.Fail(ex.Message);
            }
        }
        public async Task<Holder<string>> deleteClientTableIfExistAsync(string name)
        {
            try
            {
                var response = await _dynamoClient.DeleteTableAsync(new DeleteTableRequest { TableName = name.ToUpper() });

                return Holder<string>.Success("Created " + response.TableDescription.TableName + " table./n");
            }
            catch (ResourceNotFoundException ex)
            {
                return Holder<string>.Success("Table don't exist");
            }
            catch (Exception e)
            {
                return Holder<string>.Fail(e.Message);
            }
        }

        //public async Task<Holder<Client>> addClient(Client client)
        //{

        //}

        //public async Task<Holder<Client>> editClient(Client client)
        //{

        //}

        //public async Task<Holder<Client>> deleteClient(Client client)
        //{

        //}

    }
}
