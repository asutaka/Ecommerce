using System.Text;
using System.Text.Json;
using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services.AI;

/// <summary>
/// Groq AI Provider - FREE tier: 14,400 requests/day
/// Ultra-fast LLM inference using Llama 3
/// </summary>
public class GroqProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger<GroqProvider> _logger;

    public GroqProvider(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GroqProvider> logger)
    {
        _httpClient = httpClientFactory.CreateClient("GroqAI");
        _apiKey = configuration["ChatSettings:Groq:ApiKey"] ?? "";
        _model = configuration["ChatSettings:Groq:Model"] ?? "llama-3.3-70b-versatile";
        _logger = logger;

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Groq API key not configured. AI features will not work.");
        }
    }

    public async Task<string> GenerateResponseAsync(string userMessage, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return "Xin lỗi, hệ thống AI chưa được cấu hình. Vui lòng liên hệ admin.";
        }

        try
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = @"Bạn là trợ lý AI của cửa hàng thời trang Moderno. 
Nhiệm vụ của bạn:
- Trả lời câu hỏi về sản phẩm, giá cả, chính sách đổi trả
- Hướng dẫn khách hàng đặt hàng, thanh toán
- Giải đáp thắc mắc về vận chuyển
- Luôn lịch sự, chuyên nghiệp, nhiệt tình
- Trả lời BẰNG TIẾNG VIỆT
- KHÔNG BAO GIỜ bịa đặt thông tin nếu không biết.

Nếu câu hỏi nằm ngoài khả năng, hãy lịch sự nói rằng bạn sẽ chuyển cho nhân viên hỗ trợ."
                }
            };

            // Add conversation history (last 10 messages for context)
            foreach (var msg in history.TakeLast(10))
            {
                messages.Add(new
                {
                    role = msg.SenderType == "customer" ? "user" : "assistant",
                    content = msg.Message
                });
            }

            // Add current user message
            messages.Add(new
            {
                role = "user",
                content = userMessage
            });

            var requestBody = new
            {
                model = _model,
                messages = messages,
                temperature = 0.7,
                max_tokens = 500,
                top_p = 1,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return "Xin lỗi, hệ thống AI tạm thời gặp sự cố. Vui lòng thử lại sau hoặc đợi nhân viên hỗ trợ.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<GroqResponse>(responseJson, options);

            if (result?.Choices == null || result.Choices.Count == 0)
            {
                _logger.LogWarning("Groq API returned no choices");
                return "Xin lỗi, tôi không thể tạo phản hồi lúc này. Vui lòng thử lại.";
            }

            var aiMessage = result.Choices[0].Message.Content;
            _logger.LogInformation("Groq AI response generated successfully");

            return aiMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Groq API");
            return $"[DEBUG] Lỗi ngoại lệ: {ex.Message}";
        }
    }

    // Response models
    private class GroqResponse
    {
        public List<Choice> Choices { get; set; } = new();
        public Usage? Usage { get; set; }
    }

    private class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; } = new();
        public string? FinishReason { get; set; }
    }

    private class Message
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }

    private class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
