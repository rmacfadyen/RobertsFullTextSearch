using System;
using System.Collections.Generic;

namespace RobertsFullTextSearch
{
    internal class QueryParser
    {
        /// <summary>
        /// Parse a search string into descrete phrases, paying attention to
        /// quoted strings.
        /// </summary>
        /// <param name="SearchCriteria"></param>
        /// <returns></returns>
        public IList<string> SplitIntoSearchTerms(string SearchCriteria)
        {
            //
            // Split the search string into phrases, a quoted string counts as one word
            //
            var Phrases = new List<string>();
            
            var CurrentPhrase = string.Empty;
            var State = ParseStates.StartOfPhrase;
            for (var i = 0; i < SearchCriteria.Length; i += 1)
            {
                var c = SearchCriteria[i];
                var NextChar = i + 1 < SearchCriteria.Length ? SearchCriteria[i + 1] : -1;

                switch (State)
                {
                    //
                    // Ignore whitespace at the start of a phrase
                    //
                    case ParseStates.StartOfPhrase when char.IsWhiteSpace(c):
                        break;
                    
                    //
                    // Begin a phrase... might quoted or single word
                    //
                    case ParseStates.StartOfPhrase:
                        CurrentPhrase = string.Empty;
                        CurrentPhrase += c;

                        if (c == '"')
                        {
                            State = ParseStates.QuotedText;
                        }
                        else if (c == '-' && NextChar == 34)
                        {
                            State = ParseStates.QuotedText;
                            CurrentPhrase += '"';
                            i += 1;
                        }
                        else
                        {
                            State = ParseStates.Text;
                        }

                        break;
                    
                    //
                    // Add current character to the phrase until whitespace or closing quote
                    //
                    case ParseStates.Text when !char.IsWhiteSpace(c) && c != '"':
                        CurrentPhrase += c;
                        break;

                    //
                    // Phrase has been captured, add it to the list and switch to start or quoted
                    //
                    case ParseStates.Text:
                        if (CurrentPhrase.Trim() != string.Empty)
                        {
                            Phrases.Add(CurrentPhrase.Trim());
                        }

                        if (char.IsWhiteSpace(c))
                        {
                            State = ParseStates.StartOfPhrase;
                            CurrentPhrase = string.Empty;
                        }
                        else
                        {
                            State = ParseStates.QuotedText;
                            CurrentPhrase = "\"";
                        }

                        break;
                    
                    default:
                        CurrentPhrase += c;

                        if (c == '"')
                        {
                            Phrases.Add(CurrentPhrase.Trim());
                            State = ParseStates.StartOfPhrase;
                        }

                        break;
                }
            }

            //
            // Add the last phrase if necesary
            //
            if (State != ParseStates.StartOfPhrase)
            {
                if (CurrentPhrase != string.Empty)
                {
                    //
                    // Unterminated quoted string
                    //
                    if (CurrentPhrase.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentPhrase += '"';
                    }

                    if (CurrentPhrase != "\"\"")
                    {
                        Phrases.Add(CurrentPhrase.Trim());
                    }
                }
            }

            //
            // Lastly tidy up the list of phrases
            //
            return Phrases;
        }
    }
}
