//using Agents;
//using Azure;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.Agents;
//using Microsoft.SemanticKernel.Agents.OpenAI;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;
//using OpenAI.Assistants;
//using ResumeJobAnalysisTool.DAL.CombinedModels;
//using ResumeJobAnalysisTool.DAL.Context;
//using ResumeJobAnalysisTool.DAL.Models;
//using ResumeJobAnalysisTool.Shared;
//using ResumeJobAnalysisTool.Shared.Interfaces;
//using ResumeJobAnalysisTool.Shared.Models;
//using System.Text;
//using System.Text.Json;

//#pragma warning disable OPENAI001
//#pragma warning disable SKEXP0110
//#pragma warning disable SKEXP0001

//#pragma warning disable OPENAI001

//namespace ResumeJobAnalysisTool.SK.Agents
//{
//    public class TechAgentManager : AgentManager
//    {
//        private HRContext _context;
//        private AgentPersonaManager _agentPersonaManager;
//        private AssistantClient _assistantClient;
//        private ConfigurationValues _configValues;
//        private Kernel _kernel;
//        private IVectorProcessor _vectorProcessor;

//        public TechAgentManager(HRContext context, ConfigurationValues configValues, Kernel kernel, IVectorProcessor vectorProcessor)
//        {
//            _context = context;
//            _agentPersonaManager = new AgentPersonaManager(_context);
//            _assistantClient = new AssistantClient(configValues.OpenAISettings.OpenAI_ApiKey);
//            _configValues = configValues;
//            _kernel = kernel;

//            _vectorProcessor = vectorProcessor;
//        }

//        public async Task<List<ChatMessageContent>> GetResumeAnalysisWithFilesTeacher(ConfigurationValues configValues, Kernel kernel, HRContext hrContext)
//        {

            

//            var output = new List<ChatMessageContent>();

//            //var output = await BeginHRChatWithFilesTeacher(chat, resumePath, jobDescriptionPath, resumeFileName, jobDescriptionFileName); //, jobDescription);

//            //resumeFileName = resumeFileName.Replace(".pdf", "").Replace("_", " ");
//            //jobDescriptionFileName = jobDescriptionFileName.Replace(".pdf", "");
//            //int maxLenJob = 20;

//            //jobDescriptionFileName = jobDescriptionFileName.Length <= maxLenJob
//            //    ? jobDescriptionFileName.PadRight(maxLenJob, ' ')
//            //    : jobDescriptionFileName.Substring(0, maxLenJob);


//            //int maxLenName = 20;

//            //resumeFileName = resumeFileName.Length <= maxLenName
//            //    ? resumeFileName.PadRight(maxLenName, ' ')
//            //    : resumeFileName.Substring(0, maxLenName);

//            return output;
//        }



//        public async Task ProcessResumesAndJobPostings()
//        {
            
//            var resumes = _context.Resumes.Where(x => x.IsActive).ToList();
//            var jobPostings = _context.JobPostings.Where(x => x.IsActive).ToList();

//            int jobC = 0;
//            int resC = 0;

//            foreach (var resume in resumes)
//            {
//                resC++;
//                jobC = 0;
//                foreach(var jobPosting in jobPostings)
//                {
//                    jobC++;
//                    var analysis = _context.MatchChatAnalysisResults.FirstOrDefault(x => x.JobPostingId == jobPosting.Id && x.ResumeId == resume.Id && x.IsActive == true);
//                    if (analysis == null)
//                    {
                        

//                        var matchResult = await ProcessResumeAndJobPosting(resume, jobPosting);
//                        if (matchResult != null)
//                        {
//                            _context.MatchChatAnalysisResults.AddAsync(matchResult);
//                            try
//                            {
//                                await _context.SaveChangesAsync();
//                                await _vectorProcessor.IngestMatchAnalysisDataAsync(matchResult, () => Guid.NewGuid());
//                            }
//                            catch(Exception ex)
//                            {
//                                //var stop = 1;
//                                Thread.Sleep(60000);
//                                await ProcessResumesAndJobPostings();
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public async Task<MatchChatAnalysisResult> ProcessResumeAndJobPosting(Resume resume, JobPosting jobPosting)
//        {
//            var appGuid = Guid.NewGuid();

