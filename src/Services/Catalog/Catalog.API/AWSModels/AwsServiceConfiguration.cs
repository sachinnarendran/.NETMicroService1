using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.AWSModels
{
    public class AwsServiceConfiguration
    {
        public AwsBucketConfiguration AwsBucketConfiguration { get; set; }
    }

    public class AwsBucketConfiguration
    {
        public string BucketName { get; set; }
    }
}
