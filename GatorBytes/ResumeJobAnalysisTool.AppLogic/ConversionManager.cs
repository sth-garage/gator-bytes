using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeJobAnalysisTool.AppLogic
{
    public class ConversionManager
    {
        public ConversionManager() { }

        public SimpleResumeDTO GetSimpleResume(Resume resume)
        {
            var skills = resume.ResumeSkills;

            var skillString = skills.OrderByDescending(x => x.SkillLevel).Take(3);
            var skillsResult = new List<string>();
            
            foreach (var skill in skillString)
            {
                skillsResult.Add(String.Format("{0} - {1}", skill.Skill.SkillName, skill.SkillLevel));
            }



            return new SimpleResumeDTO
            {
                Name = resume.Name,
                Id = resume.Id,
                Skills = skillsResult
            };
        }


        public SimpleJobDTO GetSimpleJobs(JobPosting jobPosting)
        {
            var skills = jobPosting.JobSkills;

            var skillString = skills.OrderByDescending(x => x.DesiredLevel).Take(3);
            var skillsResult = new List<string>();

            foreach (var skill in skillString)
            {
                skillsResult.Add(String.Format("{0} - {1}", skill.Skill.SkillName, skill.DesiredLevel));
            }



            return new SimpleJobDTO
            {
                Name = jobPosting.Name,
                Id = jobPosting.Id,
                Skills = skillsResult
            };
        }

        public SimpleMatchDTO GetSimpleMatch(MatchAnalysisResult matchAnalysisResult)
        {
            SimpleMatchDTO result = new SimpleMatchDTO();

            result.OverallScore = (double) matchAnalysisResult.OverallMatchPercentage;
            result.TechnicalScore = (double)matchAnalysisResult.SkillMatchPercentage;
            result.GeneralScore = (double) matchAnalysisResult.GeneralMatchPercentage;

            result.Summary = matchAnalysisResult.OverallMatchSummary;


            return result;
        }
    }
}
