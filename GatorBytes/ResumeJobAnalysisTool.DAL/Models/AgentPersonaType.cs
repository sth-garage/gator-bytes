using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class AgentPersonaType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<AgentPersona> AgentPersonas { get; set; } = new List<AgentPersona>();
}
