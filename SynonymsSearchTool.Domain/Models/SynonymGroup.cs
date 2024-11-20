namespace SynonymsSearchTool.Domain.Models;

/// <summary>
/// Represents a group of synonyms for a specific word.
/// This class encapsulates the word and its associated synonyms.
/// </summary>
public class SynonymGroup
{
    /// <summary>
    /// Gets the word for which synonyms are being stored.
    /// </summary>
    public string Word { get; private set; }

    /// <summary>
    /// A case-insensitive collection of synonyms for the word.
    /// This collection ensures that each synonym is unique (no duplicates).
    /// </summary>
    public HashSet<string> Synonyms = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="SynonymGroup"/> class.
    /// </summary>
    /// <param name="word">The word for which synonyms will be stored. Cannot be null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the word is null or empty.</exception>
    public SynonymGroup(string word)
    {
        // Ensures that the word is not null or empty. If it is, throws an ArgumentNullException.
        Word = word ?? throw new ArgumentNullException(nameof(word));
    }

    /// <summary>
    /// Adds a synonym to the synonym group for the word.
    /// Synonyms are only added if they are not empty and are not the same as the word itself.
    /// </summary>
    /// <param name="synonym">The synonym to be added to the list.</param>
    /// <exception cref="ArgumentException">Thrown if the synonym is null, empty, or identical to the word.</exception>
    public void AddSynonym(string synonym)
    {
        // Check if the synonym is null, empty, or consists of only whitespace.
        if (string.IsNullOrWhiteSpace(synonym))
            throw new ArgumentException("Synonym cannot be null or empty.", nameof(synonym));

        // Ensure that the synonym is not the same as the word itself (case-insensitive check).
        if (!synonym.Equals(Word, StringComparison.OrdinalIgnoreCase))
        {
            // Add the synonym to the set (ensures uniqueness and case-insensitivity).
            Synonyms.Add(synonym);
        }
    }
}