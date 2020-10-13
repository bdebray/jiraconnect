using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Extension;

namespace JiraWriterTest.Extension
{
    [TestClass]
    public class DictionaryCsvExtensionsTest
    {
        [TestMethod]
        public void DictionaryCsvExtensionShouldAddElementsToDynamic()
        {
            var testDictionary = new Dictionary<string, object>
            {
                { "StringField", "Value1" },
                { "IntField", 2 },
                { "BoolField", true }
            };

            var target = testDictionary.ToDynamic();

            Assert.AreEqual("Value1", target.StringField);
            Assert.AreEqual(2, target.IntField);
            Assert.IsTrue(target.BoolField);
        }
    }
}
