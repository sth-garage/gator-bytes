using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Identity.Client;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.AppLogic
{
    public class MatchAnalysisManager : IMatchAnalysisManager
    {
        private HRContext _context;
        private ConfigurationValues _configValues;
        IEmbeddingGenerator<string, Embedding<float>> _test;
        IRAGManager _ragManager;
        PromptManager _promptManager;

        public MatchAnalysisManager(HRContext context, ConfigurationValues configValues, IEmbeddingGenerator<string, Embedding<float>> test, IRAGManager ragManager)
        {
            _context = context;
            _configValues = configValues;
            _test = test;
            _ragManager = ragManager;
            _promptManager = new PromptManager(_context);
        }

        public async Task MakeMatches()
        {
            var stop = 1;

            var resumes = _context.Resumes.ToList();
            var jobPostings = _context.JobPostings.ToList();
            Resume currentResume = null;
            JobPosting currentJobPosting = null;

            SimpleOutput matchOutput = null;
            SimpleOutput scoreOutput = null;
            SimpleOutput finalOutput = null;
            MatchOutput finalMatchOutput = new MatchOutput();

            for (var i = 0; i < resumes.Count; i++)
            {
                currentResume = resumes[i];

                for (var j = 0; j < jobPostings.Count; j++)
                {
                    //for (int k = 0; k < 3; k++)
                    //{



                    currentJobPosting = jobPostings[j];

                    var existingMatch = _context.MatchAnalysisResults.FirstOrDefault(x => x.ResumeId == currentResume.Id && x.JobPostingId == currentJobPosting.Id);

                    var justification = "";
                    var summary = "";
                    double score = 0.0;


                    if (existingMatch == null)
                    {
                       // for (int k = 0; k < iterations; k++)
                        //{



                            var newMatchAnalysisResult = new MatchAnalysisResult
                            {
                                CreatedOn = DateTime.Now,
                                IsActive = true,
                                //JobPosting = currentJobPosting,
                                //Resume = currentResume,
                                ResumeId = currentResume.Id,
                                JobPostingId = currentJobPosting.Id
                            };

                            // Get scores
                            justification = "";
                            summary = "";
                            score = 0.0;

                            var resumeData = await GetResumeResultById(currentResume.Id);
                            var jobPostingData = await GetJobById(currentJobPosting.Id);


                            Thread.Sleep(1000);

                            matchOutput = await GetMatchOutput(resumeData.Markdown, jobPostingData.Markdown, await _promptManager.GetMatchGeneralAnalysisPrompt());
                            finalMatchOutput.OverallMatch = matchOutput;
                            Thread.Sleep(1000);

                            scoreOutput = await GetMatchOutput(resumeData.Skills, jobPostingData.Skills, await _promptManager.GetMatchSkillsAnalysisPrompt());
                            Thread.Sleep(1000);

                            finalMatchOutput.SkillMatch = scoreOutput;
                            finalOutput = await GetMatchOutput(matchOutput.AsJson(), scoreOutput.AsJson(), await _promptManager.GetMatchCombinedPrompt());

                            newMatchAnalysisResult.GeneralMatchDetails = matchOutput.justification;
                            newMatchAnalysisResult.GeneralMatchSummary = matchOutput.summary;
                            newMatchAnalysisResult.GeneralMatchPercentage = (decimal)matchOutput.overallMatchPercentage;
                            newMatchAnalysisResult.SkillMatchDetails = scoreOutput.justification;
                            newMatchAnalysisResult.SkillMatchSummary = scoreOutput.summary;
                            newMatchAnalysisResult.SkillMatchPercentage = (decimal)scoreOutput.overallMatchPercentage;

                            newMatchAnalysisResult.OverallMatchDetails = finalOutput.justification;
                            newMatchAnalysisResult.OverallMatchSummary = finalOutput.summary;
                            newMatchAnalysisResult.OverallMatchPercentage = (decimal)finalOutput.overallMatchPercentage;

                            var matchHtml = await GetMatchOutputHTML(newMatchAnalysisResult);
                            matchHtml = matchHtml.Replace("```html", "").Replace("```", "");
                            newMatchAnalysisResult.Html = matchHtml;
                            var fileName = String.Format("New_Res_{0}_Job{1}.html", i, j);
                            File.WriteAllText(@"C:\temp\matches\" + fileName, matchHtml);

                            await _ragManager.UploadMatchToRAG(newMatchAnalysisResult);

                            _context.MatchAnalysisResults.Add(newMatchAnalysisResult);
                            await _context.SaveChangesAsync();
                        //}
                    }
                }

                // Update RAG



                //}
            }
        }

        

        private async Task<string> GetMatchOutputHTML(MatchAnalysisResult matchOutput)
        {
            var result = "";

            HttpClient customHttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60) // Set timeout to 5 minutes (or any desired duration)
            };

            OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
                    _configValues.OpenAISettings.OpenAI_ApiKey, httpClient: customHttpClient);

            var chatHistory = new ChatHistory();
            //var prompt = _promptManager.MatchPrompt;
            var prompt = await _promptManager.GetMatchHTMLPrompt();

            //matchOutput.Resume.MatchAnalysisResults = null;
            //matchOutput.JobPosting.MatchAnalysisResults = null;


            chatHistory.AddDeveloperMessage("Fill out the template that follows with this match Data: " +
                matchOutput.AsJson());
            chatHistory.AddDeveloperMessage("For candidate levels only use a single integer number (ie '7' vs '7 / 10' or '7.0')");
            chatHistory.AddDeveloperMessage("Skills should be ordered by JOB level descending.  (ie for job levels: 8,8,7,5,4) - must be in order");
            chatHistory.AddDeveloperMessage(prompt);

            //stopwatch.Start();
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("!!!Beginning chat");

            var reply = await service.GetChatMessageContentAsync(chatHistory);

            try
            {
                result = reply.Content;
            }
            catch (Exception ex)
            {
                var stop234 = 1;
            }


            return result;
        }

        private async Task<SimpleOutput> GetMatchOutput(string resumeData, string jobPostingData, string prompt, string previousMessage = null)
        {
            var result = new SimpleOutput();

            try
            {
                HttpClient customHttpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(60) // Set timeout to 5 minutes (or any desired duration)
                };


                OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
                    _configValues.OpenAISettings.OpenAI_ApiKey, httpClient: customHttpClient);

                var chatHistory = new ChatHistory();
                //var prompt = _promptManager.MatchPrompt;

                

                chatHistory.AddDeveloperMessage(prompt);
                chatHistory.AddDeveloperMessage("Resume: " + resumeData);
                chatHistory.AddDeveloperMessage("Job: " + jobPostingData);


                if (!String.IsNullOrEmpty(previousMessage))
                {
                    
                    chatHistory.AddDeveloperMessage(previousMessage);
                }

                //stopwatch.Start();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.WriteLine("!!!Beginning chat");

                var reply = await service.GetChatMessageContentAsync(chatHistory);

                try
                {
                    result = JsonSerializer.Deserialize<SimpleOutput>(reply.Content);
                }
                catch (Exception ex)
                {
                    var stop234 = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("ERROR: " + ex.StackTrace);

            }

            return result;
        }

        private async Task<ResumeResult> GetResumeResultById(int resumeId)
        {
            var vectorStore = new QdrantVectorStore(
                new QdrantClient("localhost"),
                ownsClient: true,
                new QdrantVectorStoreOptions
                {
                    EmbeddingGenerator = _test
                });


            var collection2 = vectorStore.GetCollection<Guid, RAG_Resume<Guid>>("Resume");
            await collection2.EnsureCollectionExistsAsync();

            var searchString = "Find resume records";
            var searchVector = (await _test.GenerateAsync(searchString)).Vector;
            //var searchResult2 = collection2.SearchAsync(
            //    String.Format("Find all resumes with ResumeDBId == {0}", resumeId),
            //    top: 4);
            var resultRecords = collection2.SearchAsync(searchVector, top: 4, new() { Filter = g => g.EntityId == resumeId });

            ResumeResult resumeResult = new ResumeResult();

            var result2 = "";
            //Output the matching result.
            await foreach (var result in resultRecords)
            {
                Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Data}");
                if (result.Record.Category == ResumeRAGCategories.Markdown.ToString())
                {
                    resumeResult.Markdown = result.Record.Data;
                }
                else if (result.Record.Category == ResumeRAGCategories.Skills.ToString())
                {
                    resumeResult.Skills = result.Record.Data;
                }
                //result2 = result.Record.Data;
            }

            return resumeResult;
        }


        private async Task<JobPostingResult> GetJobById(int jobId)
        {

            // The data model
            var vectorStore = new QdrantVectorStore(
                new QdrantClient("localhost"),
                ownsClient: true,
                new QdrantVectorStoreOptions
                {
                    EmbeddingGenerator = _test
                });


            var collection2 = vectorStore.GetCollection<Guid, RAG_JobPosting<Guid>>("JobPosting");
            await collection2.EnsureCollectionExistsAsync();

            var searchString = "Find job posting records";
            var searchVector = (await _test.GenerateAsync(searchString)).Vector;
            //var searchResult2 = collection2.SearchAsync(
            //    String.Format("Find all resumes with ResumeDBId == {0}", resumeId),
            //    top: 4);
            var resultRecords = collection2.SearchAsync(searchVector, top: 4, new() { Filter = g => g.EntityId == jobId });

            JobPostingResult jobPostingResult = new JobPostingResult();

            var result2 = "";
            //Output the matching result.
            await foreach (var result in resultRecords)
            {
                Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Data}");
                if (result.Record.Category == ResumeRAGCategories.Markdown.ToString())
                {
                    jobPostingResult.Markdown = result.Record.Data;
                }
                else if (result.Record.Category == ResumeRAGCategories.Skills.ToString())
                {
                    jobPostingResult.Skills = result.Record.Data;
                }
                //result2 = result.Record.Data;
            }

            return jobPostingResult;

            //return result2;
        }
    }


}
