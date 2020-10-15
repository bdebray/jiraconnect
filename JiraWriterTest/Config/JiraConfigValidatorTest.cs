using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraWriter.Config;
using JiraWriter.ErrorHandling;

namespace JiraWriterTest.Config
{
    [TestClass]
    public class JiraConfigValidatorTest
    {
        [TestMethod]
        public void JiraConfigShouldBeValid()
        {
            var config = new JiraConfig()
            {
                BaseUrl = "https://your-org.atlassian.net/rest/api/3",
                ApiKey = "some_api_key"
            };

            Assert.IsTrue(JiraConfigValidator.IsValid(config));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidJiraConfigException))]
        public void JiraConfigMissingApiKeyShouldNotBeValid()
        {
            var config = new JiraConfig()
            {
                BaseUrl = "https://your-org.atlassian.net/rest/api/3",
                ApiKey = string.Empty
            };

            JiraConfigValidator.IsValid(config);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidJiraConfigException))]
        public void JiraConfigMissingBaseUrlShouldNotBeValid()
        {
            var config = new JiraConfig()
            {
                BaseUrl = string.Empty,
                ApiKey = "some_api_key"

            };

            JiraConfigValidator.IsValid(config);
        }
    }
}
