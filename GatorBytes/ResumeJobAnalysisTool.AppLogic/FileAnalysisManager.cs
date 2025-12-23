using Microsoft.Identity.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using OpenAI.Chat;
//using ResumeJobAnalysisTool.DAL.CombinedModels;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Prompts;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ResumeJobAnalysisTool.Shared.Enums;
using BinaryContent = Microsoft.SemanticKernel.BinaryContent;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010

#pragma warning disable SKEXP0110
#pragma warning disable OPENAI001

namespace ResumeJobAnalysisTool.AppLogic
{
    public class FileAnalysisManager : IFileAnalysisManager
    {
        private PromptManager _promptManager;
        private HRContext _context;
        private ConfigurationValues _configValues;
        private IChatCompletionService _chatCompletionService;

        public FileAnalysisManager(ConfigurationValues configValue, HRContext hrContext, IChatCompletionService chatCompletionService)
        {
            _configValues = configValue;
            _context = hrContext;
            _promptManager = new PromptManager(_context);
            _chatCompletionService = chatCompletionService;
        }



        public async Task<ResumeResult> GetResumeAnalysis(DocumentUpload documentUpload)
        {
            ChatMessageContent reply = null;

            var fileBytes = Convert.FromBase64String(documentUpload.Base64Data);
            ResumeResult resumeResult = new ResumeResult();

            HttpClient customHttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60) // Set timeout to 5 minutes (or any desired duration)
            };

            OpenAIChatCompletionService service 
                = new OpenAIChatCompletionService("mistralai/ministral-3-14b-reasoning", new Uri("http://127.0.0.1:1234/v1/"), apiKey: "mistralai/ministral-3-14b-reasoning", httpClient: customHttpClient);


            //OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
            //    _configValues.OpenAISettings.OpenAI_ApiKey, httpClient: customHttpClient);

            try
            {
                var chatHistory = new ChatHistory();
                var prompt = await _promptManager.GetResumeGeneralAnalysisPrompt();
                Console.WriteLine(documentUpload.FileName);

                chatHistory.AddUserMessage([
                        new TextContent(prompt),
                        new ImageContent(fileBytes, "application/pdf"),
                    ]);


                Console.WriteLine("!!!Beginning chat");
                reply = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("ERROR: " + ex.StackTrace);

            }


            Console.WriteLine("!!!!!Chat received");

            ResumeAnalysis generalAnalysis = new ResumeAnalysis();

            if (reply != null)
            {
                try
                {
                    if (reply.Content != null)
                    {
                        Console.WriteLine("Content: " + reply.Content);
                        generalAnalysis = JsonSerializer.Deserialize<ResumeAnalysis>(reply.Content);
                    }
                    else
                    {
                        if (reply == null)
                        {
                            Console.WriteLine("Reply was null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Deserialization exception");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"{ex.InnerException.Message}");
                        Console.WriteLine($"{ex.InnerException.StackTrace}");
                    }
                }
            }
            else
            {
                Console.WriteLine("REPLY IS NULL");
            }

            resumeResult.Text = generalAnalysis.text;
            resumeResult.Markdown = generalAnalysis.markdown;
            resumeResult.Name = generalAnalysis.candidateName;
            resumeResult.Summary = generalAnalysis.candidateSummary;
            resumeResult.Personality = generalAnalysis.candidatePersonality;

            var chatHistory3 = new ChatHistory();
            var skillString = String.Join(", ", _context.Skills.OrderByDescending(x => x.SkillName).Select(x => x.SkillName));

            var prompt3 = await _promptManager.GetResumeSkillsAnalysisPrompt();
            Console.WriteLine(documentUpload.FileName);

            chatHistory3.AddUserMessage([
                    new TextContent("Existing skills: " + skillString),
                    new TextContent(prompt3),
                        new TextContent(generalAnalysis.text),
                    ]);

            Console.WriteLine("!!!Beginning chat");

            var skillResponse = await service.GetChatMessageContentAsync(chatHistory3);

            resumeResult.Skills = skillResponse?.Content;

            var chatHistory4 = new ChatHistory();
            var prompt4 = await _promptManager.GetResumeToHTMLPrompt();
            Console.WriteLine(documentUpload.FileName);

            chatHistory4.AddUserMessage([
                    new TextContent(prompt4),
                        new TextContent("Markdown: " + resumeResult.Markdown),
                        new TextContent("Text: " + resumeResult.Text),
                        new TextContent("Skills: " + resumeResult.Skills),

                    ]);


            Console.WriteLine("!!!Beginning chat");

            var htmlResponse = await service.GetChatMessageContentAsync(chatHistory4);

            resumeResult.HTML = htmlResponse?.Content;


            resumeSkillsJSON test4 = new resumeSkillsJSON();

            try
            {
                test4 = JsonSerializer.Deserialize<resumeSkillsJSON>(resumeResult.Skills);
            }
            catch (Exception ex)
            {
                var stop = 1;
            }

            resumeResult.SkillsList = test4;



