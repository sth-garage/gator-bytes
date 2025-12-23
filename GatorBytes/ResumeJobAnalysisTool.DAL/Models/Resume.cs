using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class Resume
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Name { get; set; } = null!;

    public string? Personality { get; set; }

    public string? Summary { get; set; }

    public string? Html { get; set; }

    public int DocumentUploadId { get; set; }

    public bool IsActive { get; set; }

    public virtual DocumentUpload DocumentUpload { get; set; } = null!;

    public virtual ICollection<MatchAnalysisResult> MatchAnalysisResults { get; set; } = new List<MatchAnalysisResult>();

    public virtual ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
}
