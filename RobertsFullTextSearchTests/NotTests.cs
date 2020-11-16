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
    /// These tests relate to queries that exclude certain words via the minus sign.
    /// One peculiarity of FTS queries is that a query cannot begin with a NOT clause.
    /// </summary>
    [TestClass]
    public class NotTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void Minus()
        {
            var q = fts.ToFtsQuery("running or jumping -leaping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND NOT FORMSOF(INFLECTIONAL, leaping)", q);
        }

        [TestMethod]
        public void MinusMinus()
        {
            var q = fts.ToFtsQuery("running or jumping -leaping -diver");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND NOT FORMSOF(INFLECTIONAL, leaping) AND NOT FORMSOF(INFLECTIONAL, diver)", q);
        }

        [TestMethod]
        public void FirstIsMinus()
        {
            var q = fts.ToFtsQuery("-leaping running or jumping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND NOT FORMSOF(INFLECTIONAL, leaping)", q);
        }

        [TestMethod]
        public void FirstIsMinusLastIsMinus()
        {
            var q = fts.ToFtsQuery("-leaping running -jumping");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, running) AND NOT FORMSOF(INFLECTIONAL, jumping) AND NOT FORMSOF(INFLECTIONAL, leaping)", q);
        }
        [TestMethod]
        public void FirstIsMinusThenOr()
        {
            var q = fts.ToFtsQuery("-leaping -jumping or running");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, running) AND NOT FORMSOF(INFLECTIONAL, leaping) AND NOT FORMSOF(INFLECTIONAL, jumping)", q);
        }

        [TestMethod]
        public void DoubleOrMiddle()
        {
            var q = fts.ToFtsQuery("A B OR -C OR D E");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, D)) AND FORMSOF(INFLECTIONAL, E)", q);
        }


        [TestMethod]
        public void DoubleOrStart()
        {
            var q = fts.ToFtsQuery("A OR B OR -C D E");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, A) OR FORMSOF(INFLECTIONAL, B)) AND FORMSOF(INFLECTIONAL, D) AND FORMSOF(INFLECTIONAL, E)", q);
        }



        [TestMethod]
        public void FirstIsMinusLastIsMinusSameWord()
        {
            var q = fts.ToFtsQuery("-leaping -leaping running");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, running) AND NOT FORMSOF(INFLECTIONAL, leaping) AND NOT FORMSOF(INFLECTIONAL, leaping)", q);
        }
        [TestMethod]
        public void MinusAndQuotes()
        {
            var q = fts.ToFtsQuery("-\"leap\"ing");
            Assert.AreEqual("\"leap\" AND FORMSOF(INFLECTIONAL, ing)", q);
        }

        [TestMethod]
        public void JustAMinus()
        {
            var q = fts.ToFtsQuery("-leaping");
            Assert.AreEqual("", q);
        }
    }
}
