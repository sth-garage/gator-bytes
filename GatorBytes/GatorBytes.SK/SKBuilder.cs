using GatorBytes.DAL.Context;
using GatorBytes.Shared.Models;
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

            //configValues.OpenAISettings.OpenAI_Model = "google/gemma-3-12b";
            //configValues.OpenAISettings.OpenAI_ApiKey = "Z3T84R3-MWF4GHN-GPMDV46-20TP1V0";
            //configValues.OpenAISettings.OpenAI_ApiUrl = "http://localhost:3001/api/v1";



            // Create a kernel with Azure OpenAI chat completion
            var skBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: modelId,
                endpoint: new Uri(apiUrl)

            );

            //skBuilder.Services.AddDbContext<GatorBytesDBContext>(options =>
            //{
            //    options.UseSqlServer(configValues.ConnectionStrings.ConnectionString_GatorBytes,
            //        sqlServerOptions => sqlServerOptions.CommandTimeout(999999));
            //});

            //skBuilder.Services.AddSingleton<ConfigurationValues>(configValues);

            // Plugins


            // Build the kernel
            Kernel kernel = skBuilder.Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            //var modelId = "google/gemma-3-1b";
            //var apiUrl = @"http://127.0.0.1:1234/v1/";

            //// Create a kernel with Azure OpenAI chat completion
            //var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(
            //  modelId: modelId,
            //  apiKey: modelId,
            //  endpoint: new Uri(apiUrl));

            //Kernel kernel = builder.Build();
            //var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            //// Enable planning
            //OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            //{
            //    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            //};



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
