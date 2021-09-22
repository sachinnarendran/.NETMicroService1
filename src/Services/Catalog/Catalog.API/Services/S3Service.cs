using Amazon.S3;
using Amazon.S3.Model;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.API.Services
{
    public class S3Service:IS3Service
    {
        AmazonS3Client amazonClient = new AmazonS3Client();
        private readonly IAmazonS3 amazonS3;
        private readonly IConfiguration configuration;

        public S3Service(IAmazonS3 _amazonS3,IConfiguration _configuration)
        {
            amazonS3 = _amazonS3;
            configuration = _configuration;
        }
        public async Task<bool> AddContentToS3(Product product)
        {
            var request = new PutObjectRequest
            {
                BucketName = configuration.GetValue<string>("ServiceConfiguration:BucketName"),
                Key = product.Id,
                ContentType = "application/json",
                ContentBody = JsonSerializer.Serialize(product)
            };
            var response = await amazonS3.PutObjectAsync(request);
            return true;            
        }

        public async Task<Product> GetProductFromS3(string Id)
        {
            var response = await amazonS3.GetObjectAsync(configuration.GetValue<string>("ServiceConfiguration:BucketName"),Id);
            StreamReader reader = new StreamReader(response.ResponseStream);
            var content = reader.ReadToEnd();
            var product = JsonSerializer.Deserialize<Product>(content);
            return product;
        }
        
        public async Task<IEnumerable<Product>> GetAllProductsFromS3()
        {
            List<Product> products = new List<Product>();
            ListObjectsRequest listObjectsRequest = new ListObjectsRequest 
            {
                BucketName = configuration.GetValue<string>("ServiceConfiguration:BucketName")
            };
            var listObjectResponse = await amazonS3.ListObjectsAsync(listObjectsRequest);
            foreach(var item in listObjectResponse.S3Objects)
            {
                var fileContent = await amazonS3.GetObjectAsync(configuration.GetValue<string>("ServiceConfiguration:BucketName"), item.Key);
                StreamReader reader = new StreamReader(fileContent.ResponseStream);
                var content = reader.ReadToEnd();
                var productDetail = JsonSerializer.Deserialize<Product>(content);
                products.Add(productDetail);
            }
            return products;
        }

    }
}
