using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Rag;

public interface ILlmClient
{
    Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default);
}

public class LlmOptions
{
    /// <summary>"offline" (default, no network) or "http" (OpenAI-compatible chat completions).</summary>
    public string Mode { get; set; } = "offline";
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "";
}

/// <summary>
/// No-network client used for local runs and for the eval. It is NOT a language model:
/// it returns the context block found in the user prompt, between "Context:" and "Question:".
/// </summary>
public class OfflineLlmClient : ILlmClient
{
    public Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
    {
        var start = userPrompt.IndexOf("Context:", StringComparison.Ordinal);
        var end = userPrompt.LastIndexOf("Question:", StringComparison.Ordinal);
        var text = start >= 0 && end > start
            ? userPrompt[(start + "Context:".Length)..end].Trim()
            : userPrompt;
        return Task.FromResult(text);
    }
}

/// <summary>Client for any OpenAI-compatible /chat/completions endpoint (company proxy, etc.).</summary>
public class OpenAiCompatibleLlmClient(HttpClient http, Microsoft.Extensions.Options.IOptions<LlmOptions> options) : ILlmClient
{
    private readonly LlmOptions _options = options.Value;

    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl.TrimEnd('/') + "/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            model = _options.Model,
            temperature = 0,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt },
            },
        });

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }
}

/// <summary>Client for Amazon Bedrock (Converse API). Authenticates with the default AWS chain (the instance role on the interview VM).</summary>
public class BedrockLlmClient(Amazon.BedrockRuntime.IAmazonBedrockRuntime bedrock, Microsoft.Extensions.Options.IOptions<LlmOptions> options) : ILlmClient
{
    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default)
    {
        var response = await bedrock.ConverseAsync(new Amazon.BedrockRuntime.Model.ConverseRequest
        {
            ModelId = options.Value.Model,
            System = [new Amazon.BedrockRuntime.Model.SystemContentBlock { Text = systemPrompt }],
            Messages =
            [
                new Amazon.BedrockRuntime.Model.Message
                {
                    Role = Amazon.BedrockRuntime.ConversationRole.User,
                    Content = [new Amazon.BedrockRuntime.Model.ContentBlock { Text = userPrompt }],
                },
            ],
            InferenceConfig = new Amazon.BedrockRuntime.Model.InferenceConfiguration { MaxTokens = 1024 },
        }, ct);
        return string.Concat(response.Output.Message.Content.Select(c => c.Text));
    }
}
