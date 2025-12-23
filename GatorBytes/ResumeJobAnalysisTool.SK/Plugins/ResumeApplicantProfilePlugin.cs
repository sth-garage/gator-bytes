using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI.VectorStores;
using Qdrant.Client;


//using ResumeJobAnalysisTool.DAL.CombinedModels;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.SK.RAG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.Json;
using static Microsoft.KernelMemory.Constants.CustomContext;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.SK.Plugins
{
    public class ResumeApplicantProfilePlugin
    {
        private HRContext _context;
        private ConfigurationValues _configValues;
        //private Kernel _kernel;
        //private FullMatchResultManager _fullMatchResultManager;
        IEmbeddingGenerator<string, Embedding<float>> _test;

        public ResumeApplicantProfilePlugin(HRContext context, ConfigurationValues configValues, IEmbeddingGenerator<string, Embedding<float>> test)
        {
            _context = context;
            _configValues = configValues;
            _test = test;
            //_kernel = kernel;
            //_fullMatchResultManager = new FullMatchResultManager(context);
        }


        //[KernelFunction("get_resume_for_applicant")]
        //[Description("Takes in a name that the user is searching for attempts to find the correct Resume to return.")]
        //public async Task<Resume> GetResumeForJobAppliantByName(string name)
        //{
        //    OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
        //        _configValues.OpenAISettings.OpenAI_ApiKey);

        //    ChatHistory chatHistory = new ChatHistory();

        //    var existingNames = String.Join(", ", _context.Resumes.Select(x => x.Name));

        //    var nameMessage = String.Format("Using these names: {0} - find the name that most closely matches the user input: {1}.  RETURN ONLY THE NAME - IF THERE ARE NO MATCHES RETURN 'NONE'",
        //        existingNames, name);

        //    chatHistory.AddDeveloperMessage(nameMessage);

        //    var foundName = await service.GetChatMessageContentAsync(chatHistory);

        //    return _context.Resumes.First();
        //}


        //[KernelFunction("get_job_posting")]
        //public async Task<string> GetJobPosting()
        //{

        //    // The data model
        //    var vectorStore = new QdrantVectorStore(
        //        new QdrantClient("localhost"),
        //        ownsClient: true,
        //        new QdrantVectorStoreOptions
        //        {
        //            EmbeddingGenerator = _test
        //        });


        //    var collection2 = vectorStore.GetCollection<Guid, RAG_JobPosting<Guid>>("JobPosting");
        //    await collection2.EnsureCollectionExistsAsync();


        //    var searchResult2 = collection2.SearchAsync(
        //        "Who are in the job postings?",
        //        top: 1);


        //    var result2 = "";
        //    //Output the matching result.
        //    await foreach (var result in searchResult2)
        //    {
        //        Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Data}");
        //        result2 = result.Record.Data;
        //    }

        //    return result2;
        //}

        //[KernelFunction("get_resume")]
        //public async Task<string> GetResume()
        //{

        //    // The data model
        //    var vectorStore = new QdrantVectorStore(
        //        new QdrantClient("localhost"),
        //        ownsClient: true,
        //        new QdrantVectorStoreOptions
        //        {
        //            EmbeddingGenerator = _test
        //        });


        //    var collection2 = vectorStore.GetCollection<Guid, RAG_Resume<Guid>>("Resume");
        //    await collection2.EnsureCollectionExistsAsync();


        //    var searchResult2 = collection2.SearchAsync(
        //        "Who are in the resumes?",
        //        top: 1);


        //    var result2 = "";
        //    //Output the matching result.
        //    await foreach (var result in searchResult2)
        //    {
        //        Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Data}");
        //        result2 = result.Record.Data;
        //    }

        //    return result2;
        //}


        //[KernelFunction("get_resume_for_applicant")]
        //public async Task<string> GetResumeIdByName(string name)
        //{

        //    // The data model
        //    var vectorStore = new QdrantVectorStore(
        //        new QdrantClient("localhost"),
        //        ownsClient: true,
        //        new QdrantVectorStoreOptions
        //        {
        //            EmbeddingGenerator = _test
        //        });


        //    var collection2 = vectorStore.GetCollection<Guid, RAG_Resume<Guid>>("Resume");
        //    await collection2.EnsureCollectionExistsAsync();


        //    var searchResult2 = collection2.SearchAsync(
        //        "Who are in the resumes?",
        //        top: 1);


        //    var result2 = "";
        //    //Output the matching result.
        //    await foreach (var result in searchResult2)
        //    {
        //        Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Data}");
        //        result2 = result.Record.Data;
        //    }

        //    return result2;
        //}

        [KernelFunction("get_resumes_or_applicants")]
        public async Task<List<string>> GetResumesOrApplicantNames()
        {
            var names = _context.Resumes.Select(x => x.Name).ToList();
            return names;
        }

        [KernelFunction("get_jobs")]
        public async Task<List<string>> GetJobNames()
        {
            var names = _context.JobPostings.Select(x => x.Name).ToList();
            return names;
        }


        [KernelFunction("get_resume_by_id")]
        public async Task<ResumeResult> GetResumeIdByName(int resumeId)
        {

            // The data model
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

            //return result2;
        }







        [KernelFunction("get_job_by_id")]
        public async Task<JobPostingResult> GetJobById(int jobId)
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






        [KernelFunction("get_resume_id_for_applicant")]
        public async Task<int> GetApplicantNameId(string name)
        {
            var result = -1;
            OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
                _configValues.OpenAISettings.OpenAI_ApiKey);

            ChatHistory chatHistory = new ChatHistory();
            var existingNames = String.Join(", ", _context.Resumes.Select(x => x.Name));
            var nameMessage = String.Format("Using these names: {0} - find the name that most closely matches the user input: {1}.  RETURN ONLY THE NAME - IF THERE ARE NO MATCHES RETURN 'NONE'",
                existingNames, name);
            chatHistory.AddDeveloperMessage(nameMessage);
            var message = await service.GetChatMessageContentAsync(chatHistory);
            var foundName = message.Content != null ? message.Content : "NOTFOUND";
            if (foundName != "NOTFOUND")
            {
                var resume = _context.Resumes.FirstOrDefault(x => x.Name == foundName);
                result = resume.Id;
            }

            return result;
        }

        [KernelFunction("get_job_id_for_applicant")]
        public async Task<int> GetJobByNameId(string name)
        {
            var result = -1;
            OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
                _configValues.OpenAISettings.OpenAI_ApiKey);

            ChatHistory chatHistory = new ChatHistory();
            var existingNames = String.Join(", ", _context.JobPostings.Select(x => x.Name));
            var nameMessage = String.Format("Using these names: {0} - find the name that most closely matches the user input: {1}.  RETURN ONLY THE NAME - IF THERE ARE NO MATCHES RETURN 'NONE'",
                existingNames, name);
            chatHistory.AddDeveloperMessage(nameMessage);
            var message = await service.GetChatMessageContentAsync(chatHistory);
            var foundName = message.Content != null ? message.Content : "NOTFOUND";
            if (foundName != "NOTFOUND")
            {
                var jobPosting = _context.JobPostings.FirstOrDefault(x => x.Name == foundName);
                result = jobPosting.Id;
            }

            return result;
        }

        [KernelFunction("get_resume_to_job_match_data")]
        public async Task<string> GetApplicantToJobMatchResults(int resumeId, int jobId)
        {
            var result = "Use general search to make match data";
            var match = _context.MatchAnalysisResults.FirstOrDefault(x => x.ResumeId == resumeId && x.JobPostingId == jobId);
            if (match != null)
            {
                match.JobPosting = null;
                match.Resume = null;
                result = match.AsJson();
            }

            return result;
        }
    }
}
