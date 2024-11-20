using SynonymsSearchTool.Application.DTOs;
using SynonymsSearchTool.Application.Services;

namespace SynonymsSearchTool.Tests;

public class SynonymServiceTests
{
    private readonly SynonymService _synonymService;

    public SynonymServiceTests()
    {
        _synonymService = new SynonymService();
    }

    #region GetSynonymsAsync Tests

    /// <summary>
    /// Tests that GetSynonymsAsync returns the correct synonyms when the word exists in the dictionary.
    /// </summary>
    [Fact]
    public async Task GetSynonymsAsync_ShouldReturnSynonyms_WhenWordExists()
    {
        // Arrange: Define a word and manually add its synonyms to the internal dictionary.
        var word = "happy";
        var synonyms = new HashSet<string> { "joyful", "content", "cheerful" };

        // Accessing and manipulating the internal _synonyms dictionary directly using reflection.
        var internalField = typeof(SynonymService)
            .GetField("_synonyms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var synonymsDict = (Dictionary<string, HashSet<string>>)internalField.GetValue(_synonymService);
        synonymsDict[word] = synonyms;

        // Act: Call the method to get the synonyms for the word.
        var result = await _synonymService.GetSynonymsAsync(word);

        // Assert: Check if the result is not null and contains the expected synonyms.
        Assert.NotNull(result);
        Assert.Contains("joyful", result.Synonyms);
        Assert.Contains("content", result.Synonyms);
        Assert.Contains("cheerful", result.Synonyms);
    }

    /// <summary>
    /// Tests that GetSynonymsAsync returns an empty list of synonyms when the word does not exist.
    /// </summary>
    [Fact]
    public async Task GetSynonymsAsync_ShouldReturnEmpty_WhenWordDoesNotExist()
    {
        // Arrange: Define a word that does not exist in the synonyms dictionary.
        var word = "nonexistent";

        // Act: Call the method to get synonyms for the nonexistent word.
        var result = await _synonymService.GetSynonymsAsync(word);

        // Assert: Ensure that the result is not null and contains an empty list of synonyms.
        Assert.NotNull(result);
        Assert.NotNull(result.Synonyms);  // Verify that the synonyms collection is not null
        Assert.Empty(result.Synonyms);    // Ensure the collection is empty
    }

    #endregion

    #region SaveSynonymsAsync Tests

    /// <summary>
    /// Tests that SaveSynonymsAsync saves the synonyms correctly when given a valid DTO.
    /// </summary>
    [Fact]
    public async Task SaveSynonymsAsync_ShouldSaveSynonyms_WhenValidDtoIsGiven()
    {
        // Arrange: Define a word and its list of synonyms to be saved.
        var word = "fast";
        var synonyms = new List<string> { "quick", "swift", "speedy" };
        var dto = new SaveSynonymsDto
        {
            Word = word,
            Synonyms = synonyms
        };

        // Act: Call the method to save the synonyms.
        await _synonymService.SaveSynonymsAsync(dto);

        // Accessing the internal _synonyms field using reflection to check the internal state.
        var internalField = typeof(SynonymService)
            .GetField("_synonyms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var synonymsDict = (Dictionary<string, HashSet<string>>)internalField.GetValue(_synonymService);

        // Assert: Ensure the word and its synonyms are correctly saved in the internal dictionary.
        Assert.True(synonymsDict.ContainsKey(word));   // Ensure the word is added to the dictionary.
        foreach (var synonym in synonyms)
        {
            Assert.True(synonymsDict[word].Contains(synonym));  // Ensure each synonym is associated with the word.
        }

        // Also check the reverse (each synonym should have the word as a synonym).
        foreach (var synonym in synonyms)
        {
            Assert.True(synonymsDict.ContainsKey(synonym));  // Ensure the synonym is added to the dictionary.
            Assert.True(synonymsDict[synonym].Contains(word));  // Ensure the word is added as a synonym to each synonym.
        }
    }

    /// <summary>
    /// Tests that SaveSynonymsAsync does not add duplicate synonyms.
    /// </summary>
    [Fact]
    public async Task SaveSynonymsAsync_ShouldNotDuplicateSynonyms()
    {
        // Arrange: Define a word and its list of synonyms.
        var word = "beautiful";
        var synonyms = new List<string> { "pretty", "gorgeous", "lovely" };
        var dto = new SaveSynonymsDto
        {
            Word = word,
            Synonyms = synonyms
        };

        // Act: Save the synonyms once.
        await _synonymService.SaveSynonymsAsync(dto);

        // Act: Save the same synonyms again to check for duplication.
        await _synonymService.SaveSynonymsAsync(dto);

        // Access the internal _synonyms field to inspect the dictionary after saving again.
        var internalField = typeof(SynonymService)
            .GetField("_synonyms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var synonymsDict = (Dictionary<string, HashSet<string>>)internalField.GetValue(_synonymService);

        // Assert: Ensure that the word only has the expected number of unique synonyms.
        Assert.Equal(3, synonymsDict[word].Count);  // The word should have exactly 3 synonyms (no duplicates).
    }

    #endregion
}