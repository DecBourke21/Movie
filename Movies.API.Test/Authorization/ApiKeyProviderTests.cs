using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Authentication.ApiKey;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.API.Authorization;

namespace Movies.API.Test.Authorization
{
    [TestClass]
    public class ApiKeyProviderTests
    {
        private readonly ApiKeyProvider _provider;
        private readonly Mock<ILogger<ApiKeyProvider>> _logger;
        private readonly ApiKey _apiKey;
        private readonly List<IApiKey> _apiKeys;

        public ApiKeyProviderTests()
        {
            _logger = new Mock<ILogger<ApiKeyProvider>>();
            _apiKey = new ApiKey("Service1", "Declan Bourke");
            _apiKeys = new List<IApiKey>()
            {
                new ApiKey("Service2", "Declan Bourke"),
                _apiKey
            };

            _provider = new ApiKeyProvider(_logger.Object, _apiKeys);
        }

        [TestMethod]
        public void ApiKey_WhenNull_ShouldReturnNull()
        {
            string key = null;

            _provider.ProvideAsync(key).Result.Should().BeNull();
        }

        [TestMethod]
        public void ApiKey_WhenMatch_ShouldReturnKey()
        {
            _provider.ProvideAsync(_apiKey.Key).Result.Should().BeEquivalentTo(_apiKey, "because the api key should match a key from the list");
        }

        [TestMethod]
        public void ApiKey_WhenNoMatch_ShouldReturnNull()
        {
            string key = "ServiceX";

            _provider.ProvideAsync(key).Result.Should().BeNull();
        }
    }
}
