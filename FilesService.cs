using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Extensions;
using Common.Models;
using Common.Services;
using Common.ServicesResult;
using GoWebApp.Common.Enums;
using GoWebApp.Common.Services;
using Invoicing.Data.Entities;
using Invoicing.Infrastructure.Data.Repositories;
using Invoicing.Infrastructure.Enums;
using Invoicing.Infrastructure.Models;
using Logging.Infrastructure;
using Logging.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Invoicing.Infrastructure.Services
{
    public class FilesService : BaseService
    {
        private FileRepository _fileRepo;
        private ILogService _logService;
        private IS3Service _s3Service;

        public FilesService(FileRepository fileRepo, ILogService logService, IS3Service s3Service)
        {
            _fileRepo = fileRepo;
            _logService = logService;
            _s3Service = s3Service;
        }

        public DetailsFileModel GetByName(string name)
        {
            var result = _fileRepo.Where(a => a.FileName.Equals(name) && a.CompanyId.Equals(companyId)).FirstOrDefault();           
            return Mapper.Map<DetailsFileModel>(result);
        }
             
        public IQueryable<DetailsFileModel> GetAll()
        {
            return _fileRepo.Where(a =>a.CompanyId.Equals(companyId)).ProjectTo<DetailsFileModel>();
        }

        public IOperationResult UploadFile(IFormFile value)
        {
            if (value == null || value.Length == 0)
            {
                return OperationResult.NotSucceeded(MessagesService.SaveFailMessage("File", "File not uploaded"));
            }

            try
            {
                //file data to DB
                var fileModel = Mapper.Map<UploadFileModel>(value);
                fileModel.Name = Guid.NewGuid().ToString();
                fileModel.FileName = fileModel.Name + Path.GetExtension(value.FileName);
                fileModel.CreatedBy = currentUserId;
                fileModel.TenantId = tenantId;
                fileModel.CompanyId = companyId;
                //uploading to bucket
                fileModel.Path = _s3Service.UploadFileToS3Bucket(value, fileModel.FileName).Result;
                var file = Mapper.Map<Invoicing.Data.Entities.File>(fileModel);
                              
                //saving to db
                _fileRepo.Add(file);


                //return result
                _logService.LogSuccess(ModulesEnum.Invoicing.ToString(), UserActionEnum.Upload_File.ToString(), fileModel.CreatedBy, MessagesService.CreateSuccessMessage("File"), null, file);
                return OperationResult.Succeeded(Mapper.Map<DetailsFileModel>(file), MessagesService.CreateSuccessMessage("File"));
            }
            catch (Exception ex)
            {
                _logService.LogError(ModulesEnum.Invoicing.ToString(), ex);
                return OperationResult.NotSucceeded(MessagesService.SaveFailMessage("File", ex.Message));
            }
        }

        public IOperationResult UpdateFile(IFormFile value, string name)
        {
            try
            {
                var file = GetByName(name);
                if (file != null)
                {
                    //deleting previous file
                    var fileToDelete = Mapper.Map<Invoicing.Data.Entities.File>(file);
                    _fileRepo.Delete(fileToDelete);
                    _s3Service.DeleteFileFromS3Bucket(name);

                    //creating new object
                    var fileModel = Mapper.Map<UploadFileModel>(value);
                    fileModel.FileName = file.Name + Path.GetExtension(value.FileName);
                    fileModel.CreatedBy = currentUserId;
                    fileModel.TenantId = tenantId;
                    fileModel.CompanyId = companyId;
                    //uploading to bucket
                    fileModel.Path = _s3Service.UploadFileToS3Bucket(value, fileModel.FileName).Result;

                    //saving new file to databse
                    var fileToAdd = Mapper.Map<Invoicing.Data.Entities.File>(fileModel);
                    _fileRepo.Add(fileToAdd);

                    _logService.LogSuccess(ModulesEnum.Invoicing.ToString(), UserActionEnum.Update_File.ToString(), fileModel.CreatedBy, MessagesService.UpdateSuccessMessage("File"));
                    return OperationResult.Succeeded(MessagesService.UpdateSuccessMessage("File"));
                }
                else
                {
                    return OperationResult.NotSucceeded(MessagesService.NotFoundMessage("File"));
                }

            }
            catch (Exception ex)
            {
                _logService.LogError(ModulesEnum.Invoicing.ToString(), ex);
                return OperationResult.NotSucceeded(MessagesService.UpdateFailMessage("File", ex.Message));
            }
            
        }

        public IOperationResult DeleteFile(string name)
        {
            try
            {
                var file = GetByName(name);
                if (file != null)
                {
                    var fileResult = Mapper.Map<Invoicing.Data.Entities.File>(file);
                    _fileRepo.Delete(fileResult);
                    _s3Service.DeleteFileFromS3Bucket(name);

                    _logService.LogSuccess(ModulesEnum.Invoicing.ToString(), UserActionEnum.Delete_FIle.ToString(), file.CreatedBy, MessagesService.DeleteSuccessMessage("File"));
                    return OperationResult.Succeeded(MessagesService.DeleteSuccessMessage("File"));
                }
                else
                {
                    return OperationResult.NotSucceeded(MessagesService.NotFoundMessage("File"));
                }                
            }
            catch (Exception ex)
            {
                _logService.LogError(ModulesEnum.Invoicing.ToString(), ex);
                return OperationResult.NotSucceeded(MessagesService.DeleteFailMessage("File", ex.Message));
            }          
        }

    }
}
