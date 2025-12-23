using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.SK.RAG;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.AppLogic
{
    public class RAGManager : IRAGManager
    {
        private Kernel _kernel = null;

        public RAGManager(ConfigurationValues configValues, Kernel kernel, IChatCompletionService chatCompletionService)
        {
            _kernel = kernel;
        }

        public async Task UploadResumeToRAG(ResumeFileSystemUploadEntry uploadEntry)
        {
            var embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStoreFactory = new VectorStoreFactory(embeddingGenerator);

            var vectorStore = vectorStoreFactory.VectorStore;
            var vectorResume = new VectorProcessor(vectorStore, embeddingGenerator);
            // text

            await vectorResume.IngestResumeDataAsync(RAGCollections.Resume.ToString(),
                ResumeRAGCategories.HTML.ToString(),
                uploadEntry.HTML,
                uploadEntry.Name,
                () => Guid.NewGuid(),
                uploadEntry.FileName,
                uploadEntry.ResumeDBId);


            await vectorResume.IngestResumeDataAsync(RAGCollections.Resume.ToString(),
                ResumeRAGCategories.Markdown.ToString(),
                uploadEntry.Markdown,
                uploadEntry.Name,
                () => Guid.NewGuid(),
                uploadEntry.FileName,
                uploadEntry.ResumeDBId);


            await vectorResume.IngestResumeDataAsync(RAGCollections.Resume.ToString(),
                    ResumeRAGCategories.PlainText.ToString(),
                    uploadEntry.Text,
                    uploadEntry.Name,
                    () => Guid.NewGuid(),
                    uploadEntry.FileName,
                    uploadEntry.ResumeDBId);

            await vectorResume.IngestResumeDataAsync(RAGCollections.Resume.ToString(),
                    ResumeRAGCategories.Skills.ToString(),
                    uploadEntry.JSONSkills,
                    uploadEntry.Name,
                    () => Guid.NewGuid(),
                    uploadEntry.FileName,
                    uploadEntry.ResumeDBId);

        }



        public async Task UploadMatchToRAG(MatchAnalysisResult matchResult)
        {
            var embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStoreFactory = new VectorStoreFactory(embeddingGenerator);

            var vectorStore = vectorStoreFactory.VectorStore;
            var vectorResume = new VectorProcessor(vectorStore, embeddingGenerator);
            // text

            await vectorResume.IngestMatchAnalysisDataAsync(RAGCollections.ResumeJobMatchAnalysis.ToString(), MatchRAGCategories.ResumeJobMatchGeneral.ToString(),
                matchResult.GeneralMatchDetails, matchResult.GeneralMatchSummary, (double)matchResult.GeneralMatchPercentage, matchResult.ResumeId, matchResult.JobPostingId,
                () => Guid.NewGuid()
                );

            await vectorResume.IngestMatchAnalysisDataAsync(RAGCollections.ResumeJobMatchAnalysis.ToString(), MatchRAGCategories.ResumeJobMatchSkills.ToString(),
                matchResult.SkillMatchDetails, matchResult.SkillMatchSummary, (double)matchResult.SkillMatchPercentage, matchResult.ResumeId, matchResult.JobPostingId,
                () => Guid.NewGuid()
                );

            await vectorResume.IngestMatchAnalysisDataAsync(RAGCollections.ResumeJobMatchAnalysis.ToString(), MatchRAGCategories.ResumeJobMatchCombined.ToString(),
                matchResult.OverallMatchDetails, matchResult.OverallMatchSummary, (double)matchResult.OverallMatchPercentage, matchResult.ResumeId, matchResult.JobPostingId,
                () => Guid.NewGuid()
                );

            await vectorResume.IngestMatchAnalysisHTMLDataAsync(RAGCollections.ResumeJobMatchAnalysis.ToString(), MatchRAGCategories.ResumeJobMatchHTML.ToString(),
                matchResult.Html, (double)matchResult.OverallMatchPercentage, matchResult.ResumeId, matchResult.JobPostingId,
                () => Guid.NewGuid()
                );
        }




        public async Task UploadJobPostingToRAG(JobPostingFileSystemUploadEntry uploadEntry)
        {
            var embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStoreFactory = new VectorStoreFactory(embeddingGenerator);

            var vectorStore = vectorStoreFactory.VectorStore;
            var vectorResume = new VectorProcessor(vectorStore, embeddingGenerator);
            // text

            await vectorResume.IngestJobDataAsync(RAGCollections.JobPosting.ToString(),
                JobPostingRAGCategories.HTML.ToString(),
                uploadEntry.HTML,
                uploadEntry.Name,
                () => Guid.NewGuid(),
                uploadEntry.FileName,
                uploadEntry.JobPostingDBId,
                uploadEntry.CompanyName,
                uploadEntry.Position
                );



            await vectorResume.IngestJobDataAsync(RAGCollections.JobPosting.ToString(),
                JobPostingRAGCategories.Markdown.ToString(),
                uploadEntry.Markdown,
                uploadEntry.Name,
                () => Guid.NewGuid(),
                uploadEntry.FileName,
                uploadEntry.JobPostingDBId,
                uploadEntry.CompanyName,
                uploadEntry.Position);


            await vectorResume.IngestJobDataAsync(RAGCollections.JobPosting.ToString(),
                    JobPostingRAGCategories.PlainText.ToString(),
                    uploadEntry.Text,
                    uploadEntry.Name,
                    () => Guid.NewGuid(),
                    uploadEntry.FileName,
                    uploadEntry.JobPostingDBId,
                    uploadEntry.CompanyName,
                uploadEntry.Position);

            await vectorResume.IngestJobDataAsync(RAGCollections.JobPosting.ToString(),
                    JobPostingRAGCategories.Skills.ToString(),
                    uploadEntry.JSONSkills,
                    uploadEntry.Name,
                    () => Guid.NewGuid(),
                    uploadEntry.FileName,
                    uploadEntry.JobPostingDBId,
                    uploadEntry.CompanyName,
                uploadEntry.Position);

        }






    }
}
