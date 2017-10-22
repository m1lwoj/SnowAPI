using FluentValidation;
using SnowBLL.Models.Users;

namespace SnowBLL.Validators.Users
{
    public class UserCreateModelValidator : AbstractValidator<UserCreateModel>
    {
        public UserCreateModelValidator()
        {
            RuleFor(user => user.Email).NotEmpty().WithMessage(UserMessages.EMAIL_NOTEMPTY);
            RuleFor(user => user.Email).EmailAddress().WithMessage(UserMessages.EMAIL_VALIDFORMAT);
            RuleFor(user => user.Password).NotEmpty().WithMessage(UserMessages.PASSWORD_NOTEMPTY);
            RuleFor(user => user.Password).Length(6,100).WithMessage(UserMessages.PASSWORD_LENGTH);
        }
    }
}