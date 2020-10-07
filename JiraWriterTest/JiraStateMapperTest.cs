using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JiraWriter.Data.Jira;
using JiraWriter.ErrorHandling;

namespace JiraWriterTest
{
    [TestClass]
    public class JiraStateMapperTest
    {
        [TestMethod]
        public void ShouldOnlyMapStateChanges()
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
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10000',
                    'fromString': 'TEST_READY',
                    'to': '10001',
                    'toString': 'TEST_DISCOVERY'
                  },
                  {
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10001',
                    'fromString': 'TEST_DISCOVERY',
                    'to': '10002',
                    'toString': 'TEST_BUILDING'
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

            var target = JiraStateMapper.MapStates(new List<JToken> { jsonObject });

            Assert.AreEqual(2, target.Count);
            Assert.IsTrue(target.Exists(state => state.FromState.Equals("TEST_READY") && state.ToState.Equals("TEST_DISCOVERY") && state.TransitionDate.Equals(new DateTime(2020, 10, 6))));
            Assert.IsTrue(target.Exists(state => state.FromState.Equals("TEST_DISCOVERY") && state.ToState.Equals("TEST_BUILDING") && state.TransitionDate.Equals(new DateTime(2020, 10, 6))));
        }

        [TestMethod]
        public void ShouldMapStateChangesFromDifferentDays()
        {
            string jsonForDayOne = @"{
                'id': '287186',
                'created': '2020-10-06',
                'items': [
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
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10001',
                    'fromString': 'TEST_DISCOVERY',
                    'to': '10002',
                    'toString': 'TEST_BUILDING'
                  }
                ]
            }";

            string jsonForDayTwo = @"{
                'id': '287186',
                'created': '2020-10-07',
                'items': [
                  {
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10000',
                    'fromString': 'TEST_BUILDING',
                    'to': '10001',
                    'toString': 'TEST_DONE'
                  }
                ]
            }";

            var jsonObjectForDayOne = JObject.Parse(jsonForDayOne);
            var jsonObjectForDayTwo = JObject.Parse(jsonForDayTwo);

            var target = JiraStateMapper.MapStates(new List<JToken> { jsonObjectForDayOne, jsonObjectForDayTwo });

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(2, target.FindAll(state => state.TransitionDate.Equals(new DateTime(2020, 10, 6))).Count);
            Assert.AreEqual(1, target.FindAll(state => state.TransitionDate.Equals(new DateTime(2020, 10, 7))).Count);
        }

        [TestMethod]
        public void ShouldNotMapAnyStateChangesWithNoStateChangeHistory()
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

            var target = JiraStateMapper.MapStates(new List<JToken> { jsonObject });
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingJiraFieldException))]
        public void ShouldFailWithMissingItems()
        {
            string json = @"{
                'id': '287186',
                'created': '2020-10-06',
            }";

            var jsonObject = JObject.Parse(json);

            JiraStateMapper.MapStates(new List<JToken> { jsonObject });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingJiraFieldException))]
        public void ShouldFailWithMissingField()
        {
            string json = @"{
                'id': '287186',
                'created': '2020-10-06',
                'items': [
                  {
                    'field': 'status',
                    'fieldtype': 'jira',
                    'fieldId': 'status',
                    'from': '10000',
                    'to': '10001',
                    'toString': 'TEST_DONE'
                  }
                ]
            }";

            var jsonObject = JObject.Parse(json);

            JiraStateMapper.MapStates(new List<JToken> { jsonObject });
        }
    }
}
