using SnowBLL.Validators.Users;
using Xunit;
using FluentValidation.TestHelper;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowBLL.Validators.Routes;
using SnowBLL.Models.Geo;

namespace SnowTests.Unit.Validators
{
    public class ModelValidatorTests
    {
        [Fact]
        public void ConfirmAccountEmailValidResult()
        {
            var validator = new NewPasswordModelValidator();
            validator.ShouldHaveValidationErrorFor(model => model.Code, "");
            validator.ShouldHaveValidationErrorFor(model => model.Code, "12345");
            validator.ShouldHaveValidationErrorFor(model => model.Code, "125");
            validator.ShouldNotHaveValidationErrorFor(model => model.Code, "1234");

            var wrongPasswordModel = new NewPasswordModel()
            {
                ConfirmedPassword = "test123",
                Password = "test1233"
            };

            validator.ShouldHaveValidationErrorFor(model => model.ConfirmedPassword, wrongPasswordModel);

            var validPasswordModel = new NewPasswordModel()
            {
                ConfirmedPassword = "test123",
                Password = "test123"
            };

            validator.ShouldHaveValidationErrorFor(model => model.ConfirmedPassword, wrongPasswordModel);

            validator.ShouldHaveValidationErrorFor(model => model.Email, "123123");
            validator.ShouldHaveValidationErrorFor(model => model.Email, "");

            validator.ShouldHaveValidationErrorFor(model => model.Password, "");
            validator.ShouldHaveValidationErrorFor(model => model.Email, "123sd");
        }

        [Fact]
        public void UserAccountConfirmationModelValidResult()
        {
            var validator = new UserAccountConfirmationModelValidator();
            validator.ShouldHaveValidationErrorFor(model => model.Code, "");
            validator.ShouldHaveValidationErrorFor(model => model.Code, "12345");
            validator.ShouldHaveValidationErrorFor(model => model.Code, "125");
            validator.ShouldNotHaveValidationErrorFor(model => model.Code, "1234");
        }

        [Fact]
        public void RoutesInRangeRequestModelValidatorValidResult()
        {
        //    var validator = new RoutesInRangeRequestModelValidator();
        //    validator.ShouldHaveChildValidator(model => model.Kilometers, (System.Int32)0);
        //    validator.ShouldNotHaveValidationErrorFor(model => model.Kilometers, (int)10);
        //    validator.ShouldHaveValidationErrorFor(model => model.Point, (GeoPoint)null);
        //    validator.ShouldHaveValidationErrorFor(model => model.Point.Latitude, (double?)null);
        //    validator.ShouldHaveValidationErrorFor(model => model.Point.Longitude, (double?)null);
        //    validator.ShouldNotHaveValidationErrorFor(model => model.Point, new GeoPoint() { Latitude = 2.0d, Longitude = 3.0d }) ;
        }
    }
}
