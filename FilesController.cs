using Invoicing.Infrastructure.Models;
using Invoicing.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCore.Common;
using MvcCore.Common.Controllers;
using MvcCore.Common.Filters;
using System;

namespace Invoicing.Api.Controllers
{
    [Route("invoicing/[controller]")]
    public class FilesController : BaseController
    {
        private FilesService _filesService;

        public FilesController(FilesService service)
        {
            _filesService = service;
        }

        private FilesService Files
        {
            get
            {
                return (FilesService)_filesService.SetDependencies(AppAccess.SelectedCompany, AppAccess.UserId, AppAccess.TenantId);
            }
        }

        // GET api/values/5
        [HttpGet]
        public IActionResult Get()
        {
            return new ApiResponseResult(Files.GetAll());
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            return new ApiResponseResult(Files.GetByName(name));
        }

        [HttpPost]
        [ValidateModelActionFilter]
        public IActionResult Post(IFormFile value)
        {
            return new ApiResponseResult(Files.UploadFile(value));
        }

        [HttpPut("{name}")]
        [ValidateModelActionFilter]
        public IActionResult Put(IFormFile value, string name)
        {
            return new ApiResponseResult(Files.UpdateFile(value, name));
        }

        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            return new ApiResponseResult(Files.DeleteFile(name));
        }
    }
}
