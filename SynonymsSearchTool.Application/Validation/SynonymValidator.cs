namespace SynonymsSearchTool.Application.Validation;

/// <summary>
/// A utility class for validating synonym-related operations.
/// Contains methods for validating inputs related to synonyms, such as ensuring no invalid words or duplicates.
/// </summary>
public static class SynonymValidator
{
    /// <summary>
    /// Validates the save request for synonyms.
    /// This method checks that the word and its synonyms meet certain validation criteria:
    /// 1. The word must not be null or whitespace.
    /// 2. The synonyms list must not be null or empty and cannot contain any null, empty, or whitespace values.
    /// 3. The synonyms list cannot contain the word itself.
    /// </summary>
    /// <param name="word">The word for which synonyms are being saved.</param>
    /// <param name="synonyms">The list of synonyms to be validated.</param>
    /// <exception cref="ValidationException">Thrown if any validation rule fails.</exception>
    public static void ValidateSaveRequest(string word, IEnumerable<string> synonyms)
    {
        // Check if the word is null, empty, or consists only of whitespace
        if (string.IsNullOrWhiteSpace(word))
            throw new ValidationException("Word cannot be null or whitespace.");

        // Check if synonyms is null, empty, or contains any null, empty, or whitespace values
        if (synonyms == null || !synonyms.Any() || synonyms.Any(s => string.IsNullOrWhiteSpace(s)))
            throw new ValidationException("Synonyms list cannot contain null, empty, or whitespace values.");

        // Check if the synonyms list contains the word itself (synonym list cannot contain the word itself)
        if (synonyms.Any(s => s.Equals(word, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException("Synonyms list cannot contain the word itself.");
    }
}

/// <summary>
/// Custom exception class for validation errors.
/// Thrown when an invalid operation or data is encountered during validation.
/// </summary>
public class ValidationException(string message) : Exception(message) { }