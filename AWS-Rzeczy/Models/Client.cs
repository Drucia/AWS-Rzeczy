using Amazon.DynamoDBv2.Model;
using AWS_Rzeczy.Services;
using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Generic;

namespace AWS_Rzeczy
{
    public class Client
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public static Client makeFromAWSResponse(Dictionary<string, AttributeValue> item)
        {
            return new Client
            {
                Login = item.GetValueOrDefault(DynamoDBService.LOGIN_COLUMN).S,
                Password = item.GetValueOrDefault(DynamoDBService.PASSWORD_COLUMN).S,
                Name = item.GetValueOrDefault(DynamoDBService.NAME_COLUMN).S,
                Age = IntegerType.FromString(item.GetValueOrDefault(DynamoDBService.AGE_COLUMN).N)
            };
        }
    }
}
