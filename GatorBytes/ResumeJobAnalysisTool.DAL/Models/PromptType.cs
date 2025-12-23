using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class PromptType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }
}
