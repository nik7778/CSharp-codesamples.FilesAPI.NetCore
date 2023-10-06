using Invoicing.Infrastructure.Mappings;
using Common.CustomAnnotations;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Models;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Invoicing.Data.Entities;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Invoicing.Infrastructure.Models
{
    [BsonIgnoreExtraElements]
    public class UploadFileModel : BaseModel, IHaveMappings
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string CompanyId { get; set; }

        public void CreateMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<File, UploadFileModel>().ReverseMap();
            config.CreateMap<IFormFile, UploadFileModel>().ReverseMap();
        }
    }
}
