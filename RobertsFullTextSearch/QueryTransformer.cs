using System;
using System.Collections.Generic;
using System.Linq;

namespace RobertsFullTextSearch
{
    internal class QueryTransformer
    {
        /// <summary>
        /// 
        /// </summary>
        enum QueryState 
        { 
            Starting, 
            Anding, 
            Oring 
        };


        /// <summary>
        /// Transform a list of phrases into a full text search predicate
        /// </summary>
        /// <param name="SearchTerms"></param>
        /// <returns>FTS query suitable for use with CONTAINS(..) or CONTAINSTABLE(..)</returns>
        public string TransformIntoFullTextQuery(IList<string> SearchTerms)
        {
            //
            // Move exclusion terms to the end
            //  - The FTS CONTAINS syntax does not allow a query to begin 
            //    with AND NOT (there is not "NOT" operator, there is an
            //    "AND NOT" operator).
            //
            SearchTerms = CleanupExclusionTerms(SearchTerms);
            if (SearchTerms == null || !SearchTerms.Any())
            {
                return "";
            }

            //
            // Handle each set of terms
            //  - terms that are being AND'd are added to a list of common terms
            //    being AND'd and then finally converted to an fts predicate when
            //    an OR is encountered or the end of the terms is encountered
            //
            var OpenedParenthesisCount = 0;
            var State = QueryState.Starting;
            var FtsQuery = "";
            var TermsEnumerator = SearchTerms.GetEnumerator().AsPeekable();
            while (TermsEnumerator.MoveNext())
            {
                var ThisTerm = TermsEnumerator.Current;

                //
                // Drop any "OR"'s as they are handled via "peeking"
                //
                if (IsOr(ThisTerm))
                {
                    continue;
                }

                //
                // Check if the next term is an OR
                //
                var NextTerm = TermsEnumerator.Peek;
                var NextTermIsOr = IsOr(NextTerm);

                //
                // If we're just begining initialize the query and mark that we're ANDng or ORng
                //
                if (State == QueryState.Starting)
                {
                    if (!NextTermIsOr)
                    {
                        FtsQuery = SearchTermToFullTextPredicate(ThisTerm);
                        State = QueryState.Anding;
                    }
                    else
                    {
                        FtsQuery = "(";
                        FtsQuery += SearchTermToFullTextPredicate(ThisTerm);
                        State = QueryState.Oring;
                        OpenedParenthesisCount += 1;
                    }

                    continue;
                }

                //
                // If we're adding to a sequence of ANDs
                //
                if (State == QueryState.Anding)
                {
                    if (NextTermIsOr == false)
                    {
                        FtsQuery += " AND ";
                        FtsQuery += SearchTermToFullTextPredicate(ThisTerm);
                    }
                    else
                    {
                        FtsQuery += " AND (";
                        FtsQuery += SearchTermToFullTextPredicate(ThisTerm);
                        State = QueryState.Oring;
                        OpenedParenthesisCount += 1;
                    }

                    continue;
                }

                //
                // We're adding to a sequnce of OR's
                //  - FTS can't handle "running or -leaping" so we change it to just "running"
                //
                if (!IsNot(ThisTerm))
                {
                    FtsQuery += " OR ";
                    FtsQuery += SearchTermToFullTextPredicate(ThisTerm);
                }

                //
                // If the next term isn't an OR then we've reach the end
                // of a sequence of OR's so close parenthesis and go back
                // to AND'ng
                //
                if (!NextTermIsOr)
                {
                    FtsQuery += ")";
                    State = QueryState.Anding;
                    OpenedParenthesisCount -= 1;
                }
            }

            if (OpenedParenthesisCount != 0)
            {
                FtsQuery += ")";
            }

            return FtsQuery;
        }


        private bool IsNot(string v)
        {
            return v.StartsWith('-');
        }


        private bool IsOr(string v)
        {
            return string.Compare(v, "or", StringComparison.OrdinalIgnoreCase) == 0;
        }


