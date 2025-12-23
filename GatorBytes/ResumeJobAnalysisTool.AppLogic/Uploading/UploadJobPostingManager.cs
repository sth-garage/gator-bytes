using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.DAL.ModifiedModels;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Prompts;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.SK.RAG;
using static System.Net.Mime.MediaTypeNames;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.AppLogic.Uploading
{
    public class UploadJobPostingManager : UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting>
    {
        public UploadJobPostingManager(Kernel kernel,
            ConfigurationValues configValues,
            IChatCompletionService chatCompletionService,
            HRContext context,
            IFileAnalysisManager fileAnalysisManager,
            IVectorProcessor vectorProcessor,
            IRAGManager ragManager) : base(kernel, configValues, chatCompletionService, context, fileAnalysisManager, vectorProcessor, ragManager)
        {
        }

        public JobPosting CreateNewPosting(string name,
            string companyName,
            string position,
            string summary,
            //Guid appIdentifier,
            string html,
            string json,
            DocumentUpload documentUpload)
        {
            return new JobPosting
            {
                Name = name,
                CompanyName = companyName,
                Position = position,
                CreatedOn = DateTime.Now,
                Summary = summary,
                Html = html,
                DocumentUpload = documentUpload,
                IsActive = true,
            };
        }

        public override async Task<JobPosting> GetOrCreateBaseEntity(string name, JobPosting jobPosting)
        {
            var matchingJobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Name == name);
            if (matchingJobPosting == null)
            {
                await _context.JobPostings.AddAsync(jobPosting);
                await _context.SaveChangesAsync();

                matchingJobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Name == name);
            }

            return matchingJobPosting;
        }

        public override async Task ProcessJobPostingEntry(DocumentUpload documentUpload)
        {
            Console.WriteLine("Beginning processing...");

            List<string> requirementNames = new List<string>(); // await GetExistingRequirements();

            Console.WriteLine("   Requirements found: " + requirementNames.Count);
            Console.WriteLine("   Requirements: " + String.Join(",", requirementNames));

            var analysis = await _fileAnalysisManager.GetJobPostingAnalysis(documentUpload);
            await _context.SaveChangesAsync();
            Console.WriteLine("   Analysis complete");

            if (analysis != null)
            {
                JobPostingFileSystemUploadEntry uploadEntry = GetJobPostingFileSystemUploadEntry(documentUpload.FileName, analysis);

                // Save for file output TODO make better
                var tempHTML = analysis.HTML;
                var htmlBytes = Encoding.UTF8.GetBytes(tempHTML);

                var analysisJson = analysis.AsJson();
                var jsonBytes = Encoding.UTF8.GetBytes((string)analysisJson);

                Console.WriteLine("Creating doc upload");


                var generatedJobPosting = CreateNewPosting(uploadEntry.Name,
                    uploadEntry.CompanyName,
                    uploadEntry.Position,
                    uploadEntry.Summary,
                    tempHTML,
                    analysisJson,
                    documentUpload);

                var jobPosting = await GetOrCreateBaseEntity(analysis.Name, generatedJobPosting);
                await _context.SaveChangesAsync();
                Console.WriteLine("Job Posting created: " + jobPosting.Name);

                await ExtractAndAddSkills(analysis, jobPosting);

                await _context.SaveChangesAsync();

                Console.WriteLine("Creating doc-resume");
                await _context.SaveChangesAsync();

                uploadEntry.JobPostingDBId = jobPosting.Id;
                try
                {
                    await _ragManager.UploadJobPostingToRAG(uploadEntry);
                }
                catch (Exception ex)
                {
                    _context.JobPostings.Remove(jobPosting);
                    await _context.SaveChangesAsync();
                }
                documentUpload.HasBeenProcessed = true;
                await _context.SaveChangesAsync();
                Console.WriteLine("Writing output files");
            }
            else
            {
                Console.WriteLine("ANALYSIS IS NULL");
            }

            return;
        }

        private static JobPostingFileSystemUploadEntry GetJobPostingFileSystemUploadEntry(string fileName, JobPostingResult analysis)
        {
            return new JobPostingFileSystemUploadEntry
            {
                FileName = fileName,
                //FilePath = fileInfo.DirectoryName ?? "",
                HTML = analysis.HTML,
                Name = analysis.Name,
                Summary = analysis.Summary,
                Position = analysis.Position,
                CompanyName = analysis.CompanyName,
                Markdown = analysis.Markdown,
                Text = analysis.Text,
                JSONSkills = analysis.Skills
            };
        }

        private async Task ExtractAndAddSkills(JobPostingResult analysis, JobPosting resume)
        {
            foreach (var skill in analysis.SkillsList.skills)
            {
                var foundSkill = await _context.Skills.FirstOrDefaultAsync(x => x.SkillName == skill.name);
                if (foundSkill == null && skill != null)
                {
                    foundSkill = await CreateNewSkill(skill, foundSkill);
                    await _context.SaveChangesAsync();
                }

                await AddJobSkill(resume, skill, foundSkill);

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddJobSkill(JobPosting resume, jobSkillJSON skill, Skill foundSkill)
        {
            Console.WriteLine("Adding skills with stats");

            //var usedWith = String.Join(", ", skill.usedWith);
            //Decimal? lastUsed = skill.lastUsed != null ? (decimal?)skill.lastUsed : null;

            var skillWithStats = new JobSkill
            {
                CreatedOn = DateTime.Now,
                JobPosting = resume,
                Justification = skill.justification,
                MinimumLevel = skill.minimumLevel,
                Skill = foundSkill,
                IsActive = true,
                //LastUsed = lastUsed,
                DesiredLevel = skill.desiredLevel,

            };

            _context.JobSkills.Add(skillWithStats);

            await _context.SaveChangesAsync();
        }

        private async Task<Skill> CreateNewSkill(jobSkillJSON skill, Skill foundSkill)
        {
            Console.WriteLine("Skill added: " + skill.name);
            var tempSkill = await _context.Skills.AddAsync(new Skill
            {
                SkillName = skill.name,
                CreatedOn = DateTime.Now,
                IsActive = true
            });

            await _context.SaveChangesAsync();

            foundSkill = tempSkill.Entity;
            return foundSkill;
        }
    }
}