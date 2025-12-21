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


    public ChatController(ConfigurationValues configValues,
        Kernel kernel,
        IChatCompletionService chatCompletionService
        )
    {
        _configValues = configValues;
        _chatCompletionService = chatCompletionService;
        _kernel = kernel;

    }

    [Route("/ws")]
    public async Task Get()
    {
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


            // Enable planning
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            ChatHistory chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage(Prompts.TempSystemPrompt);
            chatHistory.AddDeveloperMessage(Prompts.ResultAsRichHTMLDivRoot);
            

            while (!receiveResult.CloseStatus.HasValue)
            {
                var bytes = new ArraySegment<byte>(buffer, 0, receiveResult.Count);
                var userMessage = Encoding.UTF8.GetString(bytes);

                chatHistory.AddUserMessage(userMessage);


                ChatMessageContent content = new ChatMessageContent();

                var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: kernel);

                result.Content = result.Content.Replace("```html", "").Replace("```", "");

                chatHistory.AddAssistantMessage(result.Content);

                var resultString = result.AsJson();
                var resultMsgBytes = Encoding.UTF8.GetBytes(resultString);


                await inputWebSocket.SendAsync(
                    resultMsgBytes,
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await inputWebSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await inputWebSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

        }
        catch (Exception ex)
        {

        }
    }
}