//            MatchChatAnalysisResult result = new MatchChatAnalysisResult
//            {
//                //AppIdentifier = appGuid,
//                Resume = resume,
//                JobPosting = jobPosting,
//                //ResumeAppIdentifier = resume.AppIdentifier,
//                //JobPostingAppIdentifier = jobPosting.AppIdentifier,
//                CreatedOn = DateTime.Now,
//                IsActive = true

//            };
//            AgentGroupChatEnhanced chat = null;

//            try
//            {

//                try
//                {
//                    var test = String.Format("You are: {0}.  You are analyzing how well a given resume fits a given job description.  You have assistants that will analyze both documents and make recommendations.  Provide results in rich HTML.  After you make the final decision, you say ApprovedAndDone",
//                _agentPersonaManager.GetInitialSetupAgent());
//                    chat = await GetAgentGroupChatForTech(test, _assistantClient, _configValues.OpenAISettings.OpenAI_Model, _kernel);
//                }
//                catch (Exception ex)
//                {
//                    var stop = 1;
//                }

//                CompleteAgentChatResult output = new CompleteAgentChatResult();

//                try
//                {
//                    var finalApprover = _agentPersonaManager.GetFinalApproverAgent();
//                    output = await BeginHRChatWithFilesTeacher(chat, resume, jobPosting, result, finalApprover.FinalApproverKeyword);
//                }
//                catch (Exception ex)
//                {
//                    var stop = 1;
//                }


//                var finalOutput = JsonSerializer.Deserialize<FinalAgentAnalysisResult>(output.FinalOutput);


//                string summary = finalOutput.justification ;
//                decimal matchPercentage = (decimal) finalOutput.matchPercentage;
//                string html = finalOutput.html;
//                string json = "";

//                result.OverallMatchPercentage = matchPercentage;
//                result.MatchDescriptionSummary = summary;
//                result.AnalysisAgentResults = output.AgentResults;
//                result.Html = html;
//                //result.Json = json;

//            }
//            catch (Exception ex)
//            {
//                var stop = 1;
//            }


//            await chat.AgentGroupChat.ResetAsync();
//            return result;
//        }


//        protected async Task<CompleteAgentChatResult> BeginHRChatWithFilesTeacher(AgentGroupChatEnhanced agentGroupChat,
//            Resume resume,
//            JobPosting jobPosting,
//            MatchChatAnalysisResult matchChatAnalysisResult,
//            string finalApproverKeyword)
//        {
//            List<ChatMessageContent> messages = new List<ChatMessageContent>();

//            //List<AnalysisAgentResult> analysisAgentResults = new List<AnalysisAgentResult>();

//            Guid analysisGuid = Guid.NewGuid();

//            // Invoke chat and display messages.
//            ChatMessageContent input = new(AuthorRole.User, @"Analyze the provided files.  One is a job and the other is a resume.  
//            Analyze the fit for the provided resume for the job description.

//            DO NOT USE SINGLE QUOTES ANYWHERE IN THE RESULT

//            Return results in the following json format {
//                        ""agentName"": <name of the agent>,
//                        ""messageCount"": <the number of messages this agent has added to the chat>,
//                        ""matchPercentage"": <numerical match percentage to 2 decimal places, 38.2 or 87.22 for example>,
//                        ""justification"": <justification for the score given the input>,
//                        ""matchPercentageAdjustment"": <numerical value for the difference between the match percentage field and the last match percentage the agent gave - going from 38.2 to 40.2 would be 2.0, from 98 to 94.2 would be -4.2>,
//                        ""matchPercenageAdjustmentJustification"": <the reason for the match adjustment>,
//                        ""summary"": <an overall summary of the current match under 2000 characters>
//                    }");

