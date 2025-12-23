USE ResumeJobAnalysisTool
GO
/****** Object:  Table [dbo].[AgentPersonas]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgentPersonas](
	[Name] [varchar](500) NULL,
	[Persona] [varchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgentPersonaTypeId] [int] NOT NULL,
	[IsFinalApprover] [bit] NULL,
	[FinalApproverKeyword] [varchar](1000) NULL,
 CONSTRAINT [PK_AgentPersonas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AgentPersonaTypes]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgentPersonaTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_AgentTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnalysisAgentResults]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnalysisAgentResults](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgentPersonaId] [int] NOT NULL,
	[MatchPercentage] [decimal](5, 2) NOT NULL,
	[Summary] [varchar](3000) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Justification] [varchar](max) NULL,
	[MatchPercentageAdjustmentJustification] [varchar](max) NULL,
	[AgentChatNumber] [int] NULL,
	[AgentChatNumberOverall] [int] NULL,
	[AnalysisAppIdentifier] [uniqueidentifier] NULL,
	[ChatAnalysisResultId] [int] NOT NULL,
 CONSTRAINT [PK_AnalysisAgentResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentUploads]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentUploads](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileType] [int] NOT NULL,
	[FileName] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[AppIdentifier] [uniqueidentifier] NULL,
	[FileCreatedDate] [datetime] NULL,
	[FileModifiedDate] [datetime] NULL,
	[Base64Data] [varchar](max) NULL,
	[FileExtension] [varchar](20) NULL,
 CONSTRAINT [PK_Uploads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobPostings]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobPostings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Position] [varchar](2000) NULL,
	[School] [varchar](2000) NULL,
	[Summary] [varchar](5000) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[AppIdentifier] [uniqueidentifier] NULL,
	[Name] [varchar](1000) NULL,
	[HTML] [varchar](max) NULL,
	[JSON] [varchar](max) NULL,
	[DocumentUploadId] [int] NOT NULL,
 CONSTRAINT [PK_JobPostings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobRequirements]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobRequirements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobPostingId] [int] NOT NULL,
	[RequirementId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[MinimumLevel] [int] NULL,
	[DesiredLevel] [int] NULL,
	[Justification] [varchar](2000) NULL,
 CONSTRAINT [PK_JobRequirement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MatchChatAnalysisResults]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchChatAnalysisResults](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OverallMatchPercentage] [decimal](5, 2) NOT NULL,
	[MatchDescriptionSummary] [varchar](2000) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[HTML] [varchar](max) NULL,
	[JSON] [varchar](max) NULL,
	[ResumeAppIdentifier] [uniqueidentifier] NULL,
	[JobPostingAppIdentifier] [uniqueidentifier] NULL,
	[AppIdentifier] [uniqueidentifier] NULL,
	[JobPostingId] [int] NOT NULL,
	[ResumeId] [int] NOT NULL,
 CONSTRAINT [PK_ChatAnalysisResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prompts]    Script Date: 11/18/2025 10:11:18 AM ******/
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
	[FileExtension] [varchar](20) NULL,
 CONSTRAINT [PK_Prompts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromptTypes]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromptTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Requirements]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Requirements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Requirement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resumes]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resumes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Personality] [varchar](5000) NULL,
	[Summary] [varchar](5000) NULL,
	[AppIdentifier] [uniqueidentifier] NULL,
	[HTML] [varchar](max) NULL,
	[JSON] [varchar](max) NULL,
	[DocumentUploadId] [int] NOT NULL,
 CONSTRAINT [PK_Resume] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumeSkills]    Script Date: 11/18/2025 10:11:18 AM ******/
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
	[Justification] [varchar](300) NULL,
 CONSTRAINT [PK_ResumeSkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Skills]    Script Date: 11/18/2025 10:11:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Skill] [varchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Description] [varchar](2000) NULL,
 CONSTRAINT [PK_Skills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AgentPersonas] ADD  CONSTRAINT [DF_AgentPersonas_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AgentPersonas] ADD  CONSTRAINT [DF_AgentPersonas_IsFinalApprover]  DEFAULT ((0)) FOR [IsFinalApprover]
