using SynonymsSearchTool.Domain.Models;

namespace SynonymsSearchTool.Domain.Services;

/// <summary>
/// Service class that manages the domain logic for synonym groups.
/// Provides methods to retrieve or create synonym groups and to link synonyms together.
/// </summary>
public class SynonymDomainService
{
    // Dictionary to store synonym groups by word.
    // Key is the word, and the value is a SynonymList representing that word's synonym group.
    private readonly Dictionary<string, SynonymGroup> _synonyms;

    /// <summary>
    /// Initializes a new instance of the SynonymDomainService class.
    /// This constructor sets up an in-memory dictionary to hold synonym groups.
    /// </summary>
    public SynonymDomainService()
    {
        // Initialize the dictionary with case-insensitive comparison (StringComparer.OrdinalIgnoreCase).
        // This ensures that words are treated the same regardless of case (e.g., "apple" and "Apple" are considered the same).
        _synonyms = new Dictionary<string, SynonymGroup>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Retrieves the synonym group for the specified word, creating a new group if it does not exist.
    /// </summary>
    /// <param name="word">The word for which to get or create a synonym group.</param>
    /// <returns>The SynonymList corresponding to the word.</returns>
    public SynonymGroup GetOrCreateGroup(string word)
    {
        // Try to find the word in the dictionary. If it's not found, create a new SynonymList.
        if (!_synonyms.TryGetValue(word, out var group))
        {
            // Create a new SynonymList for the word if it doesn't already exist.
            group = new SynonymGroup(word);
            // Add the new SynonymList to the dictionary.
            _synonyms[word] = group;
        }

        // Return the existing or newly created synonym group for the word.
        return group;
    }

    /// <summary>
    /// Links the specified synonyms to the given word, creating any necessary synonym groups.
    /// This method ensures bidirectional linking between the word and its synonyms.
    /// </summary>
    /// <param name="word">The word to link with the given synonyms.</param>
    /// <param name="synonyms">The list of synonyms to link with the word.</param>
    public void LinkSynonyms(string word, IEnumerable<string> synonyms)
    {
        // Retrieve or create the synonym group for the main word.
        var mainGroup = GetOrCreateGroup(word);

        // Loop through each synonym in the list of synonyms.
        foreach (var synonym in synonyms)
        {
            // For each synonym, retrieve or create its own synonym group.
            var synonymGroup = GetOrCreateGroup(synonym);

            // Add the synonym to the main word's synonym group.
            mainGroup.AddSynonym(synonym);

            // Add the original word to the synonym's synonym group (creating bidirectional relationships).
            synonymGroup.AddSynonym(word);
        }
    }
}