using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class UploadResumeManager : UploadManagerBase<ResumeFileSystemUploadEntry, Resume>
    {
        public UploadResumeManager(Kernel kernel,
            ConfigurationValues configValues,
            IChatCompletionService chatCompletionService,
            HRContext context,
            IFileAnalysisManager fileAnalysisManager,
            IVectorProcessor vectorProcessor,
            IRAGManager ragManager) : base(kernel, configValues, chatCompletionService, context, fileAnalysisManager, vectorProcessor, ragManager)
        {
        }

        public Resume CreateNewResume(string name,
            string personality,
            string summary,
            string html,
            DocumentUpload documentUpload)
        {
            return new Resume
            {
                //AppIdentifier = appIdentifier,
                Name = name,
                Personality = personality,
                CreatedOn = DateTime.Now,
                Summary = summary,
                Html = html,
                //Json = json,
                DocumentUpload = documentUpload,
                IsActive = true
            };
        }

        public override async Task<Resume> GetOrCreateBaseEntity(string name, Resume resume)
        {
            var matchingResume = await _context.Resumes.FirstOrDefaultAsync(x => x.Name == name);
            if (matchingResume == null)
            {
                await _context.Resumes.AddAsync(resume);

                await _context.SaveChangesAsync();

                matchingResume = await _context.Resumes.FirstOrDefaultAsync(x => x.Name == name);
            }

            return matchingResume;
        }



        public override async Task ProcessResumeEntry(DocumentUpload documentUpload)
        {
            Console.WriteLine("Beginning processing...");
            

            List<string> skillNames = await GetExistingSkills();

            Console.WriteLine("   Skills found: " + skillNames.Count);
            Console.WriteLine("   Skills: " + String.Join(",", skillNames));
            //Console.WriteLine(" Limiting skills")

            var analysis = await _fileAnalysisManager.GetResumeAnalysis(documentUpload);
            //var analysis = JsonSerializer.Deserialize<ResumeResult>(File.ReadAllText(@"C:\Users\sholt\source\repos\SemanticKernel_Job_Resume_Analysis_PRIVATE2\SharedData\Temp\temptemptemp.json"));
            await _context.SaveChangesAsync();
            Console.WriteLine("   Analysis complete");

            if (analysis != null)
            {
                //Guid ragGuid = Guid.NewGuid();


                ResumeFileSystemUploadEntry uploadEntry = GetResumeFileSystemUploadEntry(documentUpload.FileName, analysis);

                // Save for file output TODO make better
                //var tempHTML = analysis.HTML;

                //File.WriteAllText(@"C:\Users\sholt\source\repos\SemanticKernel_Job_Resume_Analysis_PRIVATE2\SharedData\SampleDocuments\Resumes\tempout\test.html", tempHTML);

                //var htmlBytes = Encoding.UTF8.GetBytes(tempHTML);

                var analysisJson = analysis.AsJson();
                var jsonBytes = Encoding.UTF8.GetBytes((string)analysisJson);

                Console.WriteLine("Creating doc upload");
                //DocumentUpload documentUpload = await CreateDocumentUploadEntry(fileInfo);

                var generatedResume = CreateNewResume(uploadEntry.Name,
                    uploadEntry.Personality,
                    uploadEntry.Summary,
                    analysis.HTML,
                    documentUpload);

                var resume = await GetOrCreateBaseEntity(analysis.Name, generatedResume);
                await _context.SaveChangesAsync();
                Console.WriteLine("Resume created: " + resume.Name);

                await ExtractAndAddSkills(analysis, resume);

                await _context.SaveChangesAsync();

                Console.WriteLine("Creating doc-resume");
                await _context.SaveChangesAsync();

                uploadEntry.ResumeDBId = resume.Id;
                try
                {
                    await _ragManager.UploadResumeToRAG(uploadEntry);
                }
                catch (Exception ex)
                {
                    _context.Resumes.Remove(resume);
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

        private async Task<List<string>> GetExistingSkills()
        {
            var skills = await _context.Skills.ToListAsync();
            var skillNames = skills != null && skills.Count > 0 ? skills.OrderByDescending(x => x.SkillName.Count()).Select(x => x.SkillName).Distinct().ToList() : new List<string>();
            return skillNames;
        }

        private static ResumeFileSystemUploadEntry GetResumeFileSystemUploadEntry(string fileName, ResumeResult analysis)
        {
            return new ResumeFileSystemUploadEntry
            {
                FileName = fileName,
                //FilePath = fileInfo.DirectoryName ?? "",
                HTML = analysis.HTML,
                Name = analysis.Name,
                Summary = analysis.Summary,
                Personality = analysis.Personality,
                JSONSkills = analysis.Skills,
                Markdown = analysis.Markdown,
                Text = analysis.Text,
            };
        }

        private async Task ExtractAndAddSkills(ResumeResult analysis, Resume resume)
        {
            foreach (var skill in analysis.SkillsList.skills)
            {
                var foundSkill = await _context.Skills.FirstOrDefaultAsync(x => x.SkillName == skill.name);
                if (foundSkill == null && skill != null)
                {
                    foundSkill = await CreateNewSkill(skill, foundSkill);
                    await _context.SaveChangesAsync();
                }

                await AddResumeSkill(resume, skill, foundSkill, resume.Id);

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddResumeSkill(Resume resume, resumeSkillJSON skill, Skill foundSkill, int resumeId)
        {
            Console.WriteLine("Adding skills with stats");

            //var usedWith = String.Join(", ", skill.usedWith);
            //decimal? lastUsed = skill.lastUsed != null ? (decimal?)skill.lastUsed : null;

            var skillWithStats = new ResumeSkill
            {
                CreatedOn = DateTime.Now,
                Resume = resume,
                Justification = skill.justification,
                SkillLevel = skill.level,
                Skill = foundSkill,
                IsActive = true,
                //LastUsed = lastUsed,
                //UsedWith = usedWith,
                

            };

            _context.ResumeSkills.Add(skillWithStats);

            await _context.SaveChangesAsync();


            //var skillWithStats2 = new ResumeSkill
            //{
            //    CreatedOn = DateTime.Now,
            //    ResumeId = resumeId,
            //    Justification = skill.justification,
            //    SkillLevel = skill.level,
            //    Skill = foundSkill,
            //    IsActive = true,
            //    //LastUsed = lastUsed,
            //    //UsedWith = usedWith,


            //};

            //_context.ResumeSkills.Add(skillWithStats2);

            await _context.SaveChangesAsync();
        }

        private async Task<Skill> CreateNewSkill(resumeSkillJSON skill, Skill foundSkill)
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
