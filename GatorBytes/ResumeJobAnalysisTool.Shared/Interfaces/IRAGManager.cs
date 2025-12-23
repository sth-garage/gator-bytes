using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.DAL.Models;

namespace ResumeJobAnalysisTool.Shared.Interfaces
{
    public interface IRAGManager
    {
        public Task UploadJobPostingToRAG(JobPostingFileSystemUploadEntry uploadEntry);

        public Task UploadResumeToRAG(ResumeFileSystemUploadEntry uploadEntry);

        public Task UploadMatchToRAG(MatchAnalysisResult uploadEntry);
    }
}
