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
    public class JobController : ControllerBase
    {
        private HRContext _context;
        private ConversionManager _conversionManager = new ConversionManager();

        public JobController(HRContext context)
        {
            _context = context;
        }

        [HttpGet("GetSimpleJobs")]
        public async Task<List<SimpleJobDTO>> GetSimpleJobs()
        {
            List<SimpleJobDTO> result = new List<SimpleJobDTO>();

            var jobs = _context.JobPostings.Include(x => x.JobSkills).ThenInclude(y => y.Skill);

            foreach (var job in jobs)
            {
                result.Add(_conversionManager.GetSimpleJobs(job));
            }

            return result;
        }


        [HttpGet("GetJobAnalysis")]
        public async Task<string> GetJobAnalysis(int id)
        {
            var result = "";

            var jobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Id == id);

            if (jobPosting != null)
            {
                result = jobPosting.Html;
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

        [HttpPost("DownloadOriginalJob")]
        public async Task<IActionResult> DownloadOriginalJob([FromBody] IdHolder idHolder)
        {

            var jobs = _context.JobPostings.Include(x => x.DocumentUpload);
            var documentUploadResume = jobs.FirstOrDefault(x => x.Id == idHolder.id);
            var documentUpload = documentUploadResume.DocumentUpload;

            var filename = documentUpload.FileName;

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Convert.FromBase64String(documentUpload.Base64Data);

            // Initialize a MemoryStream with the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            var contentType = "application/octet-stream";

            return File(stream, contentType, filename);
        }


        [HttpPost("DownloadJobAnalysis")]
        public async Task<IActionResult> DownloadJobAnalysis([FromBody] IdHolder idHolder)
        {


            var jobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Id == idHolder.id);
            var html = "";

            if (jobPosting != null)
            {
                html = jobPosting.Html;
                html = html.Replace("```html\n", "");
                var finalTagIndex = html.IndexOf("</html>");
                html = html.Substring(0, finalTagIndex + 7);
                var stop = 1;
            }

            var filename = jobPosting.Name + "_Analysis.html";

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Encoding.UTF8.GetBytes(html);

            // Initialize a MemoryStream with the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            var contentType = "application/octet-stream";

            return File(stream, contentType, filename);
        }


    }
}
