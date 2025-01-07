using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StoryApp.API.Controllers;
using StoryApp.API.Models;
using StoryApp.API.Services;
using StoryApp.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StoryApp.Test.Controllers
{
    [TestClass]
    public class StoriesControllerTest
    {
        private Mock<ILogger<StoriesController>> _mockLogger;
        private Mock<IStoriesService> _mockStoriesService;
        private StoriesController _storiesController;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<StoriesController>>();            
            _mockStoriesService = new Mock<IStoriesService>();
            _storiesController = new StoriesController(_mockStoriesService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetNewStories_ShouldReturnsOk_WithValidData()
        {
            // Arrange
            var story1 = new StoryModel { Title = "Story 1", Url = "http://story.com/1" };
            var story2 = new StoryModel { Title = "Story 2", Url = "http://story.com/2" };
            var expectedPageResult = new PagedResult<StoryModel>
            {
                Stories = new List<StoryModel>() { story1, story2 },
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10               
            };

            _mockStoriesService.Setup(s => s.GetNewStoriesByPageAsync(1, 10))
                .Returns(Task.FromResult(expectedPageResult));

            // Act
            var response = await _storiesController.GetNewStories();

            // Assert
            Assert.IsNotNull(response);
            var okObjectResult = response as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okObjectResult.StatusCode);

            var actualPageResult = okObjectResult.Value as PagedResult<StoryModel>;
            Assert.IsNotNull(actualPageResult);
            Assert.AreEqual(expectedPageResult.Stories.Count, actualPageResult.Stories.Count);
            Assert.AreEqual(expectedPageResult.TotalRecords, actualPageResult.TotalRecords);
            Assert.AreEqual(expectedPageResult.CurrentPage, actualPageResult.CurrentPage);
            Assert.AreEqual(expectedPageResult.PageSize, actualPageResult.PageSize);
            Assert.AreEqual(expectedPageResult.TotalPages, actualPageResult.TotalPages);
        }

        [TestMethod]
        public async Task GetNewStories_ShouldReturnsOk_WithEmptyData()
        {
            // Arrange
            PagedResult<StoryModel> expectedPageResult = new();
            _mockStoriesService.Setup(s => s.GetNewStoriesByPageAsync(1, 10))
                .Returns(Task.FromResult(expectedPageResult));

            // Act
            var response = await _storiesController.GetNewStories();

            // Assert
            Assert.IsNotNull(response);
            var okObjectResult = response as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okObjectResult.StatusCode);

            var actualPageResult = okObjectResult.Value as PagedResult<StoryModel>;
            Assert.IsNotNull(actualPageResult);
            Assert.AreEqual(expectedPageResult.Stories.Count, actualPageResult.Stories.Count);
            Assert.AreEqual(expectedPageResult.TotalRecords, actualPageResult.TotalRecords);
            Assert.AreEqual(expectedPageResult.CurrentPage, actualPageResult.CurrentPage);
            Assert.AreEqual(expectedPageResult.PageSize, actualPageResult.PageSize);
            Assert.AreEqual(expectedPageResult.TotalPages, actualPageResult.TotalPages);
        }

        [TestMethod]
        public async Task GetNewStories_ShouldHandleException()
        {
            // Arrange
            var mockStoriesService = new Mock<IStoriesService>();
            mockStoriesService.Setup(s => s.GetNewStoriesByPageAsync(1, 10))
                .Throws(new InvalidOperationException());

            // Act
            var response = await _storiesController.GetNewStories();

            // Assert
            Assert.IsNotNull(response);
            var okObjectResult = response as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okObjectResult.StatusCode);
            Assert.IsNull(okObjectResult.Value);
        }

        [TestMethod]
        public async Task SearchStories_ShouldReturnOk_WithValidData()
        {
            // Arrange
            var story1 = new StoryModel { Title = "How to master coding", Url = "http://story.com/1" };
            var story2 = new StoryModel { Title = "Coding for beginners", Url = "http://story.com/2" };
            var search = "coding";
            var expectedPageResult = new PagedResult<StoryModel>
            {
                Stories = new List<StoryModel>() { story1, story2 },
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10
            };

            _mockStoriesService.Setup(s => s.SearchNewStoriesByPageAsync(1, 10, search))
                .Returns(Task.FromResult(expectedPageResult));

            // Act
            var response = await _storiesController.SearchNewStories(search, 1, 10);

            // Assert
            Assert.IsNotNull(response);
            var okObjectResult = response as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okObjectResult.StatusCode);

            var actualPageResult = okObjectResult.Value as PagedResult<StoryModel>;
            Assert.IsNotNull(actualPageResult);
            Assert.AreEqual(expectedPageResult.Stories.Count, actualPageResult.Stories.Count);
            Assert.AreEqual(expectedPageResult.TotalRecords, actualPageResult.TotalRecords);
            Assert.AreEqual(expectedPageResult.CurrentPage, actualPageResult.CurrentPage);
            Assert.AreEqual(expectedPageResult.PageSize, actualPageResult.PageSize);
            Assert.AreEqual(expectedPageResult.TotalPages, actualPageResult.TotalPages);
        }
    }
}