            var resultSer = JsonSerializer.Serialize(resumeResult);
            File.WriteAllText(@"C:\temp\temptemptemp.json", resultSer);
            File.WriteAllText(@"C:\temp\temptemptemp.html", resumeResult.HTML);
            File.WriteAllText(@"C:\temp\temptemptemp.txt", resumeResult.Text);
            File.WriteAllText(@"C:\temp\temptemptemp.md", resumeResult.Markdown);
            File.WriteAllText(@"C:\temp\temptemptemp.skills.json", resumeResult.Skills);

            



            return resumeResult;
        }


        public async Task<JobPostingResult> GetJobPostingAnalysis(DocumentUpload documentUpload)
        {
            ChatMessageContent reply = null;
            var fileName = documentUpload.FileName;
            var fileBytes = Convert.FromBase64String(documentUpload.Base64Data);

            JobPostingResult jobPostingResult = new JobPostingResult();

            HttpClient customHttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60) // Set timeout to 5 minutes (or any desired duration)
            };

            OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
                _configValues.OpenAISettings.OpenAI_ApiKey, httpClient: customHttpClient);


            try
            {
                var chatHistory = new ChatHistory();
                var prompt = await _promptManager.GetJobGeneralAnalysisPrompt();
                Console.WriteLine(fileName);

                chatHistory.AddUserMessage([
                        //new TextContent("Existing skills: " + String.Join(",", skills)),
                        new TextContent(prompt),
                        new Microsoft.SemanticKernel.BinaryContent(fileBytes, "application/pdf"),
                    ]);


                Console.WriteLine("!!!Beginning chat");

                reply = await service.GetChatMessageContentAsync(chatHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("ERROR: " + ex.StackTrace);

            }


            Console.WriteLine("!!!!!Chat received");

            JobPostingAnalysis test2 = new JobPostingAnalysis();

            if (reply != null)
            {
                try
                {
                    if (reply.Content != null)
                    {
                        Console.WriteLine("Content: " + reply.Content);
                        test2 = JsonSerializer.Deserialize<JobPostingAnalysis>(reply.Content);
                    }
                    else
                    {
                        if (reply == null)
                        {
                            Console.WriteLine("Reply was null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Deserialization exception");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"{ex.InnerException.Message}");
                        Console.WriteLine($"{ex.InnerException.StackTrace}");
                    }
                }
            }
            else
            {
                Console.WriteLine("REPLY IS NULL");

            }

            jobPostingResult.Text = test2.text;
            jobPostingResult.Markdown = test2.markdown;
            jobPostingResult.Name = test2.companyName + " - " + test2.position;
            jobPostingResult.Summary = test2.jobSummary;
            jobPostingResult.Culture = test2.culture;
            jobPostingResult.Position = test2.position;
            jobPostingResult.Benefits = test2.benefits;
            jobPostingResult.PostingDate = test2.postingDate;
            jobPostingResult.CompanyName = test2.companyName;


            var chatHistory3 = new ChatHistory();
            var skillString = String.Join(", ", _context.Skills.OrderByDescending(x => x.SkillName).Select(x => x.SkillName));

            var prompt3 = await _promptManager.GetJobSkillsAnalysisPrompt();
            Console.WriteLine(fileName);

            chatHistory3.AddUserMessage([
                    new TextContent("Existing skills: " + skillString),
                    new TextContent(prompt3),
                        new TextContent(test2.text),
                    ]);


            Console.WriteLine("!!!Beginning chat");

            var reply3 = await service.GetChatMessageContentAsync(chatHistory3);

            jobPostingResult.Skills = reply3?.Content;

            var chatHistory4 = new ChatHistory();
            var prompt4 = await _promptManager.GetJobToHTMLPrompt();
            Console.WriteLine(fileName);

            chatHistory4.AddUserMessage([
                    new TextContent(prompt4),
                        new TextContent("Markdown: " + jobPostingResult.Markdown),
                        new TextContent("Text: " + jobPostingResult.Text),
                        new TextContent("Skills: " + jobPostingResult.Skills),

                    ]);


            Console.WriteLine("!!!Beginning chat");

            var reply4 = await service.GetChatMessageContentAsync(chatHistory4);

            jobPostingResult.HTML = reply4?.Content;

            List<jobSkillJSON> resumeSkillStr = new List<jobSkillJSON>();
            jobSkillsJSON test4 = new jobSkillsJSON();

            try
            {
                test4 = JsonSerializer.Deserialize<jobSkillsJSON>(jobPostingResult.Skills);
            }
            catch (Exception ex)
            {
                var stop = 1;
            }

            jobPostingResult.SkillsList = test4;



            var resultSer = JsonSerializer.Serialize(jobPostingResult);
            File.WriteAllText(@"C:\temp\temptemptemp2.json", resultSer);
            File.WriteAllText(@"C:\temp\temptemptemp2.html", jobPostingResult.HTML);
            File.WriteAllText(@"C:\temp\temptemptemp2.txt", jobPostingResult.Text);
            File.WriteAllText(@"C:\temp\temptemptemp2.md", jobPostingResult.Markdown);
            File.WriteAllText(@"C:\temp\temptemptemp2.skills.json", jobPostingResult.Skills);

            return jobPostingResult;
        }

    }

    
}