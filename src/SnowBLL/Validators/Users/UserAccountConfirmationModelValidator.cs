using FluentValidation;
using SnowBLL.Models.Users;

namespace SnowBLL.Validators.Users
{
    public class UserAccountConfirmationModelValidator : AbstractValidator<UserAccountConfirmationModel>
    {
        public UserAccountConfirmationModelValidator()
        {
            RuleFor(user => user.Code).NotEmpty().WithMessage(UserMessages.CODE_NOTEMPTY);
            RuleFor(user => user.Code).Length(4, 4).WithMessage(UserMessages.CODE_4CHARACTERS);
        }
    }
}
