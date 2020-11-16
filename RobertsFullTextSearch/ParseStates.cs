namespace RobertsFullTextSearch
{
    /// <summary>
    /// States for the parsing of strings that may contain quoted strings
    /// </summary>
    internal enum ParseStates
    {
        Text,
        QuotedText,
        StartOfPhrase
    }

}
