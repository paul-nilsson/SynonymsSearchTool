using SynonymsSearchTool.Api.Models;
using SynonymsSearchTool.Application.DTOs;

namespace SynonymsSearchTool.Api.Mappers;

/// <summary>
/// Provides extension methods for mapping API response models to DTOs.
/// </summary>
internal static class SynonymsResponseMapper
{
    /// <summary>
    /// Maps a SaveSynonymsRequest object to a SaveSynonymsDto object.
    /// </summary>
    /// <param name="request">The SaveSynonymsRequest object that needs to be mapped to a DTO.</param>
    /// <returns>A new instance of SaveSynonymsDto with mapped values from the request.</returns>
    internal static SynonymsResponse ToResponse(this SynonymsDto dto) => new()
    {
        Synonyms = dto.Synonyms
    };
}
