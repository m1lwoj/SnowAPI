using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;

namespace SnowAPI.Controllers
{
    public class BaseController : ApiController
    {
        protected IActionResult WrapResponse<T>(Result<T> result, HttpStatusCode? statusCode = null)
        {
            if (result.IsOk)
            {
                if (statusCode.HasValue)
                {
                    switch (statusCode.Value)
                    {
                        case HttpStatusCode.OK:
                            return Ok<T>(result.Content);
                        case HttpStatusCode.NoContent:
                            return new NoContentResult();
                        case HttpStatusCode.Created:
                            return Created(result.Content.ToString(), null);
                        default:
                            return Content<ErrorResult>(HttpStatusCode.InternalServerError, result.Error);
                    }
                }
                else
                {
                    return Ok<T>(result.Content);
                }
            }
            else
            {
                switch (result.Error.Status)
                {
                    case SnowBLL.Enums.ErrorStatus.ObjectNotFound:
                        return Content<ErrorResult>(HttpStatusCode.NotFound, result.Error);
                    case SnowBLL.Enums.ErrorStatus.InvalidModel:
                        return Content<ErrorResult>(HttpStatusCode.BadRequest, result.Error);
                    case SnowBLL.Enums.ErrorStatus.InternalServer:
                        return Content<ErrorResult>(HttpStatusCode.InternalServerError, result.Error);
                    case SnowBLL.Enums.ErrorStatus.Forbidden:
                        return Content<ErrorResult>(HttpStatusCode.Forbidden, result.Error);
                    default:
                        return Content<ErrorResult>(HttpStatusCode.InternalServerError, result.Error);
                }
            }
        }

        protected ErrorResult WrapModelStateErrors(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Keys
                .SelectMany(key => this.ModelState[key].Errors.Select(x => new ErrorModel()
                {
                    Field = key,
                    Message = x.ErrorMessage
                })).ToArray();

            string id = Guid.NewGuid().ToString();

            return new ErrorResult()
            {
                Errors = errorList,
                Id = id,
                Message = "Invalid model",
                Status = ErrorStatus.InvalidModel
            };
        }

        protected ErrorResult WrapModelStateErrors(ValidationResult validationResult)
        {
            var errorList = validationResult.Errors
                .Select(err => new ErrorModel()
                {
                    Field = err.PropertyName,
                    Message = err.ErrorMessage
                }).ToArray();

            string id = Guid.NewGuid().ToString();

            return new ErrorResult()
            {
                Errors = errorList,
                Id = id,
                Message = "Invalid model",
                Status = ErrorStatus.InvalidModel
            };
        }
    }
}