//            var resumeHTMLBytes = Encoding.UTF8.GetBytes(resume.Html);
//            //var resumeJsonBytes = Encoding.UTF8.GetBytes(resume.Json);

//            var jobPostingHTMLBytes = Encoding.UTF8.GetBytes(jobPosting.Html);
//            //var jobPostingJsonBytes = Encoding.UTF8.GetBytes(jobPosting.Json);

//            input.Items.Add(new BinaryContent(resumeHTMLBytes, "application/pdf"));
//            input.Items.Add(new BinaryContent(jobPostingHTMLBytes, "application/pdf"));
//            input.AuthorName = "Main";


//            agentGroupChat.AgentGroupChat.AddChatMessage(input);
//            List<string> allContents = new List<string>();
//            var i = 0;
//            var j = 1;

//            //var authorCounts = new Dictionary<string, int>();

//            var agentResults = new List<AnalysisAgentResult>();
//            var lastResult = new AnalysisAgentResult();
//            var lastContent = "";
//            var analysisAgentResult = new AnalysisAgentResult();
//            ChatMessageContent lastChat = null;
            

//            Dictionary<int, string> previousAdminMessages = new Dictionary<int, string>();

//            try
//            {
//                await foreach (ChatMessageContent response in agentGroupChat.AgentGroupChat.InvokeAsync())
//                {
//                    i++;

//                    lastChat = response;
//                    var content = response.Content;
  
//                    messages.Add(response);

//                    var shouldContinue = true;

//                    lastContent = response.Content.Replace(finalApproverKeyword, "").Trim();

//                    if (response.Content.IndexOf(finalApproverKeyword) > 0)
//                    {

//                        //var stop = 1;
//                        return new CompleteAgentChatResult
//                        {
//                            AgentResults = agentResults,
//                            FinalOutput = lastContent,
//                            FinalChatMessage = lastChat
//                        };
//                    }
//                    //response.Content = response.Content.Replace("'", "\"");
//                        AgentAnalysisResult individualMessageContent = null;
//                        try
//                        {
//                            individualMessageContent = JsonSerializer.Deserialize<AgentAnalysisResult>(response.Content.Replace(finalApproverKeyword, "").Trim());
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine(ex.Message);
//                            Console.WriteLine(ex.InnerException);
//                            Console.WriteLine(lastContent);
//                        previousAdminMessages.Add(i, response.Content);
//                            shouldContinue = false;
//                        //continue;
//                        }

//                        if (shouldContinue)
//                        {

//                            var agentId = 0;

//                            ChatCompletionAgentEnhanced agentResponse = null;
//                            var finalReviewerChatCount = 0;
//                            try
//                            {
//                                //if (response.)
//                                agentResponse = agentGroupChat.EnhancedChatCompletionAgents.FirstOrDefault(x => x.ChatCompletionAgent.Name == response.AuthorName);
//                                if (agentResponse != null)
//                                {
//                                    agentId = agentResponse.AgentId;
//                                }
//                                else
//                                {
//                                    finalReviewerChatCount++;
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                var stop = 1;
//                            }



//                            try
//                            {
//                                analysisAgentResult = new AnalysisAgentResult
//                                {
//                                    AgentChatNumber = i,
//                                    AgentChatNumberOverall = i,
//                                    AgentPersonaId = agentId,
//                                    //AnalysisAppIdentifier = matchChatAnalysisResult.AppIdentifier,
//                                    MatchPercentage = (decimal) individualMessageContent.matchPercentage,
//                                    MatchPercentageAdjustment = (decimal) individualMessageContent.matchPercentageAdjustment,
//                                    CreatedOn = DateTime.Now,
//                                    MatchChatAnalysisResult  = matchChatAnalysisResult,
//                                    Justification = individualMessageContent.justification,
//                                    MatchPercentageAdjustmentJustification = individualMessageContent.matchPercenageAdjustmentJustification,
//                                    Summary = individualMessageContent.summary,
                                    
