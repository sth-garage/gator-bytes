USE [ResumeJobAnalysisTool]
GO
/****** Object:  Table [dbo].[DocumentUploads]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentUploads](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[FileCreatedDate] [datetime] NULL,
	[FileModifiedDate] [datetime] NULL,
	[Base64Data] [varchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[FileType] [varchar](50) NOT NULL,
	[HasBeenProcessed] [bit] NOT NULL,
 CONSTRAINT [PK_Uploads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobPostings]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobPostings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Position] [varchar](max) NULL,
	[CompanyName] [varchar](max) NULL,
	[Summary] [varchar](max) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Name] [varchar](max) NULL,
	[HTML] [varchar](max) NULL,
	[DocumentUploadId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_JobPostings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobSkills]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobSkills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobPostingId] [int] NOT NULL,
	[SkillId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MinimumLevel] [int] NULL,
	[DesiredLevel] [int] NULL,
	[Justification] [varchar](max) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_JobRequirement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MatchAnalysisResults]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchAnalysisResults](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GeneralMatchPercentage] [decimal](5, 2) NOT NULL,
	[GeneralMatchSummary] [varchar](max) NOT NULL,
	[GeneralMatchDetails] [varchar](max) NOT NULL,
	[SkillMatchPercentage] [decimal](5, 2) NOT NULL,
	[SkillMatchSummary] [varchar](max) NOT NULL,
	[SkillMatchDetails] [varchar](max) NOT NULL,
	[OverallMatchPercentage] [decimal](5, 2) NOT NULL,
	[OverallMatchSummary] [varchar](max) NOT NULL,
	[OverallMatchDetails] [varchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[HTML] [varchar](max) NULL,
	[JobPostingId] [int] NOT NULL,
	[ResumeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ChatAnalysisResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prompts]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prompts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PromptTypeId] [int] NOT NULL,
	[PromptText] [varchar](max) NULL,
	[CreatedOn] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Prompts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromptTypes]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromptTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PromptTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resumes]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resumes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Personality] [varchar](max) NULL,
	[Summary] [varchar](max) NULL,
	[HTML] [varchar](max) NULL,
	[DocumentUploadId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Resume] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeSkills]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumeSkills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ResumeId] [int] NOT NULL,
	[SkillId] [int] NOT NULL,
	[SkillLevel] [int] NOT NULL,
	[Justification] [varchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ResumeSkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Skills]    Script Date: 12/7/2025 10:23:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SkillName] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Skills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_Uploads_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_DocumentUploads_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_DocumentUploads_FileType]  DEFAULT ('none') FOR [FileType]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_DocumentUploads_HasBeenProcessed]  DEFAULT ((0)) FOR [HasBeenProcessed]
GO
ALTER TABLE [dbo].[JobPostings] ADD  CONSTRAINT [DF_JobPostings_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[JobPostings] ADD  CONSTRAINT [DF_JobPostings_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[JobSkills] ADD  CONSTRAINT [DF_JobSkills_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[JobSkills] ADD  CONSTRAINT [DF_JobSkills_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[MatchAnalysisResults] ADD  CONSTRAINT [DF_MatchAnalysisResults_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Prompts] ADD  CONSTRAINT [DF_Prompts_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PromptTypes] ADD  CONSTRAINT [DF_PromptTypes_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PromptTypes] ADD  CONSTRAINT [DF_PromptTypes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Resumes] ADD  CONSTRAINT [DF_Resume_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Resumes] ADD  CONSTRAINT [DF_Resumes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ResumeSkills] ADD  CONSTRAINT [DF_ResumeSkills_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ResumeSkills] ADD  CONSTRAINT [DF_ResumeSkills_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Skills] ADD  CONSTRAINT [DF_Skills_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[JobPostings]  WITH CHECK ADD  CONSTRAINT [FK_JobPostings_DocumentUploads] FOREIGN KEY([DocumentUploadId])
REFERENCES [dbo].[DocumentUploads] ([Id])
GO
ALTER TABLE [dbo].[JobPostings] CHECK CONSTRAINT [FK_JobPostings_DocumentUploads]
GO
ALTER TABLE [dbo].[JobSkills]  WITH CHECK ADD  CONSTRAINT [FK_JobSkills_JobPostings] FOREIGN KEY([JobPostingId])
REFERENCES [dbo].[JobPostings] ([Id])
GO
ALTER TABLE [dbo].[JobSkills] CHECK CONSTRAINT [FK_JobSkills_JobPostings]
GO
ALTER TABLE [dbo].[JobSkills]  WITH CHECK ADD  CONSTRAINT [FK_JobSkills_Skills] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skills] ([Id])
GO
ALTER TABLE [dbo].[JobSkills] CHECK CONSTRAINT [FK_JobSkills_Skills]
GO
ALTER TABLE [dbo].[MatchAnalysisResults]  WITH CHECK ADD  CONSTRAINT [FK_MatchAnalysisResults_JobPostings] FOREIGN KEY([JobPostingId])
REFERENCES [dbo].[JobPostings] ([Id])
GO
ALTER TABLE [dbo].[MatchAnalysisResults] CHECK CONSTRAINT [FK_MatchAnalysisResults_JobPostings]
GO
ALTER TABLE [dbo].[MatchAnalysisResults]  WITH CHECK ADD  CONSTRAINT [FK_MatchAnalysisResults_Resumes] FOREIGN KEY([ResumeId])
REFERENCES [dbo].[Resumes] ([Id])
GO
ALTER TABLE [dbo].[MatchAnalysisResults] CHECK CONSTRAINT [FK_MatchAnalysisResults_Resumes]
GO
ALTER TABLE [dbo].[Resumes]  WITH CHECK ADD  CONSTRAINT [FK_Resumes_DocumentUploads] FOREIGN KEY([DocumentUploadId])
REFERENCES [dbo].[DocumentUploads] ([Id])
GO
ALTER TABLE [dbo].[Resumes] CHECK CONSTRAINT [FK_Resumes_DocumentUploads]
GO
ALTER TABLE [dbo].[ResumeSkills]  WITH CHECK ADD  CONSTRAINT [FK_ResumeSkills_Resumes] FOREIGN KEY([ResumeId])
REFERENCES [dbo].[Resumes] ([Id])
GO
ALTER TABLE [dbo].[ResumeSkills] CHECK CONSTRAINT [FK_ResumeSkills_Resumes]
GO
ALTER TABLE [dbo].[ResumeSkills]  WITH CHECK ADD  CONSTRAINT [FK_ResumeSkills_Skills] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skills] ([Id])
GO
ALTER TABLE [dbo].[ResumeSkills] CHECK CONSTRAINT [FK_ResumeSkills_Skills]
GO
