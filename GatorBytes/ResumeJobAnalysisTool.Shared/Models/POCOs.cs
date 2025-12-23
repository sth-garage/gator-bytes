
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using System.ComponentModel.DataAnnotations;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;

namespace ResumeJobAnalysisTool.Shared.Models
{

    public class AgentFromWeb
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool FinalReviewer { get; set; }
    }

    #region For Quick Tests

    public class FinanceInfo
    {
        [VectorStoreKey]
        public Guid Key { get; set; } = Guid.NewGuid();

        [VectorStoreData]
        public string Text { get; set; } = string.Empty;

        // Note that the vector property is typed as a string, and
        // its value is derived from the Text property. The string
        // value will however be converted to a vector on upsert and
        // stored in the database as a vector.
        [VectorStoreVector(3072)]
        public string Embedding => this.Text;
    }

    public class SKQuickTestOptions
    {
        public Kernel Kernel { get; set; }

        public bool ShouldTestImage { get; set; } = false;

        public bool ShouldTestTextToAudio { get; set; } = false;

        public bool ShouldTestLocalRAG { get; set; } = false;

        public bool ShouldTestRAGSearch { get; set; } = false;

        public bool ShouldTestRAGUploadAndSearch { get; set; } = false;

        public bool ShouldAddTestRAGPlugin { get; set; } = false;

        public bool ShouldUploadToRag { get; set; } = false;

    }

    #endregion

    public class SemanticKernelBuilderResult
    {
        public AIServices AIServices { get; set; } = new AIServices();
    }


    public class AIServices
    {
        public IChatCompletionService ChatCompletionService { get; set; }

        public QdrantVectorStore QdrantVectorStore { get; set; }

        public Kernel Kernel { get; set; }

        public IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator { get; set; }
    }

    public class ResumeFileSystemUploadEntry : FileSystemUploadEntry
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Personality { get; set; }

        public int ResumeDBId { get; set; }

        
    }

    public class MatchUploadEntry
    {
        public int ResumeId { get; set; }

        public int JobPostingId { get; set; }

        public string Summary { get; set; }

        public string Justification { get; set; }

        public double MatchScore { get; set; }
    }

    public class JobPostingFileSystemUploadEntry : FileSystemUploadEntry
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Position { get; set; }

        public string Summary { get; set; }

        public int JobPostingDBId { get; set; }
    }

    public abstract class FileSystemUploadEntry
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public string HTML { get; set; }

        public string Text { get; set; }

        public string Markdown { get; set; }

        public string JSONSkills { get; set; }
    }

    public class ResumeResult
    {
        //public string Base64Input { get; set; }

        public string Text { get; set; }

        public string Markdown { get; set; }

        public string Skills { get; set; }

        public string HTML { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Personality { get; set; }

        public resumeSkillsJSON SkillsList { get; set; }
    }


    public class JobPostingResult
    {
        //public string Base64Input { get; set; }

        public string Text { get; set; }

        public string Markdown { get; set; }

        public string Skills { get; set; }

        public string HTML { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Position { get; set; }

        public string CompanyName { get; set; }

        public string Culture { get; set; }

        public string Benefits { get; set; }

        public string PostingDate { get; set; }

        public jobSkillsJSON SkillsList { get; set; }
    }

    public class SimpleOutput
    {
        public string justification { get; set; }

        public string summary { get; set; }

        public double overallMatchPercentage { get; set; }
    }

    public class MatchOutput
    {
        public SimpleOutput OverallMatch { get; set; }

        public SimpleOutput SkillMatch { get; set; }

        public SimpleOutput CombinedMatch { get; set; }
    }

    public class SimpleResumeDTO
    {
        public int Id { get; set; }

        public List<string> Skills { get; set; } = new List<string>();

        public string Name { get; set; }
    }

    public class SimpleMatchDTO
    {
        public int Id { get; set; }

        public double OverallScore { get; set; }

        public double GeneralScore { get; set; }

        public double TechnicalScore { get; set; }

        public string Summary {  get; set; }
    }

    public class SimpleJobDTO
    {
        public int Id { get; set; }

        public List<string> Skills { get; set; } = new List<string>();

        public string Name { get; set; }
    }



    /// <summary>
    /// Sample model class that represents a glossary entry.
    /// </summary>
    /// <remarks>
    /// Note that each property is decorated with an attribute that specifies how the property should be treated by the vector store.
    /// This allows us to create a collection in the vector store and upsert and retrieve instances of this class without any further configuration.
    /// </remarks>
    /// <typeparam name="TKey">The type of the model key.</typeparam>
    public sealed class RAG_Resume<TKey>
    {

        [VectorStoreKey]
        public TKey Key { get; set; }

        [VectorStoreData (IsIndexed = true)]
        public string Category { get; set; }

        [VectorStoreData]
        public string Data { get; set; }

        [VectorStoreData]
        public string Name { get; set; }

        [VectorStoreData]
        public string FileName { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public int EntityId { get; set; }

        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float> DataEmbedding { get; set; }
    }

    public sealed class RAG_JobPosting<TKey>
    {
        [VectorStoreKey]
        public TKey Key { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string Category { get; set; }

        [VectorStoreData]
        public string Data { get; set; }

        [VectorStoreData]
        public string Name { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string Company { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string Position { get; set; }

        [VectorStoreData]
        public string FileName { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public int EntityId { get; set; }

        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float> DataEmbedding { get; set; }
    }

    public sealed class RAG_ResumeJobMatch<TKey>
    {
        [VectorStoreKey]
        public TKey Key { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string Category { get; set; }

        [VectorStoreData]
        public string Data { get; set; }

        [VectorStoreData]
        public string Summary { get; set; }



        [VectorStoreData]
        public double MatchPercentage { get; set; }

        [VectorStoreData]
        public int JobPostingDBId { get; set; }

        [VectorStoreData]
        public int ResumeDBId { get; set; }

        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float> DataEmbedding { get; set; }
    }





}
