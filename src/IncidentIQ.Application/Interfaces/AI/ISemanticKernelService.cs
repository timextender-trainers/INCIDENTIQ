using Microsoft.SemanticKernel;

namespace IncidentIQ.Application.Interfaces.AI;

public interface ISemanticKernelService
{
    Kernel GetKernel();
    Task<T> ExecuteAsync<T>(string pluginName, string functionName, KernelArguments arguments);
    Task<string> ExecutePromptAsync(string prompt, KernelArguments? arguments = null);
}