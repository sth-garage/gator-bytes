using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class MatchAnalysisResult
{
    public int Id { get; set; }

    public decimal GeneralMatchPercentage { get; set; }

    public string GeneralMatchSummary { get; set; } = null!;

    public string GeneralMatchDetails { get; set; } = null!;

    public decimal SkillMatchPercentage { get; set; }

    public string SkillMatchSummary { get; set; } = null!;

    public string SkillMatchDetails { get; set; } = null!;

    public decimal OverallMatchPercentage { get; set; }

    public string OverallMatchSummary { get; set; } = null!;

    public string OverallMatchDetails { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? Html { get; set; }

    public int JobPostingId { get; set; }

    public int ResumeId { get; set; }

    public bool IsActive { get; set; }

    public virtual JobPosting JobPosting { get; set; } = null!;

    public virtual Resume Resume { get; set; } = null!;
}
