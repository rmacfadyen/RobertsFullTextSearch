using System;
using System.Collections.Generic;
using System.Linq;

namespace RobertsFullTextSearch
{

    public interface IQueryCreator
    {
        public string ToFtsQuery(string SearchCriteria);
        public HashSet<string> StopWords { get; set; }
    }


    public class QueryCreator : IQueryCreator
    {
        public HashSet<string> StopWords { get; set; } = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SearchCriteria"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public string ToFtsQuery(string SearchCriteria)
        {
            //
            // Split the search query into individual terms
            //
            var Parser = new QueryParser();
            var SearchTerms = Parser.SplitIntoSearchTerms(SearchCriteria);
            if (!SearchTerms.Any())
            {
                return string.Empty;
            }

            //
            // Remove extranous terms
            //
            SearchTerms = CleanupSearchTerms(SearchTerms);
            if (!SearchTerms.Any())
            {
                return string.Empty;
            }

            //
            // Transform the terms into full text query predicate
            //
            var Transformer = new QueryTransformer();
            return Transformer.TransformIntoFullTextQuery(SearchTerms);
        }
        

        /// <summary>
        /// Remove a number of edge cases (AND's are dropped, multiple OR's combined, etc)
        /// </summary>
        /// <param name="SearchTerms"></param>
        private IList<string> CleanupSearchTerms(IList<string> SearchTerms)
        {
            //
            // Remove extraneous terms and switch |'s to OR's
            //  - Empty terms, stop words, the word "AND", single asterisks,  characters * and -
            //
            var Terms =
                (from Term in SearchTerms
                    where !string.IsNullOrWhiteSpace(Term)
                          && !IsStopWord(Term)
                          && !IsAnd(Term)
                          && !IsStar(Term)
                          && !IsMinus(Term)
                    select IsVBar(Term) ? "OR" : Term).ToList();

            //
            // Remove OR's that are followed by OR's (eg. "sam OR OR bill")
            //
            bool Changed;
            do
            {
                Changed = false;
                for (var i = 0; i < Terms.Count - 2; i += 1)
                {
                    if (IsOr(Terms[i]) && IsOr(Terms[i + 1]))
                    {
                        Terms.RemoveAt(i + 1);
                        Changed = true;
                        break;
                    }
                }
            } while (Changed == true);

            //
            // If there are no remaining search terms we're done
            //
            if (Terms.Count == 0)
            {
                return Terms;
            }

            //
            // Remove any trailing OR's
            //
            if (IsOr(Terms[^1]))
            {
                Terms.RemoveAt(Terms.Count - 1);
            }

            //
            // If there are no remaining search terms we're done
            //
            if (Terms.Count == 0)
            {
                return Terms;
            }

            //
            // Remove any leading OR's
            //
            if (IsOr(Terms[0]))
            {
                Terms.RemoveAt(0);
            }

            return Terms;
        }


        private static bool IsAnd(string Term) => string.Compare(Term, "and", StringComparison.OrdinalIgnoreCase) == 0;
        private static bool IsOr(string Term) => string.Compare(Term, "or", StringComparison.OrdinalIgnoreCase) == 0;
        private static bool IsStar(string Term) => string.Compare(Term, "*", StringComparison.Ordinal) == 0;
        private static bool IsMinus(string Term) => string.Compare(Term, "-", StringComparison.Ordinal) == 0;
        private static bool IsVBar(string Term) => string.Compare(Term, "|", StringComparison.Ordinal) == 0;


        /// <summary>
        /// Checks if the given search term is a stop word. Handles quoted terms,
        /// "+" terms, and "-" terms.
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <param name="StopWords"></param>
        /// <param name="Comparer"></param>
        /// <returns></returns>
        private bool IsStopWord(string SearchTerm)
        {
            string Word;

            if (string.Compare(SearchTerm, "OR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return false;
            }

            if (SearchTerm.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                Word = SearchTerm[1..^1].Trim();
            }
            else if (SearchTerm.StartsWith("+", StringComparison.OrdinalIgnoreCase))
            {
                Word = SearchTerm.Substring(1);
            }
            else if (SearchTerm.StartsWith("-", StringComparison.OrdinalIgnoreCase))
            {
                Word = SearchTerm.Substring(1);
            }
            else
            {
                Word = SearchTerm;
            }

            return StopWords.Contains(Word);
        }

    }
}
