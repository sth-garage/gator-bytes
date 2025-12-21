
using GatorBytes.DAL.Context;
using GatorBytes.Shared.Models;
using GatorBytes.SK.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

namespace GatorBytes.SK
{
    public class SKBuilder
    {
        public async Task<SemanticKernelBuilderResult> BuildSemanticKernel(ConfigurationValues configValues)
        {
            var modelId = configValues.LMStudioSettings.LMStudio_Model;
            var apiKey = configValues.LMStudioSettings.LMStudio_ApiKey;
            var apiUrl = configValues.LMStudioSettings.LMStudio_ApiUrl;

            // Create a kernel with Azure OpenAI chat completion
            var skBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: modelId,
                endpoint: new Uri(apiUrl)
            );

            skBuilder.Services.AddDbContext<GatorBytesDBContext>(options =>
            {
                options.UseSqlServer(configValues.ConnectionStrings.ConnectionString_GatorBytes,
                    sqlServerOptions => sqlServerOptions.CommandTimeout(600));
            });

            skBuilder.Services.AddSingleton<ConfigurationValues>(configValues);

            // Plugins
            skBuilder.Plugins.AddFromType<DocumentUploadPlugin>();

            // Build the kernel
            Kernel kernel = skBuilder.Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            return new SemanticKernelBuilderResult
            {
                AIServices = new AIServices
                {
                    ChatCompletionService = chatCompletionService,
                    Kernel = kernel
                },
            };
        }

    }
}
