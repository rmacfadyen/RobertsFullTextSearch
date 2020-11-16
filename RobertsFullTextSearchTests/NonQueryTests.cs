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
    /// These tests result in an empty query
    /// </summary>
    [TestClass]
    public class NonQueryTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void EmptyString()
        {
            var q = fts.ToFtsQuery("");
            Assert.AreEqual("", q);
        }

        [TestMethod]
        public void JustAnOr()
        {
            var q = fts.ToFtsQuery("OR");
            Assert.AreEqual("", q);
        }

        [TestMethod]
        public void JustAnAnd()
        {
            var q = fts.ToFtsQuery("AND");
            Assert.AreEqual("", q);
        }

        [TestMethod]
        public void BunchOfOrs()
        {
            var q = fts.ToFtsQuery("OR OR OR OR");
            Assert.AreEqual("", q);
        }

        [TestMethod]
        public void BunchOfAnds()
        {
            var q = fts.ToFtsQuery("and and and and");
            Assert.AreEqual("", q);
        }
    }
}
