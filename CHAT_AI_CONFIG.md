# Chat System Configuration

Add this to your `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ChatSettings": {
    "EnableAI": true,
    "AIProvider": "groq",
    "Groq": {
      "ApiKey": "YOUR_GROQ_API_KEY_HERE",
      "Model": "llama3-8b-8192"
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
- Llama 3 8B model
- No credit card required

## Model Options:
- `llama3-8b-8192` - Fast, good for chat (Recommended)
- `llama3-70b-8192` - More powerful, slower
- `mixtral-8x7b-32768` - Longer context window

Replace `YOUR_GROQ_API_KEY_HERE` with your actual API key from Groq Console.
