using SynonymsSearchTool.Application.DTOs;

namespace SynonymsSearchTool.Application.Interfaces
{
    /// <summary>
    /// Defines a contract for managing synonyms in the application.
    /// </summary>
    public interface ISynonymService
    {
        /// <summary>
        /// Retrieves synonyms for a specified word asynchronously.
        /// </summary>
        /// <param name="word">The word for which synonyms are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="SynonymsDto"/> with the synonyms.</returns>
        public Task<SynonymsDto> GetSynonymsAsync(string word);

        /// <summary>
        /// Saves a set of synonyms for a given word asynchronously.
        /// </summary>
        /// <param name="dto">The data transfer object that contains the synonyms to be saved.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public Task SaveSynonymsAsync(SaveSynonymsDto dto);
    }
}