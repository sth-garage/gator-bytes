using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GatorBytes.Shared.Models
{


    public class SemanticKernelBuilderResult
    {
        public AIServices AIServices { get; set; } = new AIServices();
    }


    public class AIServices
    {
        public IChatCompletionService ChatCompletionService { get; set; }

        public Kernel Kernel { get; set; }
    }

}
