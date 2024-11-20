namespace SynonymsSearchTool.Application.DTOs;

/// <summary>
/// DTO for saving a word and its synonyms.
/// </summary>
public class SaveSynonymsDto
{
/// <summary>
/// The main word for which synonyms are being saved.
/// This property is required.
/// </summary>
public required string Word { get; set; }

/// <summary>
/// A list of synonyms associated with the main word.
/// This property is required.
/// </summary>
public required List<string> Synonyms { get; set; }
}
