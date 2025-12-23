using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class AgentPersona
{
    public string? Name { get; set; }

    public string? Persona { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int Id { get; set; }

    public bool? IsFinalApprover { get; set; }

    public string? FinalApproverKeyword { get; set; }

    public string? Description { get; set; }

    public bool? IsInitialAgent { get; set; }

    public virtual ICollection<AnalysisAgentResult> AnalysisAgentResults { get; set; } = new List<AnalysisAgentResult>();
}
