using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWS_Rzeczy.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Services
{
    public class DynamoDBService
    {
        public const string TABLE_NAME = "CLIENT";
        public const string LOGIN_COLUMN = "LOGIN";
        public const string PASSWORD_COLUMN = "PASSWORD";
        public const string NAME_COLUMN = "NAME";
        public const string AGE_COLUMN = "AGE";

        private IAmazonDynamoDB _dynamoClient;

        public DynamoDBService(IAmazonDynamoDB dynamoClient)
        {
            _dynamoClient = dynamoClient;
        }

        public IAmazonDynamoDB DynamoClient { get; }

        public async Task<Holder<string>> createClientTableAsync()
        {
            var attributes = new List<AttributeDefinition>() { 
                new AttributeDefinition { AttributeName = LOGIN_COLUMN, AttributeType = "S" }
            };
            var req = new CreateTableRequest
            {
                TableName = TABLE_NAME,
                KeySchema = new List<KeySchemaElement>() {
                    new KeySchemaElement { AttributeName = LOGIN_COLUMN, KeyType = KeyType.HASH }},
                AttributeDefinitions = attributes,
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
            };

            try
            {
                var response = await _dynamoClient.CreateTableAsync(req);
                Thread.Sleep(13000);

                return Holder<string>.Success(response.TableDescription.TableName);

            }
            catch (ResourceInUseException inUse)
            {
                Thread.Sleep(3000);
                return Holder<string>.Fail("repeat");
            }
            catch (Exception ex)
            {
                return Holder<string>.Fail(ex.Message);
            }
        }
        public async Task<Holder<string>> deleteClientTableIfExistAsync()
        {
            try
            {
                var response = await _dynamoClient.DeleteTableAsync(new DeleteTableRequest { TableName = TABLE_NAME });

                return Holder<string>.Success($"Deleted existed {response.TableDescription.TableName} table.");
            }
            catch (ResourceNotFoundException ex)
            {
                return Holder<string>.Success("Table don't exist so deleting is skip");
            }
            catch (Exception e)
            {
                return Holder<string>.Fail(e.Message);
            }
        }
        public async Task<Holder<IEnumerable<Client>>> getAllClients()
        {
            var request = new BatchGetItemRequest
            {
                RequestItems = new Dictionary<string, KeysAndAttributes>()
                {
                    { TABLE_NAME, new KeysAndAttributes()
                        {
                            Keys = new List<Dictionary<string, AttributeValue>>()
                            {
                                new Dictionary<string, AttributeValue>()
                                {
                                    { "Name", new AttributeValue { S = "DynamoDB" } }
                                }
                            }
                        }
                    }
                }
            };

            try
            {
                var response = await _dynamoClient.BatchGetItemAsync(request);
                var items = response.Responses[TABLE_NAME];
                

                return Holder<IEnumerable<Client>>.Success(items.ConvertAll<Client>(item => Client.makeFromAWSResponse(item)));
            }
            catch (Exception ex)
            {
                return Holder<IEnumerable<Client>>.Fail(ex.Message);
            }
        }

        public async Task<Holder<string>> createClientList(IEnumerable<Client> clients)
        {
            var request = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>
                {
                    {
                        TABLE_NAME, clients.ToList().ConvertAll<WriteRequest>(client => ClientRequestMaker.makePutRequestFromClient(client))
                    }
                }
            };

            try
            {
                var response = await _dynamoClient.BatchWriteItemAsync(request);

                return Holder<string>.Success("Added start clients list.");
            }
            catch (Exception ex)
            {
                return Holder<string>.Fail(ex.Message);
            }
        }

        public async Task<Holder<Client>> addClient(Client client)
        {
            var request = new PutItemRequest
            {
                TableName = TABLE_NAME,
                Item = ClientRequestMaker.makePutRequestFromClient(client).PutRequest.Item
            };

            try
            {
                var response = await _dynamoClient.PutItemAsync(request);
                var clientResponse = response.Attributes;

                return Holder<Client>.Success(Client.makeFromAWSResponse(clientResponse));
            } catch (Exception ex)
            {
                return Holder<Client>.Fail(ex.Message);
            } 
        }

        public async Task<Holder<Client>> editClient(string login, Client client)
        {
            var request = new UpdateItemRequest
            {
                Key = new Dictionary<string, AttributeValue> // key to update
                {
                    { LOGIN_COLUMN, new AttributeValue { S = login } }
                },
                TableName = TABLE_NAME,
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#P", PASSWORD_COLUMN},
                    {"#N", NAME_COLUMN},
                    {"#A", AGE_COLUMN},
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":newpwd", new AttributeValue { S = client.Password }},
                    {":newname", new AttributeValue { S = client.Name }},
                    {":newage", new AttributeValue { N = client.Age.ToString() }},
                },
                UpdateExpression = "SET #P = :newpwd SET #N = :newname SET #A = :newage",
            };

            try
            {
                var response = await _dynamoClient.UpdateItemAsync(request);
                var clientResponse = response.Attributes;

                return Holder<Client>.Success(Client.makeFromAWSResponse(clientResponse));
            }
            catch (Exception ex)
            {
                return Holder<Client>.Fail(ex.Message);
            }
        }

        public async Task<Holder<Client>> deleteClient(string login)
        {
            var request = new DeleteItemRequest
            {
                TableName = TABLE_NAME,
                Key = new Dictionary<string, AttributeValue>()
                {
                    {
                        LOGIN_COLUMN, new AttributeValue { S = login }
                    }
                },
            };

            try
            {
                var response = await _dynamoClient.DeleteItemAsync(request);
                var clientResponse = response.Attributes;

                return Holder<Client>.Success(Client.makeFromAWSResponse(clientResponse));
            } catch (Exception ex)
            {
                return Holder<Client>.Fail(ex.Message);
            }
        }
    }
}
