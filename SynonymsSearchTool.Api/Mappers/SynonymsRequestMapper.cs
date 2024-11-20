using SynonymsSearchTool.Api.Models;
using SynonymsSearchTool.Application.DTOs;

namespace SynonymsSearchTool.Api.Mappers;

/// <summary>
/// Provides extension methods for mapping API request models to DTOs.
/// </summary>
internal static class SynonymsRequestMapper
{
    /// <summary>
    /// Maps a SaveSynonymsRequest object to a SaveSynonymsDto object.
    /// This is an extension method for the SaveSynonymsRequest class.
    /// </summary>
    /// <param name="request">The SaveSynonymsRequest object that needs to be mapped to a DTO.</param>
    /// <returns>A new instance of SaveSynonymsDto with mapped values from the request.</returns>
    internal static SaveSynonymsDto ToDto(this SaveSynonymsRequest request) => new()
    {
        Word = request.Word,
        Synonyms = request.Synonyms
    };
}