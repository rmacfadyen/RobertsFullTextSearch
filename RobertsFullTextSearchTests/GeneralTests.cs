using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobertsFullTextSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobertsFullTextSearchTests
{
    /// <summary>
    /// Tests with no specific category
    /// </summary>
    [TestClass]
    public class GeneralTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void DuplicateWords()
        {
            var q = fts.ToFtsQuery("running jumping running");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, running) AND FORMSOF(INFLECTIONAL, jumping) AND FORMSOF(INFLECTIONAL, running)", q);
        }

        [TestMethod]
        public void SingleQuote()
        {
            var q = fts.ToFtsQuery("lea'ping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, \"lea'ping\") OR \"lea'ping*\")", q);
        }
        
        [TestMethod]
        public void Wildcard()
        {
            var q = fts.ToFtsQuery("leaping*");
            Assert.AreEqual("\"leaping*\"", q);
        }

        [TestMethod]
        public void WordWithNumber()
        {
            var q = fts.ToFtsQuery("part1701");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, part1701) OR \"part1701*\")", q);
        }
    }
}