GO
ALTER TABLE [dbo].[AgentPersonaTypes] ADD  CONSTRAINT [DF_AgentTypes_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AnalysisAgentResults] ADD  CONSTRAINT [DF_AnalysisAgentResults_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_Uploads_FileType]  DEFAULT ((0)) FOR [FileType]
GO
ALTER TABLE [dbo].[DocumentUploads] ADD  CONSTRAINT [DF_Uploads_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[JobPostings] ADD  CONSTRAINT [DF_JobPostings_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Prompts] ADD  CONSTRAINT [DF_Prompts_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PromptTypes] ADD  CONSTRAINT [DF_PromptTypes_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Resumes] ADD  CONSTRAINT [DF_Resume_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ResumeSkills] ADD  CONSTRAINT [DF_ResumeSkills_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[AgentPersonas]  WITH CHECK ADD  CONSTRAINT [FK_AgentPersonasTypes_AgentPersonas] FOREIGN KEY([AgentPersonaTypeId])
REFERENCES [dbo].[AgentPersonaTypes] ([Id])
GO
ALTER TABLE [dbo].[AgentPersonas] CHECK CONSTRAINT [FK_AgentPersonasTypes_AgentPersonas]
GO
ALTER TABLE [dbo].[AnalysisAgentResults]  WITH CHECK ADD  CONSTRAINT [FK_AnalysisAgentResults_AgentPersonas] FOREIGN KEY([AgentPersonaId])
REFERENCES [dbo].[AgentPersonas] ([Id])
GO
ALTER TABLE [dbo].[AnalysisAgentResults] CHECK CONSTRAINT [FK_AnalysisAgentResults_AgentPersonas]
GO
ALTER TABLE [dbo].[AnalysisAgentResults]  WITH CHECK ADD  CONSTRAINT [FK_AnalysisAgentResults_ChatAnalysisResults] FOREIGN KEY([ChatAnalysisResultId])
REFERENCES [dbo].[MatchChatAnalysisResults] ([Id])
GO
ALTER TABLE [dbo].[AnalysisAgentResults] CHECK CONSTRAINT [FK_AnalysisAgentResults_ChatAnalysisResults]
GO
ALTER TABLE [dbo].[JobPostings]  WITH CHECK ADD  CONSTRAINT [FK_JobPostings_DocumentUploads] FOREIGN KEY([DocumentUploadId])
REFERENCES [dbo].[DocumentUploads] ([Id])
GO
ALTER TABLE [dbo].[JobPostings] CHECK CONSTRAINT [FK_JobPostings_DocumentUploads]
GO
ALTER TABLE [dbo].[JobRequirements]  WITH CHECK ADD  CONSTRAINT [FK_JobRequirements_JobPostings] FOREIGN KEY([JobPostingId])
REFERENCES [dbo].[JobPostings] ([Id])
GO
ALTER TABLE [dbo].[JobRequirements] CHECK CONSTRAINT [FK_JobRequirements_JobPostings]
GO
ALTER TABLE [dbo].[JobRequirements]  WITH CHECK ADD  CONSTRAINT [FK_JobRequirements_Requirements] FOREIGN KEY([RequirementId])
REFERENCES [dbo].[Requirements] ([Id])
GO
ALTER TABLE [dbo].[JobRequirements] CHECK CONSTRAINT [FK_JobRequirements_Requirements]
GO
ALTER TABLE [dbo].[MatchChatAnalysisResults]  WITH CHECK ADD  CONSTRAINT [FK_MatchChatAnalysisResults_JobPostings] FOREIGN KEY([JobPostingId])
REFERENCES [dbo].[JobPostings] ([Id])
GO
ALTER TABLE [dbo].[MatchChatAnalysisResults] CHECK CONSTRAINT [FK_MatchChatAnalysisResults_JobPostings]
GO
ALTER TABLE [dbo].[MatchChatAnalysisResults]  WITH CHECK ADD  CONSTRAINT [FK_MatchChatAnalysisResults_Resumes] FOREIGN KEY([ResumeId])
REFERENCES [dbo].[Resumes] ([Id])
GO
ALTER TABLE [dbo].[MatchChatAnalysisResults] CHECK CONSTRAINT [FK_MatchChatAnalysisResults_Resumes]
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
