using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JiraWriter.Data.Jira;

namespace JiraWriterTest.Data.Jira
{
    [TestClass]
    public class JiraBlockMapperTest
    {
        [TestMethod]
        public void ShouldOnlyMapJiraFlagsAsBlocks()
        {
            string json = @"{
                'id': '287186',
                'created': '2020-10-06',
                'items': [
                  {
                    'field': 'description',
                    'fieldtype': 'jira',
                    'fieldId': 'description',
                    'from': 'TEST_FROM',
                    'fromString': 'TEST_FROM_STRING',
                    'to': 'TEST_TO',
                    'toString': 'TEST_TOSTRING'
                  },
                  {
                      'field': 'Flagged',
                      'fieldtype': 'custom',
                      'fieldId': 'customfield_10318',
                      'from': null,
                      'fromString': null,
                      'to': '[10205]',
                      'toString': 'Impediment'
                  },
                  {
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10000',
                    'fromString': 'TEST_READY',
                    'to': '10001',
                    'toString': 'TEST_DISCOVERY'
                  },
                  {
                    'field': 'description',
                    'fieldtype': 'jira',
                    'fieldId': 'description',
                    'from': 'TEST_TO',
                    'fromString': 'TEST_TO_STRING',
                    'to': 'TEST_TO2',
                    'toString': 'TEST_TOSTRING2'
                  },        
                ]
            }";

            var jsonObject = JObject.Parse(json);

            var target = JiraBlockMapper.Map(new List<JToken> { jsonObject });

            Assert.AreEqual(1, target.Count);

            Assert.IsTrue(target[0].BlockDate.Equals(new DateTime(2020, 10, 6)));
            Assert.IsNull(target[0].UnblockDate);
        }

        [TestMethod]
        public void ShouldMapSingleBlockFromBlockAndUnblockActions()
        {
            string jsonForAddingFlag = @"{
                'id': '287186',
                'created': '2020-10-06',
                'items': [
                  {
                      'field': 'Flagged',
                      'fieldtype': 'custom',
                      'fieldId': 'customfield_10318',
                      'from': null,
                      'fromString': null,
                      'to': '[10205]',
                      'toString': 'Impediment'
                  }
                ]
            }";

            string jsonForRemovingFlag = @"{
                'id': '287186',
                'created': '2020-10-07',
                'items': [
                  {
                      'field': 'Flagged',
                      'fieldtype': 'custom',
                      'fieldId': 'customfield_10318',
                      'from': '[10205]',
                      'fromString': 'Impediment',
                      'to': null,
                      'toString': null
                  }
                ]
            }";

            var jsonObjectForDayOne = JObject.Parse(jsonForAddingFlag);
            var jsonObjectForDayTwo = JObject.Parse(jsonForRemovingFlag);

            var target = JiraBlockMapper.Map(new List<JToken> { jsonObjectForDayOne, jsonObjectForDayTwo });

            Assert.AreEqual(1, target.Count);
            Assert.IsTrue(target[0].BlockDate.Equals(new DateTime(2020, 10, 6)));
            Assert.IsTrue(target[0].UnblockDate.Equals(new DateTime(2020, 10, 7)));
        }
    }
}
