using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    public class Result<T> 
    {
        public Result()
        {
            this.IsOk = true;
        }

        public Result(T content)
        {
            this.IsOk = true;
            this.Content = content;
        }

        public Result(ErrorResult error)
        {
            this.IsOk = false;
            this.Error = error;
        }

        public T Content { get; set; }
        public ErrorResult Error { get; set; }
        public bool IsOk { get; private set; }
    }
}
