using FluentValidation;
using SnowBLL.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Users
{
    public class UserUpdateModelValidator : AbstractValidator<UserUpdateModel>
    {
        public UserUpdateModelValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage(UserMessages.NAME_NOTEMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(UserMessages.EMAIL_NOTEMPTY);
            RuleFor(user => user.Email).NotEmpty().WithMessage(UserMessages.EMAIL_VALIDFORMAT);
        }
    }
}
