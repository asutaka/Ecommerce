# Chat System Configuration

Add this to your `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ChatSettings": {
    "EnableAI": true,
    "AIProvider": "groq",
    "Groq": {
      "ApiKey": "YOUR_GROQ_API_KEY_HERE",
      "Model": "llama-3.3-70b-versatile"
    },
    "SystemPrompt": "Bạn là trợ lý AI của cửa hàng thời trang Moderno.",
    "AutoReplyWhenOffline": true,
    "OfflineMessage": "Xin chào! Admin hiện không online. Tôi là AI assistant, tôi có thể giúp gì cho bạn?"
  }
}
```

## How to Get FREE Groq API Key:

1. Visit: https://console.groq.com/
2. Sign up for free account
3. Go to API Keys section
4. Create new API key
5. Copy and paste into appsettings.json

## FREE Tier Limits:
- 14,400 requests per day
- Ultra-fast responses (500+ tokens/sec)
- Llama 3 model
- No credit card required

## Model Options (Updated Dec 2025):
- `llama-3.3-70b-versatile` - Newest, fast, very capable (Recommended)
- `llama-3.1-8b-instant` - Very fast, good for simple chats
- `mixtral-8x7b-32768` - Good reasoning capabilities
- `gemma2-9b-it` - Google's Gemma model

## Connection Issues?
If you see errors, check:
1. Your API Key is correct
2. The model name hasn't been deprecated (Groq updates models frequently)
3. Check https://console.groq.com/docs/models for latest model names

Replace `YOUR_GROQ_API_KEY_HERE` with your actual API key from Groq Console.
