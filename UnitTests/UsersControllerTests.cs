using FilmLibrary.Controllers;
using FilmLibrary.Dtos;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class UsersControllerTests
    {
        [TestMethod]
        public void Register_EmailTaken_ExceptionThrown()
        {
            UserForRegisterDto userForRegister = new UserForRegisterDto()
            {
                FirstName = "Yordan",
                LastName = "Atanasov",
                Email = "whatever@whatever",
                Password = "123456",
                RepeatedPassword = "123456"
            };
            var authorizationServiceMock = new Mock<IAuthRepository>();
            var configureMock = new Mock<IConfiguration>();

            authorizationServiceMock.Setup(x => x.Register(It.IsAny<UserForRegisterDto>()))
                .Throws(new ArgumentException());
            var authorizationController = new UsersController(authorizationServiceMock.Object, configureMock.Object);
            var task = authorizationController.Register(userForRegister);
            task.Wait();

            var result = (ViewResult)task.Result;

            Assert.AreEqual(result.ViewName, "Register");
            Assert.IsTrue(result.Model is UserForRegisterDto user && user.IsEmailTaken == true);

            authorizationServiceMock.VerifyAll();
        }

        [TestMethod]
        public void Register_TryRegisterUserWithShortPassword_ErrorMessageReturned()
        {
            var user = new UserForRegisterDto { Email = " ", Password = "pass" };

            var requiredAttribute = new RequiredAttribute();
            var stringLengthAttribute = new StringLengthAttribute(maximumLength: 20)
            {
                MinimumLength = 5,
                ErrorMessage = "Password must be between 5 and 20 characters."
            };

            var requiredEmailResult = requiredAttribute.IsValid(user.Email);
            var requiredPasswordResult = requiredAttribute.IsValid(user.Password);
            var minLengthResult = stringLengthAttribute.IsValid(user.Password);

            Assert.IsTrue(requiredEmailResult == false);
            Assert.IsTrue(minLengthResult == false);
            Assert.IsTrue(requiredPasswordResult == true);
        }

        [TestMethod]
        public void Login_TryLoginUserWithShortPassword_ErrorMessageReturned()
        {
            var user = new UserForLogInDto { Email = " ", Password = "pass" };

            var requiredAttribute = new RequiredAttribute();
            var stringLengthAttribute = new StringLengthAttribute(maximumLength: 20)
            {
                MinimumLength = 5,
                ErrorMessage = "Password must be between 5 and 20 characters."
            };

            var requiredEmailResult = requiredAttribute.IsValid(user.Email);
            var requiredPasswordResult = requiredAttribute.IsValid(user.Password);
            var minLengthResult = stringLengthAttribute.IsValid(user.Password);

            Assert.IsTrue(requiredEmailResult == false);
            Assert.IsTrue(minLengthResult == false);
            Assert.IsTrue(requiredPasswordResult == true);
        }

        [TestMethod]
        public void Login_WrongEmailOrPassword_ExceptionThrown()
        {
            var user = new UserForLogInDto { Email = " ", Password = "12345" };

            var authorizationServiceMock = new Mock<IAuthRepository>();
            var configureMock = new Mock<IConfiguration>();

            authorizationServiceMock.Setup(x => x.LogIn(It.IsAny<UserForLogInDto>()))
                .Throws(new ArgumentException());
            var authorizationController = new UsersController(authorizationServiceMock.Object, configureMock.Object);
            var task = authorizationController.Login(user);
            task.Wait();

            var result = (ViewResult)task.Result;

            Assert.AreEqual(result.ViewName, "Login");
            Assert.IsTrue(result.Model is UserForLogInDto userForLogIn && userForLogIn.AreCredentialsCorrect == false);

            authorizationServiceMock.VerifyAll();
        }

        [TestMethod]
        public void Login_CorrectCredentials_SuccessfulLogin()
        {
            var userForLogIn = new UserForLogInDto { Email = "abv@abv.bg", Password = "12345" };

            var user = new User
            {
                Email = userForLogIn.Email,
                Password = userForLogIn.Password,
                FirstName = "PEsho",
                LastName = "Peshov",
                Id = Guid.NewGuid()
            };

            var urlMock = new Mock<IUrlHelper>();
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var authorizationServiceMock = new Mock<IAuthRepository>();
            var configureMock = new Mock<IConfiguration>();

            authorizationServiceMock.Setup(x => x.LogIn(It.IsAny<UserForLogInDto>()))
                .Returns(Task.Run(() => user));

            authorizationServiceMock.Setup(x => x.GenerateTokenAsync(user.Email, user.Id.ToString()))
                .Returns(Task.Run(() => "alabala"));
            var authorizationController = new UsersController(authorizationServiceMock.Object, configureMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        // How mock RequestServices?
                        RequestServices = serviceProviderMock.Object
                    }
                },
                Url = urlMock.Object
            };

            var task = authorizationController.Login(userForLogIn);
            task.Wait();

            var result = (RedirectToActionResult)task.Result;

            Assert.AreEqual(result.ActionName, "Index");
            Assert.AreEqual(result.ControllerName, "Home");

            authorizationServiceMock.VerifyAll();
            authServiceMock.VerifyAll();
            serviceProviderMock.VerifyAll();
        }

        [TestMethod]
        public void Logout_SuccessfulLogout()
        {
            var urlMock = new Mock<IUrlHelper>();
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var authorizationServiceMock = new Mock<IAuthRepository>();
            var configureMock = new Mock<IConfiguration>();

            var authorizationController = new UsersController(authorizationServiceMock.Object, configureMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        // How mock RequestServices?
                        RequestServices = serviceProviderMock.Object
                    }
                },
                Url = urlMock.Object
            };

            var task = authorizationController.Logout();
            task.Wait();

            var result = (RedirectToActionResult)task.Result;

            Assert.AreEqual(result.ActionName, "Login");
            Assert.AreEqual(result.ControllerName, "Users");
        }
    }
}
