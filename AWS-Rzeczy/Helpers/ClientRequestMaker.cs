using Amazon.DynamoDBv2.Model;
using AWS_Rzeczy.Services;
using System.Collections.Generic;

namespace AWS_Rzeczy.Models
{
    public class ClientRequestMaker
    {
        public static WriteRequest makePutRequestFromClient(Client client)
        {
            return new WriteRequest
            {
                PutRequest = new PutRequest
                {
                    Item = new Dictionary<string, AttributeValue>
                    {
                        { DynamoDBService.LOGIN_COLUMN, new AttributeValue { S = client.Login } },
                        { DynamoDBService.PASSWORD_COLUMN, new AttributeValue { S = client.Password } },
                        { DynamoDBService.NAME_COLUMN, new AttributeValue { S = client.Name } },
                        { DynamoDBService.AGE_COLUMN, new AttributeValue { N = client.Age.ToString() } }
                    }
                }
            };
        }
    }
}
