using FluentValidation;
using SnowBLL.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Routes
{
    public class RoutesInRangeRequestModelValidator
         : AbstractValidator<RoutesInRangeRequestModel>
    {
        public RoutesInRangeRequestModelValidator()
        {
            RuleFor(user => user.Kilometers).NotEmpty().WithMessage(RouteMessages.NAME_NOTEMPTY);
            RuleFor(user => user.Point).NotNull().WithMessage(RouteMessages.POINT_NOTEMPTY);
            RuleFor(user => user.Point.Lat).NotNull().WithMessage(RouteMessages.POINT_NOTEMPTY);
            RuleFor(user => user.Point.Lng).NotNull().WithMessage(RouteMessages.POINT_NOTEMPTY);
        }
    }
}