using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;

namespace Movies.API.Authorization
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly ILogger<ApiKeyProvider> _logger;
        private readonly IReadOnlyCollection<IApiKey> _apiKeys;

        public ApiKeyProvider(ILogger<ApiKeyProvider> logger, List<IApiKey> apiKeys)
        {
            _logger = logger;
            _apiKeys = apiKeys;
        }

        public Task<IApiKey> ProvideAsync(string key)
        {
            try
            {
                var apiKey = _apiKeys.FirstOrDefault(x => x.Key == key);

                return Task.FromResult(apiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
