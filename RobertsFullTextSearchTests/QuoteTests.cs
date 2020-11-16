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
    /// Tests related to quoted search terms
    /// </summary>
    [TestClass]
    public class QuoteTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void Quoted()
        {
            var q = fts.ToFtsQuery("\"running or jumping\"");
            Assert.AreEqual("\"running or jumping\"", q);
        }

        [TestMethod]
        public void QuotedExtraWhitespace()
        {
            var q = fts.ToFtsQuery("\"  running   or   jumping  \"");
            Assert.AreEqual("\"running   or   jumping\"", q);
        }

        [TestMethod]
        public void SomeQuotedSomeNot()
        {
            var q = fts.ToFtsQuery("leaping \"running or jumping\" diving");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, leaping) AND \"running or jumping\" AND FORMSOF(INFLECTIONAL, diving)", q);
        }

        [TestMethod]
        public void QuoteAfterQuote()
        {
            var q = fts.ToFtsQuery("\"leaping diver\"\"running or jumping\"");
            Assert.AreEqual("\"leaping diver\" AND \"running or jumping\"", q);
        }

        [TestMethod]
        public void UnterminatedQuote()
        {
            var q = fts.ToFtsQuery("\"running or jumping");
            Assert.AreEqual("\"running or jumping\"", q);
        }

        [TestMethod]
        public void EndingInQuote()
        {
            var q = fts.ToFtsQuery("running or jumping\"");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }

        [TestMethod]
        public void Plus()
        {
            var q = fts.ToFtsQuery("running or jumping +leaping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND \"leaping\"", q);
        }

        [TestMethod]
        public void PlusPlus()
        {
            var q = fts.ToFtsQuery("running or jumping +leaping +diver");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND \"leaping\" AND \"diver\"", q);
        }
    }
}
