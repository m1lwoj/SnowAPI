using Microsoft.Extensions.Logging;
using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Service.Interfaces;
using System;
using System.Collections.Generic;

namespace SnowBLL.Service.Concrete
{
    public class BLService : IBLService
    {
        protected readonly ILogger<BLService> _logger;
            
        public BLService(ILogger<BLService> logger)
        {
            _logger = logger;
        }

        public ErrorResult GenerateError(Exception ex)
        {
            string id = Guid.NewGuid().ToString();

            ErrorResult error = new ErrorResult()
            {
                Errors = new List<ErrorModel>().ToArray(),
                Id = id,
                Message = ex.Message,
                Status = ErrorStatus.InternalServer
            };

            _logger.LogError($"Identifier: {error.Id}{Environment.NewLine} Error: {error.Message}{Environment.NewLine} Callstack: {ex.StackTrace}{Environment.NewLine}");

            return error;
        }

        public ErrorResult GenerateError(string message, string field, string fieldmessage, ErrorStatus status)
        {
            string id = Guid.NewGuid().ToString();

            ErrorModel errorModel = new ErrorModel()
            {
                Field = field,
                Message = fieldmessage
            };

            var errors = new List<ErrorModel>();
            errors.Add(errorModel);

            ErrorResult error = new ErrorResult()
            {
                Errors = errors.ToArray(),
                Id = id,
                Message = message,
                Status = status
            };

            return error;
        }
    }
}
