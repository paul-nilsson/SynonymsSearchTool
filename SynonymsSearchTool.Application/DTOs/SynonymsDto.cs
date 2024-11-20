namespace SynonymsSearchTool.Application.DTOs;

/// <summary>
/// DTO for retrieving or representing synonyms of a word.
/// </summary>
public class SynonymsDto
{
    /// <summary>
    /// A list of synonyms associated with a word.
    /// The list is optional and may be null.
    /// </summary>
    public List<string>? Synonyms { get; set; }
}