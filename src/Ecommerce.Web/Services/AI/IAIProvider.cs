using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services.AI;

public interface IAIProvider
{
    Task<string> GenerateResponseAsync(string userMessage, List<ChatMessage> history);
}
