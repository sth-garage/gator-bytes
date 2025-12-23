using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class ResumeSkill
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public int ResumeId { get; set; }

    public int SkillId { get; set; }

    public int SkillLevel { get; set; }

    public string Justification { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Resume Resume { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
