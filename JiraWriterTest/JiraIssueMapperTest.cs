using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Newtonsoft.Json.Linq;
using JiraWriter.Data.Jira;

namespace JiraWriterTest
{
    [TestClass]
    public class JiraIssueMapperTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailWithInvalidJson()
        {
            string json = "{}";

            var jsonObject = JObject.Parse(json);

            JiraIssueMapper.MapJiraIssue(jsonObject);
        }

        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void ShouldFailWhenFieldIsMissing()
        {
            string json = @"{
                'key' : 'ASTRA-1234',
                'changelog' : {
                    'maxResults' : 0,
                    'total': 0
                },
                'fields' : {
                    'issuetype' : {
                        'key' : 'TEST_TYPE_KEY',
                        'name' : 'TEST TYPE'
                    },
                    'status' : {
                        'key' : 'TEST_STATUS_KEY',
                        'name' : 'TEST STATUS'
                    },
                    'labels' : ['label_1', 'label_2']
                }
            }";

            var jsonObject = JObject.Parse(json);

            JiraIssueMapper.MapJiraIssue(jsonObject);
        }

        [TestMethod]
        public void ShouldMapFields()
        {
            string json = @"{
                'key' : 'ASTRA-1234',
                'changelog' : {
                    'maxResults' : 0,
                    'total': 0
                },
                'fields' : {
                    'summary' : 'TEST_SUMMARY',
                    'issuetype' : {
                        'key' : 'TEST_TYPE_KEY',
                        'name' : 'TEST_TYPE'
                    },
                    'status' : {
                        'key' : 'TEST_STATUS_KEY',
                        'name' : 'TEST_STATUS'
                    },
                    'labels' : ['label_1', 'label_2']
                }
            }";

            var jsonObject = JObject.Parse(json);

            var targetIssue = JiraIssueMapper.MapJiraIssue(jsonObject);

            Assert.AreEqual("ASTRA-1234", targetIssue.Key);
            Assert.AreEqual("TEST_SUMMARY", targetIssue.Description);
            Assert.AreEqual("TEST_TYPE", targetIssue.Type);
            Assert.AreEqual("TEST_STATUS", targetIssue.Status);
            Assert.IsTrue(targetIssue.Labels.Contains("label_1"));
            Assert.IsTrue(targetIssue.Labels.Contains("label_2"));
        }

        [TestMethod]
        public void ShouldHaveMoreChangelogHistory()
        {
            string json = @"{
                'key' : 'ASTRA-1234',
                'changelog' : {
                    'maxResults' : 1,
                    'total': 10
                },
                'fields' : {
                    'summary' : 'TEST_SUMMARY',
                    'issuetype' : {
                        'key' : 'TEST_TYPE_KEY',
                        'name' : 'TEST_TYPE'
                    },
                    'status' : {
                        'key' : 'TEST_STATUS_KEY',
                        'name' : 'TEST_STATUS'
                    },
                    'labels' : ['label_1', 'label_2']
                }
            }";

            var jsonObject = JObject.Parse(json);

            var targetIssue = JiraIssueMapper.MapJiraIssue(jsonObject);

            Assert.IsTrue(targetIssue.HasMoreChangeHistory);
        }
    }
}
