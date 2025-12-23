// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Text;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared.Models;

namespace ResumeJobAnalysisTool.Shared.Interfaces;

public interface IVectorProcessor
{
    Task IngestResumeDataAsync<TKey>(string collection, string category, string text, string name, Func<TKey> uniqueKeyGenerator, string fileName, int resumeDBId)
    where TKey : notnull;


    Task IngestMatchAnalysisDataAsync<TKey>(string collectionName, string category, string data, string summary, double matchPercentage, int resumeId, int jobPostingId,
    Func<TKey> uniqueKeyGenerator);

    Task IngestMatchAnalysisHTMLDataAsync<TKey>(string collectionName, string category, string data, double overallMatchPercentage, int resumeId, int jobPostingId,
    Func<TKey> uniqueKeyGenerator);

}
