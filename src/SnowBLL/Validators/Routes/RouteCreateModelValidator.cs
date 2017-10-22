using FluentValidation;
using SnowBLL.Enums;
using SnowBLL.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Routes
{
    public class RouteCreateModelValidator : AbstractValidator<RouteCreateModel>
    {
        public RouteCreateModelValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(RouteMessages.NAME_NOTEMPTY);
            RuleFor(user => user.Line).NotEmpty().WithMessage(RouteMessages.LINE_NOTEMPTY);
            RuleFor(user => user.Difficulty).NotEmpty().WithMessage(RouteMessages.DIFFICULTY_NOTEMPTY);
            RuleFor(user => user.Difficulty).LessThanOrEqualTo(x => (int)Difficulty.Expert).WithMessage(RouteMessages.DIFFICULTY_TOOHIGH);
            RuleFor(user => user.Difficulty).GreaterThanOrEqualTo(x => (int)Difficulty.Begginner).WithMessage(RouteMessages.DIFFICULTY_TOOLOW);
        }
    }
}
