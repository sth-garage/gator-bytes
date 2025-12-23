using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class JobPosting
{
    public int Id { get; set; }

    public string? Position { get; set; }

    public string? CompanyName { get; set; }

    public string? Summary { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? Name { get; set; }

    public string? Html { get; set; }

    public int DocumentUploadId { get; set; }

    public bool IsActive { get; set; }

    public virtual DocumentUpload DocumentUpload { get; set; } = null!;

    public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public virtual ICollection<MatchAnalysisResult> MatchAnalysisResults { get; set; } = new List<MatchAnalysisResult>();
}
