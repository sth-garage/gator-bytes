using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class AnalysisAgentResult
{
    public int Id { get; set; }

    public int AgentPersonaId { get; set; }

    public decimal MatchPercentage { get; set; }

    public string Summary { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? Justification { get; set; }

    public string? MatchPercentageAdjustmentJustification { get; set; }

    public int? AgentChatNumber { get; set; }

    public int? AgentChatNumberOverall { get; set; }

    public decimal? MatchPercentageAdjustment { get; set; }

    public bool? IsActive { get; set; }

    public int MatchChatAnalysisResultId { get; set; }

    public virtual AgentPersona AgentPersona { get; set; } = null!;

    public virtual MatchChatAnalysisResult MatchChatAnalysisResult { get; set; } = null!;
}