//                                };
//                                lastResult = analysisAgentResult;

//                                if (agentId > 0)
//                                {
//                                    agentResults.Add(analysisAgentResult);
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                var stop = 1;
//                            }

//                        Console.WriteLine("");
//                        Console.WriteLine("-------------------------------------------------");
//                        Console.WriteLine("");
//                        Console.WriteLine("Message " + i + " received: " + response.AuthorName);
//                        Console.WriteLine("    Current: " + analysisAgentResult.MatchPercentage + "%"); 
//                        Console.WriteLine("    Change: " + analysisAgentResult.MatchPercentageAdjustment + "%"); 
//                        Console.WriteLine("    Justification: " + analysisAgentResult.MatchPercentageAdjustmentJustification);
//                        Console.WriteLine("");
//                        Console.WriteLine("-------------------------------------------------");
//                        //var fileName = resume.Name + " - " + jobPosting.Name + " - " + response.AuthorName + "_message_" + authorCount + "_total_" + i + ".json";
//                        //File.WriteAllText(@"C:\Code\stuff\take2\" + fileName, response.Content);
//                        }
//                        else
//                        {
//                            Console.WriteLine("SKIPPING");
 
//                        }

//                    if (i != agentResults.Count)
//                    {
//                        if (previousAdminMessages.ContainsKey(i))
//                        {
//                            agentResults.Add(new AnalysisAgentResult
//                            {
//                                IsActive = true,
//                                AgentChatNumber = j,
//                                AgentChatNumberOverall = i,
//                                AgentPersonaId = _agentPersonaManager.GetFinalApproverAgent().Id,
//                                //AnalysisAppIdentifier = matchChatAnalysisResult.AppIdentifier,
//                                MatchPercentage = 0,
//                                MatchPercentageAdjustment = 0,
//                                MatchPercentageAdjustmentJustification = "Team Message",
//                                MatchChatAnalysisResult = matchChatAnalysisResult,
//                                CreatedOn = DateTime.Now,
//                                Justification = "Team Message",
//                                Summary = previousAdminMessages[i]
//                            });
//                            j++;
//                        }
//                    }
//                    Thread.Sleep(10000);

//                }
//            }
//            catch(Exception ex)
//            {
//                var stop = 1;
//            }

            



//            return new CompleteAgentChatResult
//            {
//                AgentResults = agentResults,
//                FinalOutput = lastContent,
//                FinalChatMessage = lastChat,
//                FinalApproverMessages = previousAdminMessages
//            };
//        }


//        protected string GetInitialAgentMessage()
//        {
//            var result = String.Format("You are: {0}.  You are analyzing how well a given resume fits a given job description.  You have assistants that will analyze both documents and make recommendations.  Provide results in rich HTML.  After you make the final decision, you say ApprovedAndDone",
//                _agentPersonaManager.GetInitialSetupAgent());
//            return result;
//        }

//        protected async Task<AgentGroupChatEnhanced> GetAgentGroupChatForTech(string query,
//            AssistantClient assistantClient,
//            string model,
//            Kernel kernel)
//        {
//            var finalApproverAgent = _agentPersonaManager.GetFinalApproverAgent();
//            var finalApprover = await GetAssistant("FinalReviewer", finalApproverAgent.Persona, model, assistantClient);

//            List<AgentPersona> chatAgents = _agentPersonaManager.GetGradingAgents();


//            List<ChatCompletionAgentEnhanced> agentListEnhanced = new List<ChatCompletionAgentEnhanced>();


