using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Moq;
using MyEcommerceBackend.Controllers;
using MyEcommerceBackend.Models;
using System.Threading.Tasks;
using Xunit;

namespace MyEcommerceBackend.Tests
{
    public class AccountControllerTests
    {
        [Fact] // Defines a test method that is to be run by the test runner.
        public async Task Register_ReturnsRegisterSuccessView_WhenRegistrationIsSuccessful()
        {
            // Arrange
            // Creating mock objects for dependencies required by the AccountController.
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<IdentityUser>>>();
            var schemesMock = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmationMock = new Mock<IUserConfirmation<IdentityUser>>();

            // Creating SignInManager mock with its dependencies.
            var signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object, 
                contextAccessorMock.Object, 
                userPrincipalFactoryMock.Object, 
                optionsMock.Object, 
                loggerMock.Object, 
                schemesMock.Object, 
                userConfirmationMock.Object);

            // Mocking configuration settings related to email.
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(a => a["SmtpEmail"]).Returns("testemail@example.com");
            configurationMock.SetupGet(a => a["SmtpPassword"]).Returns("testpassword");

            // Creating an instance of the AccountController with the mocked dependencies.
            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object, configurationMock.Object);

            // Creating a sample RegisterModel for the test.
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Setting up the mock userManager to return success when CreateAsync method is called.
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
              .ReturnsAsync(IdentityResult.Success);

            // Act
            // Calling the Register method of the AccountController with the model.
            var result = await controller.Register(model);

            // Assert
            // Checking if the result of the method is a redirection to the RegisterSuccess action.
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RegisterSuccess", redirectToActionResult.ActionName);
            Assert.Equal("View", redirectToActionResult.ControllerName); 
        }
    }
}
