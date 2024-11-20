namespace SynonymsSearchTool.Api.Models;

/// <summary>
/// Response for retrieving or representing synonyms of a word.
/// </summary>
public class SynonymsResponse
{
    /// <summary>
    /// A list of synonyms associated with a word.
    /// The list is optional and may be null.
    /// </summary>
    public List<string>? Synonyms { get; set; }
}
