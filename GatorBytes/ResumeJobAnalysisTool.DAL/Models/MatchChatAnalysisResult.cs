using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class MatchChatAnalysisResult
{
    public int Id { get; set; }

    public decimal OverallMatchPercentage { get; set; }

    public string MatchDescriptionSummary { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? Html { get; set; }

    public int JobPostingId { get; set; }

    public int ResumeId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AnalysisAgentResult> AnalysisAgentResults { get; set; } = new List<AnalysisAgentResult>();

    public virtual JobPosting JobPosting { get; set; } = null!;

    public virtual Resume Resume { get; set; } = null!;
}
