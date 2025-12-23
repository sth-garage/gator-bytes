using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeJobAnalysisTool.Shared
{
    public static class Enums
    {
        public enum RAGCollections
        {
            //ResumeHTML,
            //ResumeJSON,
            //ResumeTechHTML,
            //ResumeTechJSON,
            //JobPostingHTML,
            //JobPostingJSON,
            Resume,
            JobPosting,
            //JobPostingTechHTML,
            /// <summary>
            /// JobPostingTechJSON,
            /// </summary>
            ResumeJobMatchAnalysis
        }

        public enum ResumeRAGCategories
        {
            PlainText,
            HTML,
            Markdown,
            Skills
        }

        public enum JobPostingRAGCategories
        {
            PlainText,
            HTML,
            Markdown,
            Skills
        }

        public enum MatchRAGCategories
        {
            ResumeJobMatchGeneral,
            ResumeJobMatchSkills,
            ResumeJobMatchCombined,
            ResumeJobMatchHTML
        }

        public enum PromptTypes
        {
            System = 1,
            GenericPDFFileConversion = 2,
            ResumeGeneralFileAnalysis = 3,
            JobPostingGeneralFileAnalysis = 4,
            ResumeSkillAnalysis = 5,
            JobPostingSkillAnalysis = 6,
            MatchGeneralAnalysis = 7,
            MatchSkillAnalysis = 8,
            MatchOverallAnalysis = 9,
            ResumeAnalysisToHTML = 10,
            JobPostingAnalysisToHTML = 11,
            MatchAnalysisHTML = 12
        }

        public enum Personas
        {
            TaylorGrant = 1,
            JordanLee = 2,
            SunnySam = 3,
            JonathanReyes = 4,
            InitialSetup = 5,
            FinalReviewer = 6
        }
    }
}
