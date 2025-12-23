using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.Shared.Interfaces;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.Shared
{
    public class PromptManager
    {
        private readonly HRContext _hrContext;
        private string _systemPrompt;
        private string _inputToText;
        private string _resumeGeneralAnalysisPrompt;
        private string _resumeSkillAnalysisPrompt;
        private string _jobGeneralAnalysisPrompt;
        private string _jobSkillAnalysisPrompt;
        private string _matchGeneralPrompt;
        private string _matchSkillPrompt;
        private string _matchCombinedPrompt;
        private string _resumeToHTMLPrompt;
        private string _jobToHTMLPrompt;
        private string _matchToHTMLPrompt;

        public PromptManager(HRContext hrContext)
        {
            _hrContext = hrContext;
        }

        public string GetSystemPrompt()
        {
            if (_systemPrompt == null)
            {
                var systemPrompt = _hrContext.Prompts.FirstOrDefault(x => x.IsActive && x.PromptTypeId == (int)PromptTypes.System);
                _systemPrompt = systemPrompt.PromptText + " - when asked about names assume they are job applicants with resumes and when asked about companies or positions assume they are for job postings.";
            }

            return _systemPrompt;
        }

        public async Task<string> GetInputConversionToTextPrompt()
        {
            if (String.IsNullOrEmpty(_inputToText))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.GenericPDFFileConversion);

                _inputToText = dbPrompt.PromptText;
            }

            return _inputToText;
        }

        public async Task<string> GetResumeGeneralAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_resumeGeneralAnalysisPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.ResumeGeneralFileAnalysis);

                _resumeGeneralAnalysisPrompt = dbPrompt.PromptText;
            }

            return _resumeGeneralAnalysisPrompt;
        }

        public async Task<string> GetResumeSkillsAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_resumeSkillAnalysisPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.ResumeSkillAnalysis);

                _resumeSkillAnalysisPrompt = dbPrompt.PromptText;
            }

            return _resumeSkillAnalysisPrompt;
        }


        public async Task<string> GetResumeToHTMLPrompt()
        {
            if (String.IsNullOrEmpty(_resumeToHTMLPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.ResumeAnalysisToHTML);

                _resumeToHTMLPrompt = dbPrompt.PromptText;
            }

            return _resumeToHTMLPrompt;
        }


        public async Task<string> GetJobGeneralAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_jobGeneralAnalysisPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.JobPostingGeneralFileAnalysis);

                _jobGeneralAnalysisPrompt = dbPrompt.PromptText;
            }

            return _jobGeneralAnalysisPrompt;
        }

        public async Task<string> GetJobSkillsAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_jobSkillAnalysisPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.JobPostingSkillAnalysis);

                _jobSkillAnalysisPrompt = dbPrompt.PromptText;
            }

            return _jobSkillAnalysisPrompt;
        }


        public async Task<string> GetJobToHTMLPrompt()
        {
            if (String.IsNullOrEmpty(_jobToHTMLPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.JobPostingAnalysisToHTML);

                _jobToHTMLPrompt = dbPrompt.PromptText;
            }

            return _jobToHTMLPrompt;
        }



        public async Task<string> GetMatchGeneralAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_matchGeneralPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.MatchGeneralAnalysis);

                _matchGeneralPrompt = dbPrompt.PromptText + " And provide an answer in a natural way in HTML format, using paragraphs as needed and bold/italics/underline for emphasis.";
            }

            return _matchGeneralPrompt;
        }

        public async Task<string> GetMatchSkillsAnalysisPrompt()
        {
            if (String.IsNullOrEmpty(_matchSkillPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.MatchSkillAnalysis);

                _matchSkillPrompt = dbPrompt.PromptText + " And you MUST provide an answer in a natural way in HTML format, using paragraphs as needed and bold/italics/underline for emphasis.  Use tables or lists for skills and summary analysis if needed.";
            }

            return _matchSkillPrompt;
        }


        public async Task<string> GetMatchCombinedPrompt()
        {
            if (String.IsNullOrEmpty(_matchCombinedPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.MatchOverallAnalysis);

                _matchCombinedPrompt = dbPrompt.PromptText + " And provide an answer in a natural way in HTML format, using paragraphs as needed and bold/italics/underline for emphasis.";
            }

            return _matchCombinedPrompt;
        }

        public async Task<string> GetMatchHTMLPrompt()
        {
            if (String.IsNullOrEmpty(_matchToHTMLPrompt))
            {
                var dbPrompt = await _hrContext.Prompts.FirstOrDefaultAsync(x => x.IsActive
                    && x.PromptTypeId == (int)Enums.PromptTypes.MatchAnalysisHTML);

                _matchToHTMLPrompt = dbPrompt.PromptText;
            }

            return _matchToHTMLPrompt;
        }

    }
}
