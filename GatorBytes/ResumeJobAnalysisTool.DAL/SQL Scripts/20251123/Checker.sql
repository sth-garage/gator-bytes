SELECT count(1) FROM [ResumeJobAnalysisTool].[dbo].[MatchChatAnalysisResults] where IsActive = 1
SELECT count(1) FROM [ResumeJobAnalysisTool].[dbo].AnalysisAgentResults where IsActive = 1


SELECT * FROM [ResumeJobAnalysisTool].[dbo].[MatchChatAnalysisResults] where IsActive = 1 order by 1 desc
SELECT * FROM [ResumeJobAnalysisTool].[dbo].AnalysisAgentResults where IsActive = 1 order by 1 desc




--DELETE FROM [ResumeJobAnalysisTool].[dbo].AnalysisAgentResults where MatchChatAnalysisResultId = 31
--DELETE FROM [ResumeJobAnalysisTool].[dbo].[MatchChatAnalysisResults] where id = 31

