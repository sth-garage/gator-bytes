//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
//using Microsoft.Identity.Client;
//using ResumeJobAnalysisTool.DAL.Context;
//using ResumeJobAnalysisTool.DAL.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ResumeJobAnalysisTool.DAL.CombinedModels
//{
//    public class FullMatchResultManager
//    {
//        private HRContext _context;

//        public FullMatchResultManager(HRContext context)
//        {
//            _context = context; 
//        }

//        public async Task<MatchResult> GetMatchChatAnalysisResult(int resumeId, int jobPostingId)
//        {
//            MatchResult result = new MatchResult();
//            var match = _context.MatchChatAnalysisResults.FirstOrDefault(x => x.ResumeId == resumeId && x.JobPostingId == jobPostingId);
//            if (match != null)
//            {
//                result = await GetMatchChatAnalysisResult(match.Id);
//            }

//            return result;
//        }



//        public async Task<MatchResult> GetMatchChatAnalysisResult(int matchChatAnalysisResultId)
//        {
//            var result = new MatchResult();

//            var matchAnalysis = await _context.MatchChatAnalysisResults.FirstOrDefaultAsync(x => x.Id == matchChatAnalysisResultId);
//            if (matchAnalysis != null)
//            {
//                result.OverallPercentage = (double)matchAnalysis.OverallMatchPercentage;
//                result.MatchSummary = matchAnalysis.MatchDescriptionSummary;
//                result.IsActive = matchAnalysis.IsActive;
//                //result.HTML = matchAnalysis.Html;
//                result.ResumeId = matchAnalysis.ResumeId;
//                result.JobPostingId = matchAnalysis.JobPostingId;

//                var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == result.ResumeId);
//                if (resume != null)
//                {
//                    result.CandidateName = resume.Name;
//                    result.CandidateSummary = resume.Summary;

//                    foreach (var candidateSkill in resume.ResumeSkills)
//                    {
//                        var skillName = _context.Skills.Where(x => x.Id == candidateSkill.Id).FirstOrDefault();
//                        result.CandidateSkills.Add(new SimpleCandidateOrJobSkillLevels
//                        {
//                            Name = skillName != null ? skillName.SkillName : "NOT_FOUND",
//                            Justification = candidateSkill.Justification,
//                            Level = candidateSkill.SkillLevel
//                        });
//                    }
//                }

//                var jobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Id == result.JobPostingId);
//                if (jobPosting != null)
//                {
//                    result.CompanyName = jobPosting.CompanyName;
//                    result.JobPosition = jobPosting.Position;
//                    result.JobSummary = jobPosting.Summary;

//                    foreach (var jobRequirement in jobPosting.JobRequirements)
//                    {
//                        var requirmentName = _context.Requirements.Where(x => x.Id == jobRequirement.Id).FirstOrDefault();
//                        result.JobRequirements.Add(new SimpleCandidateOrJobSkillLevels
//                        {
//                            Name = requirmentName != null ? requirmentName.Name : "NOT_FOUND",
//                            Justification = jobRequirement.Justification,
//                            Level = jobRequirement.MinimumLevel.HasValue ? jobRequirement.MinimumLevel.Value : 0
//                        });
//                    }
//                }

//                foreach (var persona in _context.AgentPersonas)
//                {
//                    result.Personas.Add(new SimplePersona
//                    {
//                        IsFinalReviewer = persona.IsFinalApprover ?? false,
//                        Description = persona.Description,
//                        Name = persona.Name,
//                        PersonaId = persona.Id,
//                    });
//                }

//                var agentResult = _context.AnalysisAgentResults.Where(x => x.MatchChatAnalysisResultId == matchChatAnalysisResultId).ToList();
//                foreach (var analysis in agentResult)
//                {
//                    result.AgentMatchAnalyses.Add(new AgentMatchAnalysis
//                    {
//                        AgentChatCount = analysis.AgentChatNumber ?? -1,
//                        AgentName = result.Personas.First(x => x.PersonaId == analysis.AgentPersonaId).Name,
//                        MatchPercentageAdjustment = (double)(analysis.MatchPercentageAdjustment ?? 0),
//                        Count = analysis.AgentChatNumberOverall ?? 0,
//                        Justification = analysis.Justification,
//                        Summary = analysis.Summary,
//                        SimplePersonaId = analysis.AgentPersonaId,
//                        MatchPercentage = (double)(analysis.MatchPercentage),
//                        MatchPercentageAdjustmentJustification = analysis.MatchPercentageAdjustmentJustification
//                    });
//                }


//            }



//            return result;
//        }



 


//    }

//    public class MatchResult
//    {

//        public double OverallPercentage { get; set; }

//        public string MatchSummary { get; set; }

//        public bool IsActive { get; set; }

//        //public string HTML { get; set; }

//        public int ResumeId { get; set; }

//        public int JobPostingId { get; set; }

//        public string CandidateName { get; set; }

//        public string CandidateSummary { get; set; }

//        public string CompanyName { get; set; }

//        public string JobPosition { get; set; }

//        public string JobSummary { get; set; }

//        public List<SimpleCandidateOrJobSkillLevels> CandidateSkills { get; set; } = new List<SimpleCandidateOrJobSkillLevels>();

//        public List<SimpleCandidateOrJobSkillLevels> JobRequirements { get; set; } = new List<SimpleCandidateOrJobSkillLevels>();

//        public List<SimplePersona> Personas { get; set; } = new List<SimplePersona>();

//        public List<AgentMatchAnalysis> AgentMatchAnalyses { get; set; } = new List<AgentMatchAnalysis>();
//    }

//    public class AgentMatchAnalysis
//    {
//        public int Count { get; set; }

//        public string AgentName { get; set; }

//        public int AgentChatCount { get; set; }

//        public int SimplePersonaId { get; set; }

//        public double MatchPercentage { get; set; }

//        public string Summary { get; set; }

//        public string Justification { get; set; }

//        public double MatchPercentageAdjustment { get; set; }

//        public string MatchPercentageAdjustmentJustification { get; set; }
//    }


//    public class SimpleCandidateOrJobSkillLevels
//    {
//        public string Name { get; set; }

//        public int Level { get; set; }

//        public string Justification { get; set; }
//    }

//    public class SimplePersona
//    {
//        public string Name { get; set; }

//        public string Description { get; set; }

//        public bool IsFinalReviewer { get; set; }

//        public int PersonaId { get; set; }
//    }

    
//}
