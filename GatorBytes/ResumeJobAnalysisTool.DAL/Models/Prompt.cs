using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class Prompt
{
    public int Id { get; set; }

    public int PromptTypeId { get; set; }

    public string? PromptText { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool IsActive { get; set; }
}
