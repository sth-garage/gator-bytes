using Microsoft.AspNetCore.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.DAL.ModifiedModels;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Prompts;
using static ResumeJobAnalysisTool.Shared.Enums;

namespace ResumeJobAnalysisTool.AppLogic.Uploading
{
    public abstract class UploadManagerBase<UploadType, BaseEntityType>
        where UploadType : FileSystemUploadEntry
        where BaseEntityType : class
    {
        protected Kernel _kernel = null;
        protected ConfigurationValues _configValues = null;
        protected IChatCompletionService _chatCompletionService = null;
        protected HRContext _context = null;
        protected IFileAnalysisManager _fileAnalysisManager = null;
        protected IVectorProcessor _vectorResume = null;
        protected IRAGManager _ragManager = null;

        public UploadManagerBase(Kernel kernel,
            ConfigurationValues configValues,
            IChatCompletionService chatCompletionService,
            HRContext context,
            IFileAnalysisManager fileAnalysisManager,
            IVectorProcessor vectorResumeor,
            IRAGManager ragManager)
        {
            _kernel = kernel;
            _configValues = configValues;
            _chatCompletionService = chatCompletionService;
            _context = context;
            _fileAnalysisManager = fileAnalysisManager;
            _vectorResume = vectorResumeor;
            _ragManager = ragManager;
        }

        public virtual async Task<BaseEntityType> GetOrCreateBaseEntity(string name, BaseEntityType newEntityIfNeeded)
        {
            return null;
        }

        /// <summary>
        /// Cycles through each file and Resumees the resume
        /// </summary>
        /// <param name="entryFilePath"></param>
        /// <returns></returns>
        public async Task ProcessEntries()
        {
            


            try
            {
                var filesToProcess = _context.DocumentUploads.Where(x => x.IsActive && !x.HasBeenProcessed);
                var resumeFiles = filesToProcess.Where(x => x.FileType == "resume").ToList();
                var jobPostingFiles = filesToProcess.Where(x => x.FileType == "jobPosting").ToList();

                foreach (var resume in resumeFiles)
                {
                    await ProcessResumeEntry(resume);
                }

                foreach (var jobPosting in jobPostingFiles)
                {
                    await ProcessJobPostingEntry(jobPosting);
                }

                //for (int i = 0; i < resumeFiles.Length; i++)
                //    {
                //        file = resumeFiles[i];

                //        var alreadyBeenProcesssed = await HasFileAlreadyBeenProcessedAsync(file.Name);
                //        Console.WriteLine(String.Format("File [{0}] of [{1}] - {2}", i + 1, resumeFiles.Length, file.Name));
                //        try
                //        {
                //            if (alreadyBeenProcesssed)
                //            {
                //                Console.WriteLine(String.Format("File SKIPPED [{0}] of [{1}] - {2} - Has Already Been Processed", i + 1, resumeFiles.Length, file.Name));

                //            }
                //            else
                //            {

                //                await ProcessEntry(file);
                //                Console.WriteLine(String.Format("File COMPLETE [{0}] of [{1}] - {2}", i + 1, resumeFiles.Length, file.Name));
                //                Console.WriteLine("---");
                //                Console.WriteLine("");
                //                Console.WriteLine("");
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            var stop = 1;
                //            continue;
                //        }
                //    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<DocumentUpload> CreateDocumentUploadEntry(FileInfo file)
        //{
        //    var fileBytes = File.ReadAllBytes(file.FullName);
        //    var base64Data = Convert.ToBase64String(fileBytes);

        //    DocumentUpload documentUpload = new DocumentUpload
        //    {
        //        FileName = file.Name,
        //        CreatedOn = DateTime.Now,
        //        //FileType = (int)FileTypes.PDF,
        //        //AppIdentifier = ragGuid,
        //        FileModifiedDate = file.LastWriteTime,
        //        FileCreatedDate = file.CreationTime,
        //        Base64Data = base64Data,
        //        //FileExtension = file.Extension,
        //        IsActive = true
        //    };

        //    var document = (await _context.DocumentUploads.AddAsync(documentUpload)).Entity;

        //    return documentUpload;
        //}

        public async Task<bool> HasFileAlreadyBeenProcessedAsync(string fileName)
        {
            var result = false;

            var match = _context.DocumentUploads.FirstOrDefault(x => x.FileName == fileName && x.HasBeenProcessed == true);
            result = match != null;


            return result;
        }

        public virtual async Task ProcessResumeEntry(DocumentUpload fileInfo)
        {

        }

        public virtual async Task ProcessJobPostingEntry(DocumentUpload fileInfo)
        {

        }
    }
}
