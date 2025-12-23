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
    public class MatchController : ControllerBase
    {
        private HRContext _context;
        private ConversionManager _conversionManager = new ConversionManager();

        public MatchController(HRContext context)
        {
            _context = context;
        }



        [HttpGet("GetSimpleMatch")]
        public async Task<SimpleMatchDTO> GetSimpleMatch(int resumeId, int jobId)
        {
            SimpleMatchDTO result = new SimpleMatchDTO();

            var match = await _context.MatchAnalysisResults.FirstOrDefaultAsync(x => x.ResumeId == resumeId && x.JobPostingId == jobId);

            if (match != null)
            {


                result = _conversionManager.GetSimpleMatch(match);
            }

            return result;
        }

        public class IdForMatchHolder
        {
            public int resumeId { get; set; }
            public int jobId { get; set; }

        }

        [HttpGet("GetMatchAnalysis")]
        public async Task<string> GetMatchAnalysis(int resumeId, int jobId)
        {
            var result = "";

            var match = await _context.MatchAnalysisResults.FirstOrDefaultAsync(x => x.ResumeId == resumeId && x.JobPostingId == jobId);

            if (match != null)
            {
                result = match.Html;
                result = result.Replace("```html\n", "");
                var finalTagIndex = result.IndexOf("</html>");
                result = result.Substring(0, finalTagIndex + 7);
                var stop = 1;
            }

            

            return result;
        }






        [HttpPost("DownloadMatchAnalysis")]
        public async Task<IActionResult> DownloadMatchAnalysis([FromBody] IdForMatchHolder matchIds)
        {


            //var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == idHolder.id);
            var html = "";

            var match = await _context.MatchAnalysisResults.FirstOrDefaultAsync(x => x.ResumeId == matchIds.resumeId && x.JobPostingId == matchIds.jobId);
            var resume = await _context.Resumes.FirstOrDefaultAsync(x => x.Id == matchIds.resumeId);
            var jobPosting = await _context.JobPostings.FirstOrDefaultAsync(x => x.Id == matchIds.jobId);


            if (match != null)
            {
                html = match.Html;
                html = html.Replace("```html\n", "");
                var finalTagIndex = html.IndexOf("</html>");
                html = html.Substring(0, finalTagIndex + 7);
                var stop = 1;
            }

            var filename = jobPosting.Name + "_" + resume.Name + "_Analysis.html";

            // Convert the string to a byte array using UTF-8 encoding
            byte[] byteArray = Encoding.UTF8.GetBytes(html);

            // Initialize a MemoryStream with the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            var contentType = "application/octet-stream";

            return File(stream, contentType, filename);
        }


    }
}
