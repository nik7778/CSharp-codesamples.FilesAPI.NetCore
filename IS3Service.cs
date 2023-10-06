using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoicing.Infrastructure.Services
{
    public interface IS3Service
    {
        Task<string> UploadFileToS3Bucket(IFormFile file, string fileName);
        Task DeleteFileFromS3Bucket(string name);
    }
}
