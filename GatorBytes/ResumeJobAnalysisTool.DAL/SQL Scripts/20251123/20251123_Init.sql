USE [ResumeJobAnalysisTool]
GO
SET IDENTITY_INSERT [dbo].[PromptTypes] ON 
GO
INSERT [dbo].[PromptTypes] ([Id], [Type], [CreatedOn], [IsActive]) VALUES (1, N'System', CAST(N'2025-11-17T01:08:17.193' AS DateTime), 1)
GO
INSERT [dbo].[PromptTypes] ([Id], [Type], [CreatedOn], [IsActive]) VALUES (2, N'GenericPDFFileConversion', CAST(N'2025-11-17T01:08:17.207' AS DateTime), 1)
GO
INSERT [dbo].[PromptTypes] ([Id], [Type], [CreatedOn], [IsActive]) VALUES (3, N'ResumeFileAnalysis', CAST(N'2025-11-17T01:08:17.213' AS DateTime), 1)
GO
INSERT [dbo].[PromptTypes] ([Id], [Type], [CreatedOn], [IsActive]) VALUES (4, N'JobPostingFileAnalysis', CAST(N'2025-11-17T01:08:17.217' AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[PromptTypes] OFF
GO

