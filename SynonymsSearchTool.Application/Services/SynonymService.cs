using SynonymsSearchTool.Application.DTOs;
using SynonymsSearchTool.Application.Interfaces;
using SynonymsSearchTool.Application.Mappers;
using SynonymsSearchTool.Application.Validation;
using SynonymsSearchTool.Domain.Models;

namespace SynonymsSearchTool.Application.Services
{
    /// <summary>
    /// Service class that manages synonyms for words. This class provides methods for retrieving and saving synonyms.
    /// It interacts with an in-memory dictionary to store word-synonym relationships.
    /// </summary>
    public class SynonymService : ISynonymService
    {
        private readonly Dictionary<string, HashSet<string>> _synonyms;

        /// <summary>
        /// Initializes a new instance of the SynonymService class.
        /// It sets up the in-memory dictionary to store word-synonym relationships.
        /// </summary>
        public SynonymService()
        {
            // The dictionary is case-insensitive
            _synonyms = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Asynchronously retrieves synonyms for a given word, including transitive synonyms.
        /// If synonyms are found, they are returned as a DTO (SynonymsDto).
        /// If no synonyms are found, an empty SynonymsDto with an empty synonym list is returned.
        /// </summary>
        /// <param name="word">The word for which synonyms are to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation, with a SynonymsDto as the result.</returns>
        public Task<SynonymsDto> GetSynonymsAsync(string word)
        {
            // Check if the word exists in the dictionary and retrieve its related synonyms
            if (_synonyms.TryGetValue(word, out var relatedWords))
            {
                // Create a new SynonymList object for the word
                var synonymList = new SynonymGroup(word);
                var allSynonyms = new HashSet<string>(relatedWords);

                // Fetch transitive synonyms
                foreach (var relatedWord in relatedWords)
                {
                    allSynonyms.UnionWith(GetTransitiveSynonyms(relatedWord, []));
                }

                // Add each related word (synonym) to the SynonymList
                foreach (var synonym in allSynonyms)
                {
                    synonymList.AddSynonym(synonym);
                }

                // Return the SynonymList as a DTO using the ToDto method
                return Task.FromResult(synonymList.ToDto());
            }

            // If no synonyms are found for the word, return an empty SynonymsDto with an empty list
            return Task.FromResult(new SynonymsDto { Synonyms = [] });
        }

        /// <summary>
        /// Asynchronously saves synonyms for a given word.
        /// Validates the save request and updates the internal dictionary with the provided synonyms.
        /// </summary>
        /// <param name="dto">The DTO containing the word and its synonyms to be saved.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task SaveSynonymsAsync(SaveSynonymsDto dto)
        {
            // Validate the DTO to ensure that the save request is valid (e.g., no duplicates, word exists, etc.)
            SynonymValidator.ValidateSaveRequest(dto.Word, dto.Synonyms);

            // Convert the SaveSynonymsDto to a SynonymList domain model
            var synonymList = DomainToDtoMapper.ToDomain(dto);

            // Create a unified set of all words in the group (word and its synonyms).
            var allWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                synonymList.Word
            };
            allWords.UnionWith(synonymList.Synonyms);

            // Add each word to the dictionary, ensuring bidirectional relationships.
            foreach (var word in allWords)
            {
                if (!_synonyms.TryGetValue(word, out var wordSynonyms))
                {
                    wordSynonyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _synonyms[word] = wordSynonyms;
                }
                wordSynonyms.UnionWith(allWords);
            }

            return Task.CompletedTask; // Indicate the task is complete.
        }

        /// <summary>
        /// Recursively retrieves transitive synonyms for a word.
        /// </summary>
        /// <param name="word">The word for which transitive synonyms are being fetched.</param>
        /// <param name="visited">Set of words already visited to prevent infinite loops.</param>
        /// <returns>A set of transitive synonyms.</returns>
        private HashSet<string> GetTransitiveSynonyms(string word, HashSet<string> visited)
        {
            var transitiveSynonyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // If the word has already been visited, return an empty set to avoid cycles.
            if (visited.Contains(word))
                return transitiveSynonyms;

            // Mark the word as visited.
            visited.Add(word);

            // Check if the word exists in the dictionary.
            if (_synonyms.TryGetValue(word, out var relatedWords))
            {
                foreach (var relatedWord in relatedWords)
                {
                    // Add the related word to the transitive synonyms.
                    transitiveSynonyms.Add(relatedWord);

                    // Recursively fetch transitive synonyms for the related word.
                    transitiveSynonyms.UnionWith(GetTransitiveSynonyms(relatedWord, visited));
                }
            }

            return transitiveSynonyms; // Return the full set of transitive synonyms.
        }
    }
}