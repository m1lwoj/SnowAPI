using SnowBLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    public class ErrorResult
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public ErrorModel[] Errors { get; set; }
        public ErrorStatus Status { get; set; }
    }
}