//            foreach (var agentPersona in chatAgents)
//            {
//                agentListEnhanced.Add(new ChatCompletionAgentEnhanced
//                {
//                    AgentId = agentPersona.Id,
//                    ChatCompletionAgent = GetAgent(agentPersona.Name.Replace(" ", "_"), agentPersona.Persona, kernel)
//                });
//            }

//            var agentList = agentListEnhanced.Select(x => (Agent)x.ChatCompletionAgent).ToList();

//            OpenAIAssistantAgent finalApproverAssistant = new OpenAIAssistantAgent(finalApprover, assistantClient);

//            agentList.Add(finalApproverAssistant);

//            AgentGroupChat chat =
//                new(agentList.ToArray())
//                {
//                    ExecutionSettings =
//                        new()
//                        {
//                            // Here a TerminationStrategy subclass is used that will terminate when
//                            // an assistant message contains the term "approve".
//                            TerminationStrategy =
//                                new ApprovalTerminationStrategy(finalApproverAgent.FinalApproverKeyword)
//                                {
//                                    // Only the art-director may approve.
//                                    Agents = [finalApproverAssistant],
//                                    // Limit total number of turns
//                                    MaximumIterations = 50,
                                   
//                                },

                            
//                        },
                    
//                };

            

//            return new AgentGroupChatEnhanced
//            {
//                AgentGroupChat = chat,
//                EnhancedChatCompletionAgents = agentListEnhanced
//            };
//        }















//        public async Task<string> ConvertMatchAnalysisToHtml(MatchResult matchResult)
//        {
//            string result = "";
//            var serializedMR = JsonSerializer.Serialize(matchResult);
//            HttpClient customHttpClient = new HttpClient
//            {
//                Timeout = TimeSpan.FromMinutes(60) // Set timeout to 5 minutes (or any desired duration)
//            };

//            OpenAIChatCompletionService service = new OpenAIChatCompletionService(_configValues.OpenAISettings.OpenAI_Model,
//            _configValues.OpenAISettings.OpenAI_ApiKey, httpClient: customHttpClient);

//            ChatHistory chatHistory = new ChatHistory();
//            chatHistory.AddUserMessage([
//                        new TextContent("Convert the input json object into a single HTML file using the template provided"),
//                        new TextContent(serializedMR),
//                        new TextContent(TechAgentManager.TempTemplate),
//                    ]);
//            var reply = await service.GetChatMessageContentAsync(chatHistory);
//            result = reply.Content;
//            return result;
//        }




//        public static string TempTemplate = @"
//<!DOCTYPE html>
//<html lang=""en"">
//<head>
//  <meta charset=""utf-8"" />
//  <title>{{CompanyName}} - {{JobPosition}} - {{CandidateName}} Match Analysis</title>
//  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />

//  <style>
//    :root {
//      --bg: #f3f4f6;
//      --card-bg: #ffffff;
//      --text: #111827;
//      --muted: #6b7280;
//      --border: #e5e7eb;
//      --shadow: 0 1px 4px rgba(15, 23, 42, 0.06);
//      --radius: 10px;

//      --green: #166534;
//      --green-bg: #dcfce7;
//      --orange: #9a3412;
//      --orange-bg: #ffedd5;
//      --yellow: #854d0e;
//      --yellow-bg: #fef9c3;
//      --red: #7f1d1d;
//      --red-bg: #fee2e2;
//    }

//    /* Reset & base */
//    *, *::before, *::after {
//      box-sizing: border-box;
//    }

//    html, body {
//      margin: 0;
//      padding: 0;
//      background: var(--bg);
//      color: var(--text);
//      font-family: system-ui, -apple-system, BlinkMacSystemFont, ""Segoe UI"", sans-serif;
//      line-height: 1.5;
//    }

//    #content {
//      max-width: 980px;
//      margin: 24px auto 48px;
//      padding: 0 16px;
//    }

//    /* Header */
//    .header {
//      background: var(--card-bg);
//      border: 1px solid var(--border);
//      border-radius: var(--radius);
//      box-shadow: var(--shadow);
//      padding: 16px 18px;
//      font-size: 18px;
//      font-weight: 600;
//    }

