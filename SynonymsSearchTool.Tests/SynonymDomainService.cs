using SynonymsSearchTool.Domain.Services;

namespace SynonymsSearchTool.Tests
{
    public class SynonymDomainServiceTests
    {
        private readonly SynonymDomainService _synonymDomainService;

        public SynonymDomainServiceTests()
        {
            _synonymDomainService = new SynonymDomainService();
        }

        /// <summary>
        /// Test to ensure that a new synonym group is created when the word does not exist.
        /// </summary>
        [Fact]
        public void GetOrCreateGroup_ShouldReturnNewGroup_WhenWordDoesNotExist()
        {
            // Arrange: Define a word that doesn't exist in the synonym group.
            var word = "happy";

            // Act: Call the method to retrieve or create a synonym group for 'happy'.
            var result = _synonymDomainService.GetOrCreateGroup(word);

            // Assert: Ensure that the result is not null and that the word matches the expected word.
            Assert.NotNull(result);              // Should return a valid SynonymList
            Assert.Equal(word, result.Word);     // Word in the SynonymList should match
        }

        /// <summary>
        /// Test to ensure that an existing synonym group is returned when the word already exists.
        /// </summary>
        [Fact]
        public void GetOrCreateGroup_ShouldReturnExistingGroup_WhenWordExists()
        {
            // Arrange: Define a word and create its synonym group first.
            var word = "happy";
            _synonymDomainService.GetOrCreateGroup(word);  // Create the word group first

            // Act: Call the method to retrieve the existing synonym group for 'happy'.
            var result = _synonymDomainService.GetOrCreateGroup(word);  // Retrieve it again

            // Assert: Ensure that the same group is returned and the word matches.
            Assert.NotNull(result);              // Should return a valid SynonymList
            Assert.Equal(word, result.Word);     // Word should match
        }

        /// <summary>
        /// Test to ensure that synonyms are correctly linked between the main word and the synonyms.
        /// </summary>
        [Fact]
        public void LinkSynonyms_ShouldLinkSynonymsCorrectly()
        {
            // Arrange: Define a word and a list of synonyms.
            var word = "happy";
            var synonyms = new List<string> { "joyful", "content", "cheerful" };

            // Act: Link the synonyms to the main word.
            _synonymDomainService.LinkSynonyms(word, synonyms);

            // Assert: Ensure the main word is linked to each synonym and vice versa.
            var mainGroup = _synonymDomainService.GetOrCreateGroup(word);
            foreach (var synonym in synonyms)
            {
                var synonymGroup = _synonymDomainService.GetOrCreateGroup(synonym);
                // Check that the synonym has been added to the main word's group
                Assert.Contains(synonym, mainGroup.Synonyms);
                // Check that the main word has been added to the synonym's group
                Assert.Contains(word, synonymGroup.Synonyms);
            }
        }

        /// <summary>
        /// Test to ensure that groups are created for synonyms when they do not already exist.
        /// </summary>
        [Fact]
        public void LinkSynonyms_ShouldCreateGroupsForSynonyms_WhenNotExist()
        {
            // Arrange: Define a word and a list of synonyms.
            var word = "fast";
            var synonyms = new List<string> { "quick", "speedy" };

            // Act: Link the synonyms to the main word and create groups for the synonyms if they don't exist.
            _synonymDomainService.LinkSynonyms(word, synonyms);

            // Assert: Ensure the main word and synonyms have been linked and groups created for them.
            var mainGroup = _synonymDomainService.GetOrCreateGroup(word);
            Assert.Contains("quick", mainGroup.Synonyms);
            Assert.Contains("speedy", mainGroup.Synonyms);

            // Ensure that synonym groups were created for 'quick' and 'speedy'
            var quickGroup = _synonymDomainService.GetOrCreateGroup("quick");
            Assert.Contains(word, quickGroup.Synonyms);

            var speedyGroup = _synonymDomainService.GetOrCreateGroup("speedy");
            Assert.Contains(word, speedyGroup.Synonyms);
        }

        /// <summary>
        /// Test to ensure that synonyms are not duplicated when linked multiple times.
        /// </summary>
        [Fact]
        public void LinkSynonyms_ShouldNotDuplicateSynonyms()
        {
            // Arrange: Define a word and a list of synonyms.
            var word = "smart";
            var synonyms = new List<string> { "intelligent", "clever" };

            // Act: Link the synonyms to the word, then link them again to check for duplicates.
            _synonymDomainService.LinkSynonyms(word, synonyms);
            _synonymDomainService.LinkSynonyms(word, synonyms);  // Link again to check for duplicates

            // Assert: Ensure no duplicates exist in the synonym groups.
            var mainGroup = _synonymDomainService.GetOrCreateGroup(word);
            Assert.Equal(2, mainGroup.Synonyms.Count);  // Only 2 synonyms should exist, no duplicates

            // Ensure each synonym only appears once in their respective groups
            foreach (var synonym in synonyms)
            {
                var synonymGroup = _synonymDomainService.GetOrCreateGroup(synonym);
                Assert.Equal(1, synonymGroup.Synonyms.Count);  // Each synonym should appear only once
            }
        }
    }
}