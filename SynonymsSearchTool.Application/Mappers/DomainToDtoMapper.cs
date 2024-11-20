using SynonymsSearchTool.Application.DTOs;
using SynonymsSearchTool.Domain.Models;

namespace SynonymsSearchTool.Application.Mappers;

/// <summary>
/// Provides mapping functions for converting domain models to DTOs and vice versa.
/// </summary>
internal static class DomainToDtoMapper
{
    /// <summary>
    /// Maps a <see cref="SynonymGroup"/> (domain model) to a <see cref="SynonymsDto"/> (DTO).
    /// </summary>
    /// <param name="synonymList">The domain model to be converted to a DTO.</param>
    /// <returns>A <see cref="SynonymsDto"/> representing the synonyms in the provided <see cref="SynonymGroup"/>.</returns>
    internal static SynonymsDto ToDto(this SynonymGroup synonymList) => new()
    {
        Synonyms = [.. synonymList.Synonyms]
    };

    /// <summary>
    /// Maps a <see cref="SaveSynonymsDto"/> (DTO) to a <see cref="SynonymGroup"/> (domain model).
    /// </summary>
    /// <param name="dto">The SaveSynonymsDto to be converted to a SynonymList.</param>
    /// <returns>A <see cref="SynonymGroup"/> object representing the synonyms from the provided DTO.</returns>
    internal static SynonymGroup ToDomain(this SaveSynonymsDto dto) => new(dto.Word)
    {
        // Convert the list of synonyms from the DTO (List<string>) to a HashSet<string> in the domain model
        // HashSet is used to ensure uniqueness of synonyms (duplicates are removed)
        Synonyms = new HashSet<string>(dto.Synonyms)
    };
}