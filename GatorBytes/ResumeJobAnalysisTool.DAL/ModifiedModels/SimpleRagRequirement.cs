using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeJobAnalysisTool.DAL.ModifiedModels
{
    public class SimpleRagRequirement
    {
        public string Name { get; set; }

        public string Justification { get; set; }

        public int DesiredLevel { get; set; }

        public int MinimumLevel { get; set; }
    }
}
