using FilmLibrary.Controllers;
using FilmLibrary.Dtos;
using FilmLibrary.Models;
using FilmLibrary.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    [TestClass]
    public class StatisticsControllerTests
    {
        [TestMethod]
        public void GetTopGenresCount_CategoryIsNull_ExceptionThrown()
        {
            var moviesServiceMock = new Mock<IMoviesRepository>();
            var userServiceMock = new Mock<IUserRepository>();

            moviesServiceMock.Setup(x => x.GetGenresCountByCategoryForUser(new Guid(), null))
                .Throws(new ArgumentNullException());
            var moviesController = new StatisticsController(moviesServiceMock.Object, userServiceMock.Object);

            var task = moviesController.GetTopGenresCount(null);
            task.Wait();

            var result = (JsonResult)task.Result;
            var list = (List<GenreCount>)result.Value;

            Assert.IsTrue(list.Count == 0);
            moviesServiceMock.VerifyAll();
        }

        [TestMethod]
        public void GetTopActorsCount_CategoryIsNull_ExceptionThrown()
        {
            var moviesServiceMock = new Mock<IMoviesRepository>();
            var userServiceMock = new Mock<IUserRepository>();

            moviesServiceMock.Setup(x => x.GetActorsCountByCategoryForUser(new Guid(), null))
                .Throws(new ArgumentNullException());
            var moviesController = new StatisticsController(moviesServiceMock.Object, userServiceMock.Object);

            var task = moviesController.GetTopActorsCount(null);
            task.Wait();

            var result = (JsonResult)task.Result;
            var list = (List<GenreCount>)result.Value;

            Assert.IsTrue(list.Count == 0);
            moviesServiceMock.VerifyAll();
        }

        [TestMethod]
        public void GetTopProductionsCount_CategoryIsNull_ExceptionThrown()
        {
            var moviesServiceMock = new Mock<IMoviesRepository>();
            var userServiceMock = new Mock<IUserRepository>();

            moviesServiceMock.Setup(x => x.GetProductionsCountByCategoryForUser(new Guid(), null))
                .Throws(new ArgumentNullException());
            var moviesController = new StatisticsController(moviesServiceMock.Object, userServiceMock.Object);

            var task = moviesController.GetTopProductionsCount(null);
            task.Wait();

            var result = (JsonResult)task.Result;
            var list = (List<GenreCount>)result.Value;

            Assert.IsTrue(list.Count == 0);
            moviesServiceMock.VerifyAll();
        }
    }
}
