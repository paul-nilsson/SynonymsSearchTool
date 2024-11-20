using Microsoft.AspNetCore.Mvc;
using SynonymsSearchTool.Api.Mappers;
using SynonymsSearchTool.Api.Models;
using SynonymsSearchTool.Application.Interfaces;
using SynonymsSearchTool.Application.Validation;

namespace SynonymsSearchTool.Api.Controllers;

/// <summary>
/// API controller for managing synonyms.
/// </summary>
/// <remarks>
/// Constructor to inject the ISynonymService dependency.
/// </remarks>
/// <param name="synonymService">The synonym service used to handle synonyms.</param>
[ApiController]
[Route("[controller]")]
public class SynonymsController(ISynonymService synonymService) : ControllerBase
{
    private readonly ISynonymService _synonymService = synonymService;

    /// <summary>
    /// Endpoint to get synonyms for a given word.
    /// </summary>
    /// <param name="word">The word for which synonyms are to be fetched.</param>
    /// <returns>Returns a list of synonyms or an error message if not found.</returns>
    [HttpGet("{word}")]
    public async Task<ActionResult<SynonymsResponse>> GetSynonyms(string word)
    {
        // Check if the word is null, empty, or just whitespace
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("The word parameter cannot be null, empty, or whitespace.");

        try
        {
            // Fetch the synonyms from the service asynchronously
            var synonymsDto = await _synonymService.GetSynonymsAsync(word);

            // If no synonyms are found, return a NotFound result
            if (synonymsDto?.Synonyms == null || !synonymsDto.Synonyms.Any())
                return NotFound($"No synonyms found for the word: {word}");

            // Map the SynonymsDto to SynonymsResponse and return as OK response
            return Ok(synonymsDto.ToResponse());
        }
        catch (Exception ex)
        {
            // Catch any general exception and return a 500 status with the error message
            return StatusCode(500, $"An error occurred while fetching synonyms: {ex.Message}");
        }
    }

    /// <summary>
    /// Endpoint to save synonyms for a given word.
    /// </summary>
    /// <param name="request">The request body containing the word and its synonyms.</param>
    /// <returns>A status message indicating whether the synonyms were saved successfully or not.</returns>
    [HttpPost] // Maps POST requests (e.g., "/synonyms")
    public async Task<IActionResult> SaveSynonyms([FromBody] SaveSynonymsRequest request)
    {
        // Check if the request body is null
        if (request == null)
            return BadRequest("The request body cannot be null.");

        try
        {
            // Map the SaveSynonymsRequest to SaveSynonymsDto and pass it to the service for saving
            await _synonymService.SaveSynonymsAsync(request.ToDto());
            return Ok("Synonyms saved successfully.");
        }
        catch (ValidationException ex)
        {
            // Catch validation errors and return a 400 status with the error message
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Catch any other exceptions and return a 500 status with the error message
            return StatusCode(500, $"An error occurred while saving synonyms: {ex.Message}");
        }
    }
}