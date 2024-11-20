using Microsoft.AspNetCore.Mvc;
using Moq;
using SynonymsSearchTool.Api.Controllers;
using SynonymsSearchTool.Api.Models;
using SynonymsSearchTool.Application.DTOs;
using SynonymsSearchTool.Application.Interfaces;
using SynonymsSearchTool.Application.Validation;

namespace SynonymsSearchTool.Tests;

public class SynonymsControllerTests
{
    // Mocking the ISynonymService dependency that is injected into the controller.
    private readonly Mock<ISynonymService> _synonymServiceMock;

    private readonly SynonymsController _controller;

    public SynonymsControllerTests()
    {
        _synonymServiceMock = new Mock<ISynonymService>();
        _controller = new SynonymsController(_synonymServiceMock.Object);
    }

    #region GetSynonyms Tests

    /// <summary>
    /// Tests that the controller returns a 200 OK result when synonyms exist for a word.
    /// </summary>
    [Fact]
    public async Task GetSynonyms_ShouldReturnOk_WhenSynonymsExist()
    {
        // Arrange: Prepare the word and expected synonyms data.
        var word = "happy";
        var synonymsDto = new SynonymsDto
        {
            Synonyms = ["joyful", "content"]
        };

        // Setup the mock service to return the synonyms.
        _synonymServiceMock
            .Setup(service => service.GetSynonymsAsync(word))
            .ReturnsAsync(synonymsDto);

        // Act: Call the controller's method to get synonyms for the word.
        var result = await _controller.GetSynonyms(word);

        // Assert: Verify that the result is of type OkObjectResult and contains the expected synonyms.
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<SynonymsResponse>(okResult.Value);
        Assert.Contains("joyful", response.Synonyms);
    }

    /// <summary>
    /// Tests that the controller returns a 404 NotFound when no synonyms exist for the word.
    /// </summary>
    [Fact]
    public async Task GetSynonyms_ShouldReturnNotFound_WhenNoSynonymsExist()
    {
        // Arrange: Prepare a word that does not have synonyms.
        var word = "unknown";
        _synonymServiceMock
            .Setup(service => service.GetSynonymsAsync(word))
            .ReturnsAsync((SynonymsDto)null);

        // Act: Call the controller's method to get synonyms for the word.
        var result = await _controller.GetSynonyms(word);

        // Assert: Verify that the result is a NotFoundObjectResult with the appropriate message.
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"No synonyms found for the word: {word}", notFoundResult.Value);
    }

    /// <summary>
    /// Tests that the controller returns a 400 BadRequest when the word parameter is invalid (empty or null).
    /// </summary>
    [Fact]
    public async Task GetSynonyms_ShouldReturnBadRequest_WhenWordIsInvalid()
    {
        // Act: Call the controller's method with an invalid word parameter.
        var result = await _controller.GetSynonyms("");

        // Assert: Verify that the result is a BadRequestObjectResult with the appropriate error message.
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("The word parameter cannot be null, empty, or whitespace.", badRequestResult.Value);
    }

    /// <summary>
    /// Tests that the controller returns a 500 InternalServerError when an exception occurs while fetching synonyms.
    /// </summary>
    [Fact]
    public async Task GetSynonyms_ShouldReturnServerError_OnException()
    {
        // Arrange: Setup the mock service to throw an exception when fetching synonyms.
        var word = "happy";
        _synonymServiceMock
            .Setup(service => service.GetSynonymsAsync(word))
            .ThrowsAsync(new Exception("Test exception"));

        // Act: Call the controller's method, which should trigger the exception.
        var result = await _controller.GetSynonyms(word);

        // Assert: Verify that the result is an ObjectResult with a 500 status code and the appropriate error message.
        var serverErrorResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, serverErrorResult.StatusCode);
        Assert.Contains("An error occurred while fetching synonyms", serverErrorResult.Value.ToString());
    }

    #endregion

    #region SaveSynonyms Tests

    /// <summary>
    /// Tests that the controller returns a 200 OK result when the synonyms are successfully saved.
    /// </summary>
    [Fact]
    public async Task SaveSynonyms_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange: Create a valid request with a word and its synonyms.
        var request = new SaveSynonymsRequest
        {
            Word = "happy",
            Synonyms = ["joyful", "content"]
        };

        // Setup the mock service to complete the save operation successfully.
        _synonymServiceMock
            .Setup(service => service.SaveSynonymsAsync(It.IsAny<SaveSynonymsDto>()))
            .Returns(Task.CompletedTask);

        // Act: Call the controller's method to save the synonyms.
        var result = await _controller.SaveSynonyms(request);

        // Assert: Verify that the result is an OkObjectResult with a success message.
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Synonyms saved successfully.", okResult.Value);
    }

    /// <summary>
    /// Tests that the controller returns a 400 BadRequest when the request is null.
    /// </summary>
    [Fact]
    public async Task SaveSynonyms_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act: Call the controller's method with a null request.
        var result = await _controller.SaveSynonyms(null);

        // Assert: Verify that the result is a BadRequestObjectResult with the appropriate error message.
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The request body cannot be null.", badRequestResult.Value);
    }

    /// <summary>
    /// Tests that the controller returns a 400 BadRequest when a validation exception occurs while saving synonyms.
    /// </summary>
    [Fact]
    public async Task SaveSynonyms_ShouldReturnBadRequest_OnValidationException()
    {
        // Arrange: Create a valid request, but setup the mock service to throw a validation exception.
        var request = new SaveSynonymsRequest
        {
            Word = "happy",
            Synonyms = ["joyful", "content"]
        };

        _synonymServiceMock
            .Setup(service => service.SaveSynonymsAsync(It.IsAny<SaveSynonymsDto>()))
            .ThrowsAsync(new ValidationException("Validation failed"));

        // Act: Call the controller's method to save the synonyms.
        var result = await _controller.SaveSynonyms(request);

        // Assert: Verify that the result is a BadRequestObjectResult with the validation exception message.
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Validation failed", badRequestResult.Value);
    }

    /// <summary>
    /// Tests that the controller returns a 500 InternalServerError when an exception occurs while saving synonyms.
    /// </summary>
    [Fact]
    public async Task SaveSynonyms_ShouldReturnServerError_OnException()
    {
        // Arrange: Setup the mock service to throw a generic exception during save operation.
        var request = new SaveSynonymsRequest
        {
            Word = "happy",
            Synonyms = ["joyful", "content"]
        };

        _synonymServiceMock
            .Setup(service => service.SaveSynonymsAsync(It.IsAny<SaveSynonymsDto>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act: Call the controller's method to save the synonyms, which will trigger an exception.
        var result = await _controller.SaveSynonyms(request);

        // Assert: Verify that the result is an ObjectResult with a 500 status code and the appropriate error message.
        var serverErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverErrorResult.StatusCode);
        Assert.Contains("An error occurred while saving synonyms", serverErrorResult.Value.ToString());
    }

    #endregion
}