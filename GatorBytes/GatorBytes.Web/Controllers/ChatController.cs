//using GatorBytes.DAL.CombinedModels;
using GatorBytes.Shared.Models;
using GatorBytes.Shared.Prompts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
//using GatorBytes.SK.Agents;
using System.Net.WebSockets;
using System.Text;


#pragma warning disable OPENAI001
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001

#pragma warning disable OPENAI001

namespace GatorBytes.Controllers;

#region snippet_Controller_Connect
public class ChatController : ControllerBase
{
    readonly ConfigurationValues _configValues;
    readonly IChatCompletionService _chatCompletionService;
    readonly Kernel _kernel;
    //readonly UploadManagerBase<ResumeFileSystemUploadEntry, Resume> _uploadResumeManager;
    //readonly UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting> _uploadJobPostingManager;
    //readonly PromptManager _promptManager;
    //readonly HRContext _context;
    //readonly IMatchAnalysisManager _matchAnalysisManager;
    //readonly IVectorProcessor _vectorProcessor;


    public ChatController(ConfigurationValues configValues,
        Kernel kernel,
        IChatCompletionService chatCompletionService//,
        //UploadManagerBase<ResumeFileSystemUploadEntry, Resume> uploadResumeManager,
        //UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting> uploadJobPostingManager,
        //HRContext context,
        //IVectorProcessor vectorProcessor,
        //IMatchAnalysisManager matchAnalysisManager
        )
    {
        _configValues = configValues;
        _chatCompletionService = chatCompletionService;
        _kernel = kernel;
        //_uploadResumeManager = uploadResumeManager;
        //_uploadJobPostingManager = uploadJobPostingManager;
        //_uploadResumeManager = uploadResumeManager;
        //_context = context;
        //_promptManager = new PromptManager(_context);
        //_vectorProcessor = vectorProcessor;
        //_matchAnalysisManager = matchAnalysisManager;
    }

    [Route("/ws")]
    public async Task Get()
    {
        //await _uploadResumeManager.ProcessEntries();
        //await _context.SaveChangesAsync();
        //await _uploadJobPostingManager.ProcessEntries();
        //await _context.SaveChangesAsync();
        //await _matchAnalysisManager.MakeMatches();

        var stop = 1;


        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket, null, _chatCompletionService, _kernel, _configValues);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }


    }
    #endregion

    private static async Task Echo(WebSocket inputWebSocket,
        WebSocket outputWebSocket,
        IChatCompletionService chatCompletionService,
        Kernel kernel,
        ConfigurationValues configValues)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult receiveResult = null;

        try
        {
            receiveResult = await inputWebSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch (Exception ex)
        {

        }

        // Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory chatHistory = new ChatHistory();
        //chatHistory.AddSystemMessage(promptManager.GetSystemPrompt());
        //chatHistory.AddDeveloperMessage(Prompts.SaveAsRichHTMLThenPDF);
        chatHistory.AddSystemMessage(Prompts.TempSystemPrompt);
        chatHistory.AddDeveloperMessage(Prompts.ResultAsRichHTMLDivRoot);

        var isAgentChat = false;
        var hasAgentQuestion = false;


        var agentTest = false;

        while (!receiveResult.CloseStatus.HasValue)
        {
            var bytes = new ArraySegment<byte>(buffer, 0, receiveResult.Count);
            var userMessage = Encoding.UTF8.GetString(bytes);

            if (isAgentChat && !hasAgentQuestion)
            {
                hasAgentQuestion = true;
            }


            chatHistory.AddUserMessage(userMessage);


            ChatMessageContent content = new ChatMessageContent();

            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            var resultString = result.AsJson();
            var resultMsgBytes = Encoding.UTF8.GetBytes(resultString);


            await inputWebSocket.SendAsync(
                resultMsgBytes,
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            try
            {
                receiveResult = await inputWebSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception ex)
            {

            }
        }

        await inputWebSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
