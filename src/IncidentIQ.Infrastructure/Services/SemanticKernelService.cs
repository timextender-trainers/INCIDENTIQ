using IncidentIQ.Application.Interfaces.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace IncidentIQ.Infrastructure.Services;

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly ILogger<SemanticKernelService> _logger;

    public SemanticKernelService(IConfiguration configuration, ILogger<SemanticKernelService> logger)
    {
        _logger = logger;
        
        var builder = Kernel.CreateBuilder();
        
        // Configure OpenAI
        var openAiApiKey = configuration["OpenAI:ApiKey"];
        if (!string.IsNullOrEmpty(openAiApiKey))
        {
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4o", 
                apiKey: openAiApiKey);
        }

        // Configure Azure OpenAI (fallback or primary)
        var azureOpenAiApiKey = configuration["AzureOpenAI:ApiKey"];
        var azureOpenAiEndpoint = configuration["AzureOpenAI:Endpoint"];
        if (!string.IsNullOrEmpty(azureOpenAiApiKey) && !string.IsNullOrEmpty(azureOpenAiEndpoint))
        {
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: "gpt-4o",
                endpoint: azureOpenAiEndpoint,
                apiKey: azureOpenAiApiKey);
        }

        // Perplexity API removed - focusing on OpenAI only

        _kernel = builder.Build();
    }

    public Kernel GetKernel() => _kernel;

    public async Task<T> ExecuteAsync<T>(string pluginName, string functionName, KernelArguments arguments)
    {
        try
        {
            var result = await _kernel.InvokeAsync<T>(pluginName, functionName, arguments);
            return result ?? throw new InvalidOperationException($"Semantic Kernel function {pluginName}.{functionName} returned null");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Semantic Kernel function {PluginName}.{FunctionName}", pluginName, functionName);
            throw;
        }
    }

    public async Task<string> ExecutePromptAsync(string prompt, KernelArguments? arguments = null)
    {
        try
        {
            arguments ??= new KernelArguments();
            var result = await _kernel.InvokePromptAsync(prompt, arguments);
            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing prompt: {Prompt}", prompt.Substring(0, Math.Min(prompt.Length, 100)));
            throw;
        }
    }
}