        // TODO: I think there's a bug involving how exclusions are shifted to
        //       then end of the terms... -A OR -B C D after shift is OR -B C D -A?

        private IList<string> CleanupExclusionTerms(IList<string> SearchTerms)
        {
            //
            // No terms? 
            //
            if (SearchTerms == null || !SearchTerms.Any())
            {
                return null;
            }

            //
            // No exclusions?
            //
            if (!SearchTerms.Select(t => IsNot(t)).Any())
            {
                return SearchTerms;
            }

            //
            // Check if there are only exclusions
            //  - FTS query syntax requires at least one non-exlusion term
            //
            var ContainsOnlyNots = (from t in SearchTerms where IsNot(t) select 1).Count() == SearchTerms.Count();
            if (ContainsOnlyNots)
            {
                return null;
            }

            //
            // Move any exclusions at the start to the end
            //  - This can't be an infinite loop because there's at least one term
            //
            while (IsNot(SearchTerms.First()))
            {
                var f = SearchTerms.First();
                SearchTerms = SearchTerms.Skip(1).ToList();
                SearchTerms.Add(f);
            }

            return SearchTerms;
        }

        


        /// <summary>
        /// Convert a single search term into the appropriate full text predicate.
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        private static string SearchTermToFullTextPredicate(string SearchTerm)
        {
            // term starts with "-" becomes [NOT FORMSOF(INFLECTIONAL, ..)]
            // term is quoted becomes [".."]
            // term starts with "+" becomes [".."]
            // term ends with * becomes [".."]

            if (SearchTerm.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                return $"\"{EscapeQuotes(SearchTerm[1..^1].Trim())}\"";
            }
            else if (SearchTerm.StartsWith("+", StringComparison.OrdinalIgnoreCase))
            {
                return $"\"{EscapeQuotes(SearchTerm.Substring(1))}\"";
            }
            else if (SearchTerm.StartsWith("-", StringComparison.OrdinalIgnoreCase))
            {
                if (SearchTerm[1] == '"')
                {
                    return $"NOT \"{EscapeQuotes(SearchTerm[2..^1].Trim())}\"";
                }
                else
                {
                    return $"NOT FORMSOF(INFLECTIONAL, {EscapeSearchTerm(SearchTerm.Substring(1))})";
                }
            }
            else if (SearchTerm.EndsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                return $"\"{EscapeQuotes(SearchTerm)}\"";
            }
            else
            {
                if (!TermShouldAlsoIncludePrefixMatch(SearchTerm))
                {
                    return $"FORMSOF(INFLECTIONAL, {EscapeSearchTerm(SearchTerm)})";
                }
                else
                {
                    return
                        $"(FORMSOF(INFLECTIONAL, {EscapeSearchTerm(SearchTerm)}) OR {EscapeSearchTerm(SearchTerm + "*")})";
                }
            }
        }


        /// <summary>
        /// FTS doesn't handle words that contain punctuation and digits well so
        /// if a term has anything other than letters in it the query should also
        /// include a prefix match.
        /// </summary>
        /// <param name="Term"></param>
        /// <returns></returns>
        private static bool TermShouldAlsoIncludePrefixMatch(string Term)
        {
            foreach (var c in Term)
            {
                if (!char.IsLetter(c))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Escape a search term if it contains anything other than digits or letters
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        private static string EscapeSearchTerm(string SearchTerm)
        {
            var EscapingRequired = false;
            foreach (var c in SearchTerm)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    EscapingRequired = true;
                    break;
                }
            }
            if (!EscapingRequired)
            {
                return SearchTerm;
            }
            else
            {
                return $"\"{EscapeQuotes(SearchTerm)}\"";
            }
        }


        /// <summary>
        /// If the string contains doublequotes quote them (ie double them up). This is
        /// just being overly cautious because quotes in search terms will never make
        /// it to the actual FTS query. But better safe than sorry.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string EscapeQuotes(string s)
        {
            if (s.IndexOf('"') == -1)
            {
                return s;
            }
            else
            {
                return s.Replace("\"", "\"\"");
            }
        }
    }
}
