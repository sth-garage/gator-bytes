using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ResumeJobAnalysisTool.Shared.Prompts
{
    public static partial class Prompts
    {

        public const string ResultAsRichHTMLDivRoot = @"Answers need to be in rich HTML format with a div as the root node.";

        public const string SaveAsRichHTMLThenPDF = "Whenever saving information to a PDF, use Rich HTML - include colors, tables, charts, etc. to provide more detail or clarity";

    }
}

