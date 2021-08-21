using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RobertsFullTextSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobertsFullTextSearchDemo.Pages
{
    public class IndexModel : PageModel
    {
        //
        // List of noise/stop words originally from SQL Server 2005
        //
        private static readonly HashSet<string> WordsToIgnore = new()
        {
            "$", 
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "about", "after", "all", "also", "an", "and", "another", "any", "are", "as", "at", 
            "be", "because", "been", "before", "being", "between", "both", "but", "by", 
            "came", "can", "come", "could", 
            "did", "do", "does", 
            "each", "else", 
            "for", "from", 
            "get", "got", 
            "had", "has", "have", "he", "her", "here", "him", "himself", "his", "how", 
            "if", "in", "into", "is", "it", "its", 
            "just", 
            "like", 
            "make", "many", "me", "might", "more", "most", "much", "must", "my", 
            "never", "no", "now", 
            "of", "on", "only", "or", "other", "our", "out", "over", 
            "re", 
            "said", "same", "see", "should", "since", "so", "some", "still", "such", 
            "take", "than", "that", "the", "their", "them", "then", "there", "these", "they", "this", "those", "through", "to", "too", 
            "under", "up", "use", 
            "very", 
            "want", "was", "way", "we", "well", "were", "what", "when", "where", "which", "while", "who", "will", "with", "would", 
            "you", "your"
        };


        #region Bind properties

        [BindProperty] public string SearchFor { get; set; }

        #endregion


        #region View model

        public bool HaveResult { get; set; }
        public string Result { get; set; }
        #endregion


        public IndexModel()
        {
        }


        public void OnGet()
        {
        }


        public void OnPost()
        {
            //
            // Wrap everything in try/catch just for caution
            //  - No exceptions are expected, but for a demo a
            //    "something when wrong" message is preferred
            //    over the stock error page
            //
            try
            {
                TranslateQuery();
            }
            catch
            {
                HaveResult = true;
                Result = "Something went wrong.";
            }
        }


        private void TranslateQuery()
        {
            HaveResult = true;

            if (string.IsNullOrWhiteSpace(SearchFor))
            {
                Result = "Search resulted in an empty query";
            }
            else
            {
                //
                // Translate the query
                //
                var fts = new QueryCreator
                {
                    StopWords = WordsToIgnore
                };
                var q = fts.ToFtsQuery(SearchFor);

                //
                // Prepare the results
                //
                if (string.IsNullOrWhiteSpace(q))
                {
                    Result = "Search resulted in an empty query";
                }
                else
                {
                    //
                    // The FTS query is a string argument so quote any single quotes
                    //
                    var Escaped = q.Replace("'", "''");

                    //
                    // Long CONTAINS arguments are hard to read so split them into lines
                    //
                    var SlightlyFormatted = Escaped.Replace(" AND ", "\n    AND ");

                    //
                    // Do a quick and dirty pretty print on the SQL
                    //
                    Result = $"SELECT\n    *\nFROM SomeTableFullTextIndex AS ft\nWHERE CONTAINS(ft.IndexedField, '{SlightlyFormatted}')";
                }
            }
        }
    }
}