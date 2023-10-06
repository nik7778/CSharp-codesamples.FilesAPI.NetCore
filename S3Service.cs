using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Invoicing.Infrastructure.Services
{
    public class S3Service: IS3Service
    {
        private string bucketName;
        private string awsAccessKeyId;
        private string awsSecretAccessKey;
        private string awsPath;
        private RegionEndpoint regionEndpoint;

        public S3Service(string _bucketName, string _awsAccessKeyId, string _awsSecretAccessKey, string _regionEndpoint, string _awsPath)
        {
            bucketName = _bucketName;
            awsAccessKeyId = _awsAccessKeyId;
            awsSecretAccessKey = _awsSecretAccessKey;
            regionEndpoint = GetRegionEndpoint(_regionEndpoint);
            awsPath = _awsPath;
        }     

        public async Task<string> UploadFileToS3Bucket(IFormFile file, string fileName)
        {
            using (var client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, regionEndpoint))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = fileName,
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.NoACL
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    string filePath = awsPath + fileName;

                    return filePath;
                }
            }
        }  
        
        public async Task DeleteFileFromS3Bucket(string name)
        {
            using (var client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, regionEndpoint))
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = name
                };

                await client.DeleteObjectAsync(deleteObjectRequest);
            }
        }

        #region helpers

        private RegionEndpoint GetRegionEndpoint(string regionEndpoint)
        {
            switch (regionEndpoint)
            {
                case "USEast1":
                    return RegionEndpoint.USEast1;
                case "CACentral1":
                    return RegionEndpoint.CACentral1;
                case "CNNorthWest1":
                    return RegionEndpoint.CNNorthWest1;
                case "CNNorth1":
                    return RegionEndpoint.CNNorth1;
                case "USGovCloudWest1":
                    return RegionEndpoint.USGovCloudWest1;
                case "USGovCloudEast1":
                    return RegionEndpoint.USGovCloudEast1;
                case "SAEast1":
                    return RegionEndpoint.SAEast1;
                case "APSoutheast1":
                    return RegionEndpoint.APSoutheast1;
                case "APSouth1":
                    return RegionEndpoint.APSouth1;
                case "APNortheast3":
                    return RegionEndpoint.APNortheast3;
                case "APSoutheast2":
                    return RegionEndpoint.APSoutheast2;
                case "APNortheast1":
                    return RegionEndpoint.APNortheast1;
                case "USEast2":
                    return RegionEndpoint.USEast2;
                case "APNortheast2":
                    return RegionEndpoint.APNortheast2;
                case "USWest2":
                    return RegionEndpoint.USWest2;
                case "EUNorth1":
                    return RegionEndpoint.EUNorth1;
                case "USWest1":
                    return RegionEndpoint.USWest1;
                case "EUWest2":
                    return RegionEndpoint.EUWest2;
                case "EUWest3":
                    return RegionEndpoint.EUWest3;
                case "EUCentral1":
                    return RegionEndpoint.EUCentral1;
                case "EUWest1":
                    return RegionEndpoint.EUWest1;
            }

            return RegionEndpoint.USEast1;
        }

        #endregion
    }
}
