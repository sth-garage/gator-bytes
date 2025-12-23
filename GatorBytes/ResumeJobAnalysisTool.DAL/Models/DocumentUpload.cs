using System;
using System.Collections.Generic;

namespace ResumeJobAnalysisTool.DAL.Models;

public partial class DocumentUpload
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? FileCreatedDate { get; set; }

    public DateTime? FileModifiedDate { get; set; }

    public string? Base64Data { get; set; }

    public bool IsActive { get; set; }

    public string FileType { get; set; } = null!;

    public bool HasBeenProcessed { get; set; }

    public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();

    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
