// FilesController.cs
using HtmlRendererCore.Core;
using HtmlRendererCore.Core.Entities;
using HtmlRendererCore.PdfSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using ResumeJobAnalysisTool.AppLogic;
using ResumeJobAnalysisTool.AppLogic.Uploading;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.DAL.Models;
using ResumeJobAnalysisTool.Shared;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.Shared.Utility;
using ResumeJobAnalysisTool.SK.RAG;
using System;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static Microsoft.KernelMemory.DocumentUploadRequest;
using static System.Net.Mime.MediaTypeNames;

namespace ResumeJobAnalysisTool.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    readonly ConfigurationValues _configValues;
    readonly IChatCompletionService _chatCompletionService;
    readonly Kernel _kernel;
    readonly UploadManagerBase<ResumeFileSystemUploadEntry, Resume> _uploadResumeManager;
    readonly UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting> _uploadJobPostingManager;
    readonly PromptManager _promptManager;
    readonly HRContext _context;
    readonly IMatchAnalysisManager _matchAnalysisManager;
    readonly IVectorProcessor _vectorProcessor;


    public FileController(ConfigurationValues configValues,
    Kernel kernel,
    IChatCompletionService chatCompletionService,
    UploadManagerBase<ResumeFileSystemUploadEntry, Resume> uploadResumeManager,
    UploadManagerBase<JobPostingFileSystemUploadEntry, JobPosting> uploadJobPostingManager,
    HRContext context,
    IVectorProcessor vectorProcessor,
    IMatchAnalysisManager matchAnalysisManager
    )
    {
        _configValues = configValues;
        _chatCompletionService = chatCompletionService;
        _kernel = kernel;
        _uploadResumeManager = uploadResumeManager;
        _uploadJobPostingManager = uploadJobPostingManager;
        _uploadResumeManager = uploadResumeManager;
        _context = context;
        _promptManager = new PromptManager(_context);
        _vectorProcessor = vectorProcessor;
        _matchAnalysisManager = matchAnalysisManager;
    }

    public class gatorChat
    {
        public string data { get; set; }

        public string style { get; set; }

        public bool saveAll { get; set; }
    }

    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] gatorChat product)
    {
        var filename = (product.saveAll ? "full_chat_" : "message_chat_") + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html";
        string full = "";

        product.style = product.style.Replace("overflow: hidden", "")

                .Replace("height: 100%", "");


        if (product.saveAll)
        {
            

            product.data = product.data.Replace("class=\"chat-input-row\"", "style=\"display:none\"");

            full = "<html>" + product.style + "<div class=\"main-container\"><div class=\"app-content\">" + product.data + "</div></div></html>";
        }
        else 
        {
            full = "<html>" + product.style + "<div class=\"main-container\"><div class=\"app-content\">" + product.data + "</div></div></html>";
        }

        // Convert the string to a byte array using UTF-8 encoding
        byte[] byteArray = Encoding.UTF8.GetBytes(full);

        // Initialize a MemoryStream with the byte array
        MemoryStream stream = new MemoryStream(byteArray);

        var contentType = "application/octet-stream";

        return File(stream, contentType, filename);
    }

    private async Task<string> GetBase64DataFromFile(IFormFile file)
    {
        var bytes = await GetBytesFromIFormFileAsync(file);

        return Convert.ToBase64String(bytes);
    }

    private async Task<byte[]> GetBytesFromIFormFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(UploadTest input)

    {
        //if (file == null || file.Length == 0)
        //{
        //    return BadRequest("No file uploaded.");
        //}

        // Define the path where the file will be saved
        //var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        //if (!Directory.Exists(uploadsFolder))
        //{
        //    Directory.CreateDirectory(uploadsFolder);
        //}

        //var filePath = Path.Combine(uploadsFolder, file.FileName);

        //var uploadedFile = await _uploadManager.GetResumeUploadFile(filePath, file);

        // return Ok(new { Message = uploadedFile.Message, FileName = file.FileName });
        //return Ok(new { Message = "test", FileName = file.FileName });

        

        var returnMsg = "";

        var existing = _context.DocumentUploads.FirstOrDefault(x => x.IsActive && x.FileName == input.file.FileName);

        if (existing == null)
        {


            if (input != null
                && input.file != null
                && input.file.Length != 0
                && !String.IsNullOrEmpty(input.uploadType))
            {
                var base64Data = await GetBase64DataFromFile(input.file);

                DocumentUpload documentUpload = new DocumentUpload
                {
                    Base64Data = base64Data,
                    FileCreatedDate = DateTime.Now,
                    FileModifiedDate = null,
                    FileName = input.file.FileName,
                    FileType = input.uploadType,
                    CreatedOn = DateTime.Now,
                    HasBeenProcessed = false,
                    IsActive = true
                };

                _context.DocumentUploads.Add(documentUpload);
                await _context.SaveChangesAsync();
                returnMsg = input.file.FileName + " was successfully uploaded";
            }
            else
            {
                returnMsg = "There was a problem with the input file or type provided";
            }
        }
        else
        {
            returnMsg = input.file.FileName + " has already been added";
        }

            return Ok(returnMsg);

    }

    public class UploadTest
    {
        public IFormFile file { get; set; }
        public string uploadType { get; set; }
    }


    [HttpPost("processFiles")]
    public async Task<IActionResult> ProcessFiles()
    {

        // TEMP
        Console.WriteLine("START");
        //await _uploadResumeManager.ProcessEntries();
        //await _context.SaveChangesAsync();
        //await _uploadJobPostingManager.ProcessEntries();
        //await _context.SaveChangesAsync();
        //Thread.Sleep(1000);
        //await _uploadResumeManager.ProcessEntries(resumeFilePath);
        //await _uploadJobPostingManager.ProcessEntries(jobPostingFilePath);
        //Thread.Sleep(1000);
        ////await _uploadResumeManager.ProcessEntries(resumeFilePath);
        ////await _uploadJobPostingManager.ProcessEntries(jobPostingFilePath);
        //await _matchAnalysisManager.MakeMatches();
        //Thread.Sleep(5000);
        //await _matchAnalysisManager.MakeMatches();
        //Thread.Sleep(5000);
        //await _matchAnalysisManager.MakeMatches();
        //Thread.Sleep(5000);




        return Ok();

    }

}