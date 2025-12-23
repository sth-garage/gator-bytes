// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Text;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.SK.RAG;

/// <summary>
/// This class is part of an example that shows how to ingest data into a vector store and then use vector search to find related records to a given string.
/// The example shows how to write code that can be used with multiple database types.
/// This class contains the common code.
///
/// For the entry point of the example for each database, see the following classes:
/// <para><see cref="VectorStore_VectorSearch_MultiStore_AzureAISearch"/></para>
/// <para><see cref="VectorStore_VectorSearch_MultiStore_Qdrant"/></para>
/// <para><see cref="VectorStore_VectorSearch_MultiStore_Redis"/></para>
/// <para><see cref="VectorStore_VectorSearch_MultiStore_InMemory"/></para>
/// <para><see cref="VectorStore_VectorSearch_MultiStore_Postgres"/></para>
/// </summary>
/// <param name="vectorStore">The vector store to ingest data into.</param>
/// <param name="embeddingGenerator">The service to use for generating embeddings.</param>
/// <param name="output">A helper to write output to the xUnit test output stream.</param>
public class VectorProcessor(VectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IVectorProcessor
{
    /// <summary>
    /// Ingest data into a collection with the given name, and search over that data.
    /// </summary>
    /// <typeparam name="TKey">The type of key to use for database records.</typeparam>
    /// <param name="collectionName">The name of the collection to ingest the data into.</param>
    /// <param name="uniqueKeyGenerator">A function to generate unique keys for each record to upsert.</param>
    /// <returns>An async task.</returns>
    public async Task IngestResumeDataAsync<TKey>(string collectionName,
        string category,
        string text,
        string name,
        Func<TKey> uniqueKeyGenerator,
        string fileName,
        //Guid ragGuid,
        int entityId)
        where TKey : notnull
    {
        // Get and create collection if it doesn't exist.
        var collection = vectorStore.GetCollection<TKey, RAG_Resume<TKey>>(collectionName);
        
        await collection.EnsureCollectionExistsAsync();

        var glossaryEntry = new RAG_Resume<TKey>
        {
            Key = uniqueKeyGenerator(),
            Category = category,
            Data = text,
            FileName = fileName ?? "",
            Name = name,
            //RAGGuid = ragGuid.ToString(),
            EntityId = entityId,
        };

        IEnumerable<RAG_Resume<TKey>> glossaryEntries = new List<RAG_Resume<TKey>>()
        {
            glossaryEntry
        };

        var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
        {
            entry.DataEmbedding = (await embeddingGenerator.GenerateAsync(entry.Data)).Vector;
        }));
        await Task.WhenAll(tasks);

        // Upsert the glossary entries into the collection.
        await collection.UpsertAsync(glossaryEntries);
    }

    public async Task IngestJobDataAsync<TKey>(string collectionName,
       string category,
       string text,
       string name,
       Func<TKey> uniqueKeyGenerator,
       string fileName,
       //Guid ragGuid,
       int entityId,
       string company,
       string position)
       where TKey : notnull
    {
        // Get and create collection if it doesn't exist.
        var collection = vectorStore.GetCollection<TKey, RAG_JobPosting<TKey>>(collectionName);

        await collection.EnsureCollectionExistsAsync();

        var glossaryEntry = new RAG_JobPosting<TKey>
        {
            Key = uniqueKeyGenerator(),
            Category = category,
            Data = text,
            FileName = fileName ?? "",
            Name = name,
            //RAGGuid = ragGuid.ToString(),
            EntityId = entityId,
            Company = company,
            Position = position
        };

        IEnumerable<RAG_JobPosting<TKey>> glossaryEntries = new List<RAG_JobPosting<TKey>>()
        {
            glossaryEntry
        };

        var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
        {
            entry.DataEmbedding = (await embeddingGenerator.GenerateAsync(entry.Data)).Vector;
        }));
        await Task.WhenAll(tasks);

        // Upsert the glossary entries into the collection.
        await collection.UpsertAsync(glossaryEntries);
    }



    public async Task IngestMatchAnalysisHTMLDataAsync<TKey>(string collectionName, string category, string data, double overallMatchPercentage, int resumeId, int jobPostingId,
Func<TKey> uniqueKeyGenerator)
where TKey : notnull
    {
        // Get and create collection if it doesn't exist.
        var collection = vectorStore.GetCollection<TKey, RAG_ResumeJobMatch<TKey>>(collectionName);

        await collection.EnsureCollectionExistsAsync();


        var glossaryEntry = new RAG_ResumeJobMatch<TKey>
        {
            Key = uniqueKeyGenerator(),
            Category = category,
            Data = data,
            JobPostingDBId = jobPostingId,
            ResumeDBId = resumeId,
            //AppIdentifier = matchChatAnalysisResult.AppIdentifier.ToString(),
            MatchPercentage = (double)overallMatchPercentage,
            //Summary = summary,
            

        };

        IEnumerable<RAG_ResumeJobMatch<TKey>> glossaryEntries = new List<RAG_ResumeJobMatch<TKey>>()
        {
            glossaryEntry
        };

        var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
        {
            entry.DataEmbedding = (await embeddingGenerator.GenerateAsync(entry.Data)).Vector;
        }));
        await Task.WhenAll(tasks);

        // Upsert the glossary entries into the collection.
        await collection.UpsertAsync(glossaryEntries);
    }



    public async Task IngestMatchAnalysisDataAsync<TKey>(string collectionName, string category, string data, string summary, double matchPercentage, int resumeId, int jobPostingId,
    Func<TKey> uniqueKeyGenerator)
    where TKey : notnull
    {
        // Get and create collection if it doesn't exist.
        var collection = vectorStore.GetCollection<TKey, RAG_ResumeJobMatch<TKey>>(collectionName);

        await collection.EnsureCollectionExistsAsync();


        var glossaryEntry = new RAG_ResumeJobMatch<TKey>
        {
            Key = uniqueKeyGenerator(),
            Category = category,
            Data = data,
            JobPostingDBId = jobPostingId,
            ResumeDBId = resumeId,
            //AppIdentifier = matchChatAnalysisResult.AppIdentifier.ToString(),
            MatchPercentage = (double)matchPercentage,
            //AnalysisIteration = iteration,
            Summary = summary,

        };

        IEnumerable<RAG_ResumeJobMatch<TKey>> glossaryEntries = new List<RAG_ResumeJobMatch<TKey>>()
        {
            glossaryEntry
        };

        var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
        {
            entry.DataEmbedding = (await embeddingGenerator.GenerateAsync(entry.Data)).Vector;
        }));
        await Task.WhenAll(tasks);

        // Upsert the glossary entries into the collection.
        await collection.UpsertAsync(glossaryEntries);
    }


    public async Task<IAsyncEnumerable<VectorSearchResult<RAG_Resume<TKey>>>> Search<TKey>(string collectionName, Func<TKey> uniqueKeyGenerator, string text, string category = "", List<string> terms = null)
        where TKey : notnull
    {
        List<VectorSearchResult<RAG_Resume<TKey>>> result = new List<VectorSearchResult<RAG_Resume<TKey>>>();

        var collection = vectorStore.GetCollection<TKey, RAG_Resume<TKey>>(collectionName);
        await collection.EnsureCollectionExistsAsync();

        var searchVector = (await embeddingGenerator.GenerateAsync(text)).Vector;
        var resultRecords = collection.SearchAsync(searchVector, top: 20, new()
        {

        });

        return resultRecords;
    }
}
