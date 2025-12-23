using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string SkillName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public virtual ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
}
