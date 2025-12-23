using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.DAL.ModifiedModels;
//using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Metadata;

namespace ResumeJobAnalysisTool.Shared.Models
{

    public class ResumeAnalysis
    {
        public string text { get; set;  }

        public string markdown { get; set;  }

        public string candidateName { get; set; }

        public string candidateSummary { get; set; }

        public string candidatePersonality { get; set; }
    }


    public class JobPostingAnalysis
    {
        public string text { get; set; }

        public string markdown { get; set; }

        public string companyName { get; set; }

        public string position { get; set; }

        public string jobSummary { get; set; }

        public string culture { get; set; }

        public string jobId { get; set; }

        public string postingDate { get; set; }

        public string benefits { get; set; }
    }




    public class resumeSkillsJSON
    {
        public List<resumeSkillJSON> skills { get; set; } = new List<resumeSkillJSON>();
    }

    public class resumeSkillJSON
    {
        public string name { get; set; }
        public int level { get; set; }
        public string justification { get; set; }

        public double? lastUsed { get; set; }

        public List<string> usedWith { get; set; } = new List<string>();
    }

    public class jobSkillsJSON
    {
        public List<jobSkillJSON> skills { get; set; } = new List<jobSkillJSON>();
    }


    public class jobSkillJSON
    {
        public string name { get; set; }
        public int minimumLevel { get; set; }

        public int desiredLevel { get; set; }

        public string justification { get; set; }

    }

    //public class PDFtoHTMLResumeResult
    //{
    //    public string HTML { get; set; }

    //    public string Name { get; set; }

    //    public string Summary { get; set; }

    //    public string Personality { get; set; }

    //    public List<SimpleRagSkill> Skills { get; set; } = new List<SimpleRagSkill>();
    //}

    //public class PDFtoHTMLJobPostingResult
    //{
    //    public string HTML { get; set; }

    //    public string CompanyName { get; set; }

    //    public string Position { get; set; }

    //    public string Name { get; set; }

    //    public string Summary { get; set; }

    //    public List<SimpleRagRequirement> Requirements { get; set; } = new List<SimpleRagRequirement>();

    //    public List<string> SoftSkills { get; set; } = new List<string>();
    //}

    public class AgentAnalysisResult
    {
        public string agentName { get; set; }

        public int messageCount { get; set; }

        public double matchPercentage { get; set; }

        public string justification { get; set; }

        public double matchPercentageAdjustment { get; set; }

        public string matchPercenageAdjustmentJustification { get; set;  }

        public string summary { get; set; }
    }

    public class  FinalIndividualAgentAnalysisResult 
    {
        public string name { get; set; }
        public double matchPercentage { get; set; }

        public string justification { get; set; }

        public double matchDifference { get; set; }

        public string matchDifferenceJustification { get; set; }
    }

    public class FinalAgentAnalysisResult
    {
        public double matchPercentage { get; set; }

        public string justification { get; set; }

        public string html { get; set; }

        public List<FinalIndividualAgentAnalysisResult> AgentResults { get; set; } = new List<FinalIndividualAgentAnalysisResult>();
    }

    public class ChatCompletionAgentEnhanced
    {
        public ChatCompletionAgent ChatCompletionAgent { get; set; }

        public int AgentId { get; set; }
    }

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110
#pragma warning disable OPENAI001
    public class AgentGroupChatEnhanced
    {

        public AgentGroupChat AgentGroupChat { get; set; } = new AgentGroupChat();

        public List<ChatCompletionAgentEnhanced> EnhancedChatCompletionAgents { get; set; } = new List<ChatCompletionAgentEnhanced>();
    }

    public class CompleteAgentChatResult
    {
        public List<AnalysisAgentResult> AgentResults { get; set; } = new List<AnalysisAgentResult>();

        public string FinalOutput { get; set; }

        public ChatMessageContent FinalChatMessage {get; set; }

        public Dictionary<int, string> FinalApproverMessages { get; set; } = new Dictionary<int, string>();
    }


}
