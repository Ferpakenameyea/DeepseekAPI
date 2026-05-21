using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DeepseekAPI.Constants;
using DeepseekAPI.Messages;
using DeepseekAPI.Schema;
using DeepseekAPI.Tools;
using Newtonsoft.Json;

namespace DeepseekAPI
{
    public sealed class DeepseekAPI
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly HttpClient _httpClient;
        private readonly DeepseekAPIConfiguration _configuration;

        public DeepseekAPI(
            string apiKey,
            string model,
            string endpoint = API.Endpoint)
        : this(
            apiKey,
            new DeepseekAPIConfiguration(model),
            endpoint
        ) {}

        public DeepseekAPI(
            string apiKey,
            DeepseekAPIConfiguration config,
            string endpoint = API.Endpoint)
        {
            _configuration = config;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(endpoint)
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                scheme: "Bearer",
                parameter: apiKey
            );
        }

        public async Task<ChatCompletion> CompleteChatAsync(
            IEnumerable<Message> messages, 
            IEnumerable<ToolDefinition>? tools = null, 
            ToolChoice? toolChoice = null)
        {
            var requestBody = BuildRequest(messages, tools, toolChoice, stream: false);
            var requestJson = JsonConvert.SerializeObject(requestBody, _jsonSerializerSettings);

            using var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(API.ChatRoute, httpContent);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var completion = JsonConvert.DeserializeObject<ChatCompletion>(responseJson)
                ?? throw new InvalidOperationException("Failed to deserialize chat completion response.");

            return completion;
        }

        public async IAsyncEnumerable<StreamChatCompletion> CompleteStreamedChatAsync(
            IEnumerable<Message> messages, 
            IEnumerable<ToolDefinition>? tools = null, 
            ToolChoice? toolChoice = null)
        {
            const string ssePrefix = "data: ";
            const string done = "[DONE]";

            var requestBody = BuildRequest(messages, tools, toolChoice, stream: true);
            var requestJson = JsonConvert.SerializeObject(requestBody, _jsonSerializerSettings);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, API.ChatRoute)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            using var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead
            );
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                if (!line.StartsWith(ssePrefix))
                {
                    continue;
                }

                var data = line[ssePrefix.Length..];
                if (data == done)
                {
                    break;
                }

                var chunk = JsonConvert.DeserializeObject<StreamChatCompletion>(data);
                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }

        private ChatCompletionRequest BuildRequest(IEnumerable<Message> messages, IEnumerable<ToolDefinition>? tools, ToolChoice? toolChoice, bool stream)
        {
            var request = new ChatCompletionRequest
            {
                Model            = _configuration.Model,
                Temperature      = _configuration.Temperature,
                TopP             = _configuration.TopP,
                FrequencyPenalty = _configuration.FrequencyPenalty,
                PresencePenalty  = _configuration.PresencePenalty,
                MaxTokens        = _configuration.MaxTokens,
                Stop             = _configuration.Stop,
                Logprobs         = _configuration.Logprobs,
                TopLogprobs      = _configuration.TopLogprobs,
                ReasoningEffort  = _configuration.ReasoningEffort?.AsParameterString(),
                UserId           = _configuration.UserId,

                Messages         = messages,
                Stream           = stream,
                Tools            = tools,
                ToolChoiceOption = toolChoice,
            };

            if (_configuration.Thinking.HasValue)
            {
                request.Thinking = new ChatCompletionRequest.ThinkingConfig(
                    _configuration.Thinking.Value ? ThinkingMode.Enabled : ThinkingMode.Disabled
                );
            }

            if (_configuration.ResponseFormat.HasValue)
            {
                request.ResponseFormat = new ChatCompletionRequest.ResponseFormatConfig(
                    _configuration.ResponseFormat.Value
                );
            }

            return request;
        }
    }
}
