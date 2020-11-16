using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobertsFullTextSearch;

namespace RobertsFullTextSearchTests
{
    /// <summary>
    /// Tests related to how the resulting query is built using AND and OR
    /// </summary>
    [TestClass]
    public class LogicTests
    {
        private readonly IQueryCreator fts = new QueryCreator();

        [TestMethod]
        public void SimpleAnd()
        {
            var q = fts.ToFtsQuery("running and jumping");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, running) AND FORMSOF(INFLECTIONAL, jumping)", q);
        }

        [TestMethod]
        public void SimpleOr()
        {
            var q = fts.ToFtsQuery("running or jumping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }

        [TestMethod]
        public void EndingWithOr()
        {
            var q = fts.ToFtsQuery("running or jumping or");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }

        [TestMethod]
        public void EndingWithMultipleOr()
        {
            var q = fts.ToFtsQuery("running or jumping or or or");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }

        [TestMethod]
        public void StartingWithOr()
        {
            var q = fts.ToFtsQuery("or running or jumping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }

        [TestMethod]
        public void StartingWithMultipleOr()
        {
            var q = fts.ToFtsQuery("or or or running or jumping");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping))", q);
        }
 
        [TestMethod]
        public void aborcd()
        {
            IQueryCreator fts = new QueryCreator();
            var q = fts.ToFtsQuery("A B OR C D");
            Assert.AreEqual("FORMSOF(INFLECTIONAL, A) AND (FORMSOF(INFLECTIONAL, B) OR FORMSOF(INFLECTIONAL, C)) AND FORMSOF(INFLECTIONAL, D)", q);
        }
 
        [TestMethod]
        public void WeirdOr()
        {
            var q = fts.ToFtsQuery("running or -diving");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running))", q);
        }

        [TestMethod]
        public void AnotherWeirdOr()
        {
            var q = fts.ToFtsQuery("running or jumping -diving");
            Assert.AreEqual("(FORMSOF(INFLECTIONAL, running) OR FORMSOF(INFLECTIONAL, jumping)) AND NOT FORMSOF(INFLECTIONAL, diving)", q);
        }
    }
}
