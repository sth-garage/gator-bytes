using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeJobAnalysisTool.AppLogic;
using ResumeJobAnalysisTool.DAL.Context;
using ResumeJobAnalysisTool.Shared.Models;
using System.Text;
using static ResumeJobAnalysisTool.Web.Controllers.FileController;

namespace ResumeJobAnalysisTool.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private HRContext _context;
        private ConversionManager _conversionManager = new ConversionManager();

        public ResumeController(HRContext context)
        {
            _context = context;
        }

        [HttpGet("GetSimpleResumes")]
        public async Task<List<SimpleResumeDTO>> GetSimpleResumes()
        {
            List<SimpleResumeDTO> result = new List<SimpleResumeDTO>();

            var resumes = _context.Resumes.Include(x => x.ResumeSkills).ThenInclude(y => y.Skill);

            foreach (var resume in resumes)
            {
                result.Add(_conversionManager.GetSimpleResume(resume));
            }

            return result;
        }


        [HttpGet("GetResumeAnalysis")]
        public async Task<string> GetResumeAnalysis(int id)
        {
            var result = "";

            var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == id);

            if (resume != null)
            {
                result = resume.Html;
                result = result.Replace("```html\n", "");
                var finalTagIndex = result.IndexOf("</html>");
                result = result.Substring(0, finalTagIndex + 7);
                var stop = 1;
            }

            

            return result;
        }

        public class IdHolder
        {
            public int id { get; set; }
        }

        [HttpPost("DownloadOriginalResume")]
        public async Task<IActionResult> DownloadOriginalResume([FromBody] IdHolder idHolder)
        {

            var resumes = _context.Resumes.Include(x => x.DocumentUpload);
            var documentUploadResume = resumes.FirstOrDefault(x => x.Id == idHolder.id);
            var documentUpload = documentUploadResume.DocumentUpload;

            var filename = documentUpload.FileName;

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Convert.FromBase64String(documentUpload.Base64Data);

            // Initialize a MemoryStream with the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            var contentType = "application/octet-stream";

            return File(stream, contentType, filename);
        }


        [HttpPost("DownloadResumeAnalysis")]
        public async Task<IActionResult> DownloadResumeAnalysis([FromBody] IdHolder idHolder)
        {


            var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == idHolder.id);
            var html = "";

            if (resume != null)
            {
                html = resume.Html;
                html = html.Replace("```html\n", "");
                var finalTagIndex = html.IndexOf("</html>");
                html = html.Substring(0, finalTagIndex + 7);
                var stop = 1;
            }

            var filename = resume.Name + "_Analysis.html";

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Encoding.UTF8.GetBytes(html);

            // Initialize a MemoryStream with the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            var contentType = "application/octet-stream";

            return File(stream, contentType, filename);
        }


    }
}