//    /* Overall Summary */
//    .overallSummary {
//      margin-top: 20px;
//      background: var(--card-bg);
//      border: 1px solid var(--border);
//      border-radius: var(--radius);
//      box-shadow: var(--shadow);
//      padding: 14px 16px 16px;
//    }

//    .overallSummary-header {
//      display: flex;
//      align-items: center;
//      flex-wrap: wrap;
//      gap: 10px;
//      margin-bottom: 10px;
//    }

//    .score-badge {
//      display: inline-flex;
//      align-items: center;
//      justify-content: center;
//      min-width: 70px;
//      padding: 4px 10px;
//      border-radius: 999px;
//      font-weight: 600;
//      font-variant-numeric: tabular-nums;
//      border: 1px solid transparent;
//      box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.6);
//      font-size: 14px;
//    }

//    /* Score color scheme:
//       100–80 => .score-green
//        80–60 => .score-orange
//        60–40 => .score-yellow
//         < 40 => .score-red
//       Apply one of these via {{OverallScoreClass}} or {{AgentScoreClass}}.
//    */
//    .score-green  { color: var(--green);  background: var(--green-bg);  border-color: rgba(22, 101, 52, 0.3); }
//    .score-orange { color: var(--orange); background: var(--orange-bg); border-color: rgba(154, 52, 18, 0.3); }
//    .score-yellow { color: var(--yellow); background: var(--yellow-bg); border-color: rgba(133, 77, 14, 0.3); }
//    .score-red    { color: var(--red);    background: var(--red-bg);    border-color: rgba(127, 29, 29, 0.3); }

//    .overall-label {
//      padding-left: 8px; /* ""some left padding"" for the label */
//      font-size: 15px;
//      font-weight: 600;
//    }

//    .overallSummaryText {
//      margin-top: 4px;
//      font-size: 14px;
//      color: var(--text);
//    }

//    /* Common section card styling */
//    .jobInfo,
//    .employeeInfo,
//    .personas,
//    .agentChat {
//      margin-top: 20px;
//      background: var(--card-bg);
//      border: 1px solid var(--border);
//      border-radius: var(--radius);
//      box-shadow: var(--shadow);
//      padding: 14px 16px 16px;
//    }

//    .section-title {
//      font-weight: 600;
//      font-size: 15px;
//      margin-bottom: 8px;
//    }

//    .section-meta {
//      font-size: 13px;
//      color: var(--muted);
//    }

//    .jobInfo-body,
//    .employeeInfo-body {
//      margin-top: 6px;
//      font-size: 14px;
//    }

//    /* Personas */
//    .persona-row {
//      display: flex;
//      align-items: flex-start;
//      gap: 6px;
//      padding: 6px 8px;
//      border-radius: 8px;
//      margin-top: 6px;
//      font-size: 14px;
//      border: 1px solid var(--border);
//    }

//    .persona-name {
//      font-weight: 600;
//    }

//    .persona-description {
//      color: var(--muted);
//    }

//    .persona-final {
//      margin-left: auto;
//      font-size: 11px;
//      text-transform: uppercase;
//      letter-spacing: 0.04em;
//      color: var(--muted);
//    }

//    /* Persona color mapping (up to 7 personas).
//       Apply via persona-{{PersonaId}} or persona-{{SimplePersonaId}}. */
//    .persona-1 { border-left: 4px solid #1d4ed8; background: #eff6ff; }
//    .persona-2 { border-left: 4px solid #059669; background: #ecfdf5; }
//    .persona-3 { border-left: 4px solid #7c3aed; background: #f5f3ff; }
//    .persona-4 { border-left: 4px solid #f97316; background: #fff7ed; }
//    .persona-5 { border-left: 4px solid #0ea5e9; background: #f0f9ff; }
//    .persona-6 { border-left: 4px solid #e11d48; background: #fff1f2; }
//    .persona-7 { border-left: 4px solid #22c55e; background: #f0fdf4; }

