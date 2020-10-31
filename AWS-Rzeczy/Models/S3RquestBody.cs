using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWS_Rzeczy.Models
{
    public class S3RquestBody
    {
        public string bucketName { get; set; }
        public string fileName { get; set; }
        public string fileContent { get; set; }
    }
}
