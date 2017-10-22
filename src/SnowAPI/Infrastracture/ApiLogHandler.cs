using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SnowBLL.Resolvers;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SnowAPI.Infrastracture
{
    /// <summary>
    /// Middleware for logging requests
    /// </summary>
    public class ApiLogMiddleware
    {
        #region Members

        private readonly RequestDelegate _next;
        private readonly IRequestLogService _logService;
        private IUserResolver _userResolver;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Construct Api log middleware
        /// </summary>
        /// <param name="next">Request delegate</param>
        /// <param name="logService">Log service</param>
        /// <param name="userResolver">User resolver</param>
        public ApiLogMiddleware(RequestDelegate next, IRequestLogService logService, IUserResolver userResolver)
        {
            _next = next;
            _logService = logService;
            _userResolver = userResolver;
        }

        #endregion Constructors

        #region Public Methods

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("/api/"))
            {
                APILogEntity logEntity = new APILogEntity();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await LogRequest(context, logEntity);

                await LogResponseAndInvokeNext(context, logEntity);

                stopwatch.Stop();

                logEntity.ResponseTime = stopwatch.Elapsed;

                _logService.SaveLog(logEntity);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task LogRequest(HttpContext context, APILogEntity logEntity)
        {
            string body = string.Empty;

            //using (var buffer = new MemoryStream())
            //{
            //    context.Request.Body.CopyTo(buffer);
            //    context.Request.Body.Seek(0, SeekOrigin.Begin);

            //    using (var bodyReader = new StreamReader(buffer))
            //    {
            //         body = await bodyReader.ReadToEndAsync();
            //    }
            //}

            logEntity.Application = "SnowApp";
            logEntity.User = _userResolver.GetEmail();
            logEntity.Machine = Environment.MachineName;
            logEntity.RequestContentType = context.Request.ContentType;
            logEntity.RequestContentBody = body;
            logEntity.RequestIpAddress = context.Connection.RemoteIpAddress.ToString();
            logEntity.RequestMethod = context.Request.Method;
            logEntity.RequestHeaders = SerializeHeaders(context.Request.Headers);
            logEntity.RequestTimestamp = DateTime.Now;
            logEntity.RequestUri = System.Uri.UnescapeDataString(context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path + context.Request.QueryString.ToUriComponent());
        }

        private async Task LogResponseAndInvokeNext(HttpContext context, APILogEntity logEntity)
        {
            using (var buffer = new MemoryStream())
            {
                //replace the context response with our buffer
                var stream = context.Response.Body;
                context.Response.Body = buffer;

                //invoke the rest of the pipeline
                await _next(context);

                //reset the buffer and read out the contents
                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                using (var bufferReader = new StreamReader(buffer))
                {
                    string body = await bufferReader.ReadToEndAsync();

                    //reset to start of stream
                    buffer.Seek(0, SeekOrigin.Begin);

                    //copy our content to the original stream and put it back
                    await buffer.CopyToAsync(stream);
                    context.Response.Body = stream;

                    if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
                    {
                        logEntity.ResponseContentBody = body;
                    }
                    else
                    {
                        logEntity.ResponseContentBody = "... NOT A JSON ...";
                    }

                    logEntity.ResponseContentType = context.Response.ContentType;
                    logEntity.ResponseHeaders = SerializeHeaders(context.Response.Headers);
                    logEntity.ResponseStatusCode = (int)context.Response.StatusCode;
                    logEntity.ResponseTimestamp = DateTime.Now;
                }
            }
        }

        ///// <summary>
        ///// Executes middleware logic
        ///// </summary>
        ///// <param name="context">Http context</param>
        //public async Task Invoke(HttpContext context)
        //{
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();

        //    //Workaround - copy original Stream
        //    var initalBody = context.Request.Body;

        //    using (var bodyReader = new StreamReader(context.Request.Body))
        //    {
        //        string body = await bodyReader.ReadToEndAsync();
        //        //Do something with body
        //        //Replace write only request body with read/write memorystream so you can read from it later

        //        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));



        //        //handle other middlewares
        //        await _next.Invoke(context);

        //        //Workaround - return back to original Stream
        //        context.Request.Body = initalBody;

        //    }

        //    //using (var buffer = new MemoryStream())
        //    //{
        //    //    //Workaround of unreadable response
        //    //    var stream = context.Response.Body;
        //    //    context.Response.Body = buffer;

        //    //    APILogEntity logEntity = await GetRequestInfo(context);

        //    //    await _next(context);

        //    //    buffer.Seek(0, SeekOrigin.Begin);
        //    //    var reader = new StreamReader(buffer);

        //    //    using (var bufferReader = new StreamReader(buffer))
        //    //    {
        //    //        string body = await bufferReader.ReadToEndAsync();

        //    //        //reset to start of stream
        //    //        buffer.Seek(0, SeekOrigin.Begin);

        //    //        //copy our content to the original stream and put it back
        //    //        await buffer.CopyToAsync(stream);
        //    //        context.Response.Body = stream;

        //    //        if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
        //    //        {
        //    //            logEntity.ResponseContentBody = body;
        //    //        }
        //    //        else
        //    //        {
        //    //            logEntity.ResponseContentBody = "Not a JSON";
        //    //        }
        //    //    }


        //    //    logEntity.ResponseContentType = context.Response.ContentType;
        //    //    logEntity.ResponseHeaders = SerializeHeaders(context.Response.Headers);
        //    //    logEntity.ResponseStatusCode = (int)context.Response.StatusCode;
        //    //    logEntity.ResponseTimestamp = DateTime.Now;

        //    //    stopwatch.Stop();

        //    //    logEntity.ResponseTime = stopwatch.Elapsed;

        //    //    _logService.SaveLog(logEntity);
        //    //}
        //}

        #endregion Public Methods

        #region Private Methods

        private async Task<APILogEntity> GetRequestInfo(HttpContext context)
        {
            string requestBody = string.Empty;
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                requestBody = await bodyReader.ReadToEndAsync();
            }

            APILogEntity logEntity = new APILogEntity();
            logEntity.Application = "SnowApp";
            logEntity.User = _userResolver.GetEmail();
            logEntity.Machine = Environment.MachineName;
            logEntity.RequestContentType = context.Request.ContentType;
            logEntity.RequestContentBody = requestBody;
            logEntity.RequestIpAddress = context.Connection.RemoteIpAddress.ToString();
            logEntity.RequestMethod = context.Request.Method;
            logEntity.RequestHeaders = SerializeHeaders(context.Request.Headers);
            logEntity.RequestTimestamp = DateTime.Now;
            logEntity.RequestUri = System.Uri.UnescapeDataString(context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.Path + context.Request.QueryString.ToUriComponent());
            return logEntity;
        }

        private string SerializeHeaders(IHeaderDictionary headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    var header = String.Empty;
                    foreach (var value in item.Value)
                    {
                        header += value + " ";
                    }

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        #endregion Private Methods
    }
}