//    /* Agent chat bubbles */
//    .agentChat-list {
//      display: flex;
//      flex-direction: column;
//      gap: 10px;
//      margin-top: 6px;
//    }

//    .chatBubble {
//      border-radius: 10px;
//      padding: 10px 12px;
//      font-size: 14px;
//      border: 1px solid var(--border);
//      background: #ffffff;
//    }

//    /* If persona-# is also on the bubble, reuse that accent */
//    .chatBubble.persona-1,
//    .chatBubble.persona-2,
//    .chatBubble.persona-3,
//    .chatBubble.persona-4,
//    .chatBubble.persona-5,
//    .chatBubble.persona-6,
//    .chatBubble.persona-7 {
//      border-left-width: 4px;
//    }

//    .chatBubble-header {
//      display: flex;
//      align-items: center;
//      justify-content: space-between;
//      gap: 8px;
//      margin-bottom: 4px;
//    }

//    .chatBubble-agentName {
//      font-weight: 600;
//    }

//    .chatBubble-summary {
//      margin-bottom: 4px;
//    }

//    .chatBubble-justification,
//    .chatBubble-adjustment {
//      margin-top: 4px;
//    }

//    .chatBubble-label {
//      font-weight: 600;
//      color: var(--muted);
//    }

//    /* MatchPercentageAdjustment display:
//       Use one of:
//         .adjustment-positive (green arrow up)
//         .adjustment-neutral  (black square, no change)
//         .adjustment-negative (red arrow down)
//       via {{AdjustmentClass}} per entry.
//    */
//    .adjustment-value {
//      font-variant-numeric: tabular-nums;
//      font-weight: 600;
//      margin-right: 6px;
//    }

//    .adjustment-positive {
//      color: var(--green);
//    }
//    .adjustment-positive::before {
//      content: ""▲ "";
//    }

//    .adjustment-neutral {
//      color: #111827;
//    }
//    .adjustment-neutral::before {
//      content: ""■ "";
//    }

//    .adjustment-negative {
//      color: var(--red);
//    }
//    .adjustment-negative::before {
//      content: ""▼ "";
//    }

//    /* Hide unused chat entries (e.g., for fixed 40-slot templates) */
//    .chatBubble.is-empty {
//      display: none;
//    }

//    .muted {
//      color: var(--muted);
//    }

//    @media (max-width: 640px) {
//      .header {
//        font-size: 16px;
//      }
//    }
//  </style>
//</head>
//<body>

//<div id=""content"">

//  <!-- Header -->
//  <div class=""header"">
//    <span>{{CompanyName}} - {{JobPosition}} Analysis for {{CandidateName}}</span>
//  </div>

//  <!-- Overall Summary -->
//  <div class=""overallSummary"">
//    <div class=""overallSummary-header"">
//      <!-- Overall match percentage.
//           Apply one of: score-green / score-orange / score-yellow / score-red
//           as {{OverallScoreClass}} based on {{OverallPercentage}}. -->
//      <span class=""score-badge {{OverallScoreClass}}"">
//        {{OverallPercentage}}%
//      </span>
//      <span class=""overall-label"">Overall Summary</span>
//    </div>
//    <div class=""overallSummaryText"">
//      {{MatchSummary}}
//    </div>
//  </div>

//  <!-- Job / Role Information -->
//  <div class=""jobInfo"">
//    <div class=""section-title"">
//      {{CompanyName}} – {{JobPosition}}
//    </div>
//    <div class=""section-meta"">
//      Job / Role Overview
//    </div>
//    <div class=""jobInfo-body"">
//      {{JobSummary}}
//    </div>
//  </div>

