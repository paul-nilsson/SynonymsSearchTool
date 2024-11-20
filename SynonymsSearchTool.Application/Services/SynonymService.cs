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
        /// Asynchronously retrieves synonyms for a given word.
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

                // Add each related word (synonym) to the SynonymList
                foreach (var relatedWord in relatedWords)
                {
                    synonymList.AddSynonym(relatedWord);
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

            // Check if the word already exists in the dictionary; if not, create a new HashSet for its synonyms
            if (!_synonyms.TryGetValue(synonymList.Word, out var wordSynonyms))
            {
                wordSynonyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _synonyms[synonymList.Word] = wordSynonyms;
            }

            // Add each synonym from the SynonymList to the dictionary
            foreach (var synonym in synonymList.Synonyms)
            {
                wordSynonyms.Add(synonym);

                // For each synonym, ensure it is also added to the dictionary if it doesn't already exist
                if (!_synonyms.TryGetValue(synonym, out var synonymGroup))
                {
                    synonymGroup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _synonyms[synonym] = synonymGroup;
                }

                // Add the original word as a synonym to the synonym group's list (ensure bidirectional synonym relationship)
                synonymGroup.Add(synonymList.Word);
            }

            return Task.CompletedTask;
        }
    }
}