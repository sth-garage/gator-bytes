using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using ResumeJobAnalysisTool.AppLogic;
using ResumeJobAnalysisTool.AppLogic.Uploading;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Prompts;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.SK;
using ResumeJobAnalysisTool.SK.RAG;

#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001


var webBuilder = WebApplication.CreateBuilder(args);

webBuilder.Services.AddControllers();

var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var configValues = UserSecretManager.GetSecrets(configBuilder);


SKBuilder skBuilder = new SKBuilder();
var semanticKernelBuildResult = await skBuilder.BuildSemanticKernel(configValues, new SKQuickTestOptions
{
    //ShouldAddTestRAGPlugin = true,
    //ShouldTestLocalRAG = true
    //ShouldTestRAGUploadAndSearch = true
    //ShouldTestImage = true,
});

webBuilder.Services.AddDbContext<HRContext>(options =>
{
    options.UseSqlServer(configValues.ConnectionStrings.ConnectionString_ResumeJobAnalysisTool,
        sqlServerOptions => sqlServerOptions.CommandTimeout(999999));
});

VectorProcessor vectorResumeor = new VectorProcessor(semanticKernelBuildResult.AIServices.QdrantVectorStore, semanticKernelBuildResult.AIServices.EmbeddingGenerator);

webBuilder.Services.AddSingleton<QdrantVectorStore>(semanticKernelBuildResult.AIServices.QdrantVectorStore);
webBuilder.Services.AddSingleton<IChatCompletionService>(semanticKernelBuildResult.AIServices.ChatCompletionService);
webBuilder.Services.AddSingleton<Kernel>(semanticKernelBuildResult.AIServices.Kernel);
webBuilder.Services.AddSingleton<ConfigurationValues>(configValues);
webBuilder.Services.AddSingleton<IVectorProcessor>(vectorResumeor);
webBuilder.Services.AddScoped(typeof(IFileAnalysisManager), typeof(FileAnalysisManager));

webBuilder.Services.AddQdrantVectorStore("localhost", 6333, false, null, new QdrantVectorStoreOptions
{
    EmbeddingGenerator = semanticKernelBuildResult.AIServices.EmbeddingGenerator
});


webBuilder.Services.AddScoped(typeof(UploadManagerBase<ResumeFileSystemUploadEntry, Resume>), typeof(UploadResumeManager));
webBuilder.Services.AddScoped(typeof(UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting>), typeof(UploadJobPostingManager));
webBuilder.Services.AddSingleton(typeof(IEmbeddingGenerator<string, Embedding<float>>), semanticKernelBuildResult.AIServices.EmbeddingGenerator);
webBuilder.Services.AddScoped(typeof(IRAGManager), typeof(RAGManager));
webBuilder.Services.AddScoped(typeof(IMatchAnalysisManager), typeof(MatchAnalysisManager));

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var app = webBuilder.Build();


// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(20)
};

app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
