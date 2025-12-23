using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class JobRequirement
{
    public int Id { get; set; }

    public int JobPostingId { get; set; }

    public int RequirementId { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? MinimumLevel { get; set; }

    public int? DesiredLevel { get; set; }

    public string? Justification { get; set; }

    public bool IsActive { get; set; }

    public virtual JobPosting JobPosting { get; set; } = null!;

    public virtual Requirement Requirement { get; set; } = null!;
}
