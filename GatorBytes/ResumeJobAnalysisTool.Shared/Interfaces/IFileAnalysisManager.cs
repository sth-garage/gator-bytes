using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.DAL.Models;

namespace ResumeJobAnalysisTool.Shared.Interfaces
{
    public interface IFileAnalysisManager
    {
        Task<ResumeResult> GetResumeAnalysis(DocumentUpload file);

        Task<JobPostingResult> GetJobPostingAnalysis(DocumentUpload file);
    }
}
