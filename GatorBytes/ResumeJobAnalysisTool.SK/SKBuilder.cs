using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Plugins.Core;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.Plugins;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.SK.Plugins;
using ResumeJobAnalysisTool.SK.RAG;
using ResumeJobAnalysisTool.SK.SKQuickTesting;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

namespace ResumeJobAnalysisTool.SK
{
    public class SKBuilder
    {
        public async Task<SemanticKernelBuilderResult> BuildSemanticKernel(ConfigurationValues configValues, SKQuickTestOptions sKQuickTestOptions = null)
        {
            var modelId = "openai/gpt-oss-20b"; // configValues.OpenAISettings.OpenAI_Model;
            var apiKey = "openai/gpt-oss-20b"; // configValues.OpenAISettings.OpenAI_ApiKey;
            var apiUrl = "http://127.0.0.1:1234"; // configValues.OpenAISettings.OpenAI_ApiUrl;

            //configValues.OpenAISettings.OpenAI_Model = "google/gemma-3-12b";
            //configValues.OpenAISettings.OpenAI_ApiKey = "Z3T84R3-MWF4GHN-GPMDV46-20TP1V0";
            //configValues.OpenAISettings.OpenAI_ApiUrl = "http://localhost:3001/api/v1";



            // Create a kernel with Azure OpenAI chat completion
            var skBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: apiKey,
                endpoint: new Uri(apiUrl)

            )
            .AddOpenAIChatClient(modelId)
            .AddOpenAITextToAudio(
                modelId: configValues.OpenAISettings.OpenAI_TextToAudioModel,
                apiKey: apiKey
            )
            .AddOpenAIEmbeddingGenerator(configValues.OpenAISettings.OpenAI_EmbeddingModel, apiKey);

            skBuilder.Services.AddDbContext<HRContext>(options =>
            {
                options.UseSqlServer(configValues.ConnectionStrings.ConnectionString_ResumeJobAnalysisTool,
                    sqlServerOptions => sqlServerOptions.CommandTimeout(999999));
            });

            skBuilder.Services.AddSingleton<ConfigurationValues>(configValues);

            skBuilder.Services.AddQdrantVectorStore("localhost", 6333, false, null, new QdrantVectorStoreOptions
            {
            });

            

            skBuilder.Services.AddTransient<VectorProcessor>();

            // Plugins
            var usePlugins = true;
            if (usePlugins)
            {
                skBuilder.Plugins.AddFromType<FileHelperPlugin>();
                skBuilder.Plugins.AddFromType<TimePlugin>();
                skBuilder.Plugins.AddFromType<ResumeApplicantProfilePlugin>();
            }

            // Build the kernel
            Kernel kernel = skBuilder.Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStoreFactory = new VectorStoreFactory(embeddingGenerator);
            var vectorStore = vectorStoreFactory.VectorStore;

            sKQuickTestOptions.Kernel = kernel;
            var skQuickTests = new SKQuickTests(configValues);
            await skQuickTests.RunTests(sKQuickTestOptions, chatCompletionService, configValues);

            return new SemanticKernelBuilderResult
            {
                AIServices = new AIServices
                {
                    ChatCompletionService = chatCompletionService,
                    Kernel = kernel,
                    EmbeddingGenerator = embeddingGenerator,
                    QdrantVectorStore = vectorStore
                },
            };
        }

    }
}
