using System.Net.Http;
using System.Text;
using ChatGPT_CSharp.Services.IService;
using Newtonsoft.Json;

namespace ChatGPT_CSharp.Services
{
    public class OpenAIService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var requestPayload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "user", content = prompt }
        },
                max_tokens = 60
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var maxRetries = 5;
            var retryDelay = TimeSpan.FromSeconds(2);

            for (var retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseString);
                        return result.choices[0].message.content;
                    }

                    var errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        await Task.Delay(retryDelay);
                        retryDelay = retryDelay * 2; // Exponential backoff
                    }
                    else
                    {
                        throw new HttpRequestException($"Error: {response.StatusCode} - {errorMessage}");
                    }
                }
                catch
                {
                    if (retry == maxRetries - 1)
                    {
                        throw; // Rethrow the exception if max retries exceeded
                    }
                    await Task.Delay(retryDelay);
                    retryDelay = retryDelay * 2; // Exponential backoff
                }
            }

            throw new Exception("Unable to complete the request after several retries.");
        }
    }

}
