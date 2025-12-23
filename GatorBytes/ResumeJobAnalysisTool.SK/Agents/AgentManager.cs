// Copyright (c) Microsoft. All rights reserved.

//using DocumentFormat.OpenXml.Math;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Assistants;
using System.ClientModel;
using System.Net.WebSockets;
using System.Text;
using ResumeJobAnalysisTool;
using ResumeJobAnalysisTool.Shared.Models;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using ResumeJobAnalysisTool.DAL.Models;


#pragma warning disable OPENAI001
#pragma warning disable SKEXP0110
#pragma warning disable OPENAI001

namespace Agents;


/// <summary>
/// Demonstrate that two different agent types are able to participate in the same conversation.
/// In this case a <see cref="ChatCompletionAgent"/> and <see cref="OpenAIAssistantAgent"/> participate.
/// </summary>
public class AgentManager()
{
    public ChatCompletionAgent GetAgent(string name, string instructions, Kernel kernel)
    {
        return new ChatCompletionAgent()
        {
            Instructions = instructions,
            Name = name,
            Kernel = kernel,
        };
    }

    public async Task<Assistant> GetAssistant(string name, string instructions, string model, AssistantClient assistantClient)
    {

        var result = await assistantClient.CreateAssistantAsync(
            model,
            new AssistantCreationOptions
            {
                Name = name,
                Instructions = instructions,
            });

        return result;
    }


#pragma warning disable SKEXP0110
    protected sealed class ApprovalTerminationStrategy(string valueToCauseAnExit = "ApprovedAndDone") : TerminationStrategy
    {
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
            => Task.FromResult(history[history.Count - 1].Content?.Contains("ApprovedAndDone", StringComparison.OrdinalIgnoreCase) ?? false);
    }


    protected virtual async Task<ClientResult<Assistant>> CreateAssistantAsync(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await CreateAssistantAsync(model, options, cancellationToken).ConfigureAwait(false);
        return ClientResult.FromValue((Assistant)protocolResult, protocolResult.GetRawResponse());
    }

}