//  <!-- Candidate Information -->
//  <div class=""employeeInfo"">
//    <div class=""section-title"">
//      {{CandidateName}}
//    </div>
//    <div class=""section-meta"">
//      Candidate Overview
//    </div>
//    <div class=""employeeInfo-body"">
//      {{CandidateSummary}}
//    </div>
//  </div>

//  <!-- Personas -->
//  <div class=""personas"">
//    <div class=""section-title"">Evaluation Personas</div>

//    <!--
//      Repeat .persona-row for each entry in Personas:

//      JSON fields:
//        Name            -> {{Name}}
//        Description     -> {{Description}}
//        PersonaId       -> {{PersonaId}}
//        IsFinalReviewer -> {{IsFinalReviewer}} (boolean)

//      Apply color class: persona-{{PersonaId}}
//    -->
//    {{#Personas}}
//    <div class=""persona-row persona-{{PersonaId}}"">
//      <span class=""persona-name"">{{Name}}</span>
//      <span class=""persona-description"">{{Description}}</span>
//      {{#IsFinalReviewer}}
//      <span class=""persona-final"">Final reviewer</span>
//      {{/IsFinalReviewer}}
//    </div>
//    {{/Personas}}
//  </div>

//  <!-- Agent Chat / Match Analyses -->
//  <div class=""agentChat"">
//    <div class=""section-title"">Agent Match Analyses</div>
//    <div class=""section-meta"">
//      Individual persona assessments of the candidate–role match.
//    </div>

//    <div class=""agentChat-list"">
//      <!--
//        Repeat .chatBubble for each entry in AgentMatchAnalyses (max 40).

//        JSON fields:
//          Count                             -> {{Count}}
//          AgentName                         -> {{AgentName}}
//          AgentChatCount                    -> {{AgentChatCount}}
//          SimplePersonaId                   -> {{SimplePersonaId}}  (1–7, map to persona-{{SimplePersonaId}})
//          MatchPercentage                   -> {{MatchPercentage}}
//          Summary                           -> {{Summary}}
//          Justification                     -> {{Justification}}
//          MatchPercentageAdjustment         -> {{MatchPercentageAdjustment}}
//          MatchPercentageAdjustmentJustification -> {{MatchPercentageAdjustmentJustification}}

//        Additional template-only helpers you can provide:
//          AgentScoreClass -> one of score-green / score-orange / score-yellow / score-red
//          AdjustmentClass -> one of adjustment-positive / adjustment-neutral / adjustment-negative
//          EmptyClass      -> ""is-empty"" when this slot is unused (to hide it), otherwise """".
//      -->
//      {{#AgentMatchAnalyses}}
//      <div class=""chatBubble persona-{{SimplePersonaId}} {{EmptyClass}}"">
//        <div class=""chatBubble-header"">
//          <span class=""chatBubble-agentName"">
//            {{AgentName}}
//          </span>
//          <span class=""score-badge {{AgentScoreClass}}"">
//            {{MatchPercentage}}%
//          </span>
//        </div>

//        <!-- Optional short per-agent summary -->
//        <div class=""chatBubble-summary"">
//          {{Summary}}
//        </div>

//        <!-- Detailed justification text -->
//        <div class=""chatBubble-justification"">
//          <span class=""chatBubble-label"">Justification:</span>
//          <span>{{Justification}}</span>
//        </div>

//        <!-- Adjustment vs. prior/overall score -->
//        <div class=""chatBubble-adjustment"">
//          <!-- MatchPercentageAdjustment rendered with color + arrow/symbol -->
//          <span class=""adjustment-value {{AdjustmentClass}}"">
//            {{MatchPercentageAdjustment}}
//          </span>
//          <span class=""chatBubble-label"">Adjustment rationale:</span>
//          <span>{{MatchPercentageAdjustmentJustification}}</span>
//        </div>
//      </div>
//      {{/AgentMatchAnalyses}}
//    </div>
//  </div>

//</div>

//</body>
//</html>";


//    }
//}
