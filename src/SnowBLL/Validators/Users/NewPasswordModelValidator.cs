using FluentValidation;
using SnowBLL.Models.Auth;

namespace SnowBLL.Validators.Users
{
    public class NewPasswordModelValidator : AbstractValidator<NewPasswordModel>
    {
        public NewPasswordModelValidator()
        {
            RuleFor(user => user.Code).NotEmpty().WithMessage(UserMessages.CODE_NOTEMPTY);
            RuleFor(user => user.Code).Length(4,4).WithMessage(UserMessages.CODE_4CHARACTERS);
            RuleFor(user => user.Email).NotEmpty().WithMessage(UserMessages.EMAIL_NOTEMPTY);
            RuleFor(user => user.Email).EmailAddress().WithMessage(UserMessages.EMAIL_VALIDFORMAT);
            RuleFor(user => user.Password).NotEmpty().WithMessage(UserMessages.PASSWORD_NOTEMPTY);
            RuleFor(user => user.Password).Length(6, 100).WithMessage(UserMessages.PASSWORD_LENGTH);
            RuleFor(user => user.ConfirmedPassword).Equal(user => user.Password).WithMessage(UserMessages.CONFIRMATIONPASSWORD_EQUAL);
        }
    }
}
