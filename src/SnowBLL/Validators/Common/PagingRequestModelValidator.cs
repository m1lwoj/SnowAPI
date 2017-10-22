using FluentValidation;
using SnowBLL.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Common
{
    public class PagingRequestModelValidator
     : AbstractValidator<PagingRequestModel>
    {
        public PagingRequestModelValidator()
        {
        }
    }
}
