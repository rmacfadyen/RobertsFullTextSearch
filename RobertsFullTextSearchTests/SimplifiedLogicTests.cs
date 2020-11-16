using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobertsFullTextSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobertsFullTextSearchTests
{
    [TestClass]
    public class SimplifiedLogicTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void aborcdSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A B OR C D");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C)) AND FORMSOF(INFLECTIONAL, D)", q);
        }

        [TestMethod]
        public void aborcSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A B OR C");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C))", q);
        }
        [TestMethod]
        public void borcdSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("B OR C D");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C)) AND FORMSOF(INFLECTIONAL, D)", q);
        }
        [TestMethod]
        public void borcSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("B OR C");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C))", q);
        }


        [TestMethod]
        public void borSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("B OR");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, B)", q);
        }

        [TestMethod]
        public void orbSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("OR B");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, B)", q);
        }

        [TestMethod]
        public void aborcordSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A B OR C OR D");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C) OR FORMSOF(INFLECTIONAL, D))", q);
        }

        [TestMethod]
        public void aborcordeSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A B OR C OR D E");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C) OR FORMSOF(INFLECTIONAL, D)) AND FORMSOF(INFLECTIONAL, E)", q);
        }
        [TestMethod]
        public void aornotbSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A OR -B");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, A))", q);
        }

        [TestMethod]
        public void aorplusbSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A OR +B");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, A) OR \"B\")", q);
        }

        [TestMethod]
        public void aorquotebSimplifiedOr()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A OR \"B\"");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, A) OR \"B\")", q);
        }
    }
}
