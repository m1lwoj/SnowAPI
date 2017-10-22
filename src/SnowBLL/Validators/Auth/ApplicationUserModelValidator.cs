using FluentValidation;
using SnowBLL.Models.Auth;
using SnowBLL.Validators.Users;

namespace SnowBLL.Validators.Auth
{
    public class ApplicationUserModelValidator : AbstractValidator<ApplicationUserModel>
    {
        public ApplicationUserModelValidator()
        {
            RuleFor(user => user.Email).NotEmpty().WithMessage(UserMessages.EMAIL_NOTEMPTY);
            RuleFor(user => user.Email).EmailAddress().WithMessage(UserMessages.EMAIL_VALIDFORMAT);
            RuleFor(user => user.Password).NotEmpty().WithMessage(UserMessages.PASSWORD_NOTEMPTY);
        }
    }
}
