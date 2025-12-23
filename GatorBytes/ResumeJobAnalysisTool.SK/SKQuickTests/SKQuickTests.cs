using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.TextToAudio;
using OpenAI.Images;
using Qdrant.Client;
using ResumeJobAnalysisTool.Shared.Interfaces;
using ResumeJobAnalysisTool.Shared.Models;
using ResumeJobAnalysisTool.SK.RAG;
using TextContent = Microsoft.SemanticKernel.TextContent;

#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

namespace ResumeJobAnalysisTool.SK.SKQuickTesting
{
    public class SKQuickTests
    {
        private ConfigurationValues _configValues = new ConfigurationValues();

        public SKQuickTests(ConfigurationValues configValues)
        {
            _configValues = configValues;
        }


        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public async Task RunTests(SKQuickTestOptions options, IChatCompletionService chatCompletionService, ConfigurationValues configValues)
        {

            var quickTestAudioPath = @"C:\temp\";
            var quickTestImagePath = @"C:\temp\friday";
            var imageFilePrefix = "test_image_";
            var audioFilePrefix = "test_audio_";
            var ragFilePath = @"C:\temp\test.txt";


            SKQuickTests skQuickTests = new SKQuickTests(_configValues);
            if (options.ShouldTestImage)
            {
                var imageDescription = "Draw a boat";

                await skQuickTests.TestImage(imageDescription, quickTestImagePath, imageFilePrefix, 1);
            }

            if (options.ShouldTestTextToAudio)
            {
                var textToAudioServe = options.Kernel.GetRequiredService<ITextToAudioService>();

                await skQuickTests.TestAudioAsync("Hello There", quickTestAudioPath, audioFilePrefix, textToAudioServe);
            }

            if (options.ShouldTestRAGSearch)
            {
                var embeddingGenerator = options.Kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

                var vectorStore = new QdrantVectorStore(
                   new QdrantClient("localhost"),
                   ownsClient: true,
                   new QdrantVectorStoreOptions
                   {
                       EmbeddingGenerator = embeddingGenerator
                   });

                await skQuickTests.SearchOnExistingAsync(vectorStore);
            }

            if (options.ShouldTestRAGUploadAndSearch)
            {
                var embeddingGenerator = options.Kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

                await skQuickTests.UploadAndSearchAsync(embeddingGenerator);
            }

            if (options.ShouldUploadToRag)
            {
                
            }
        }

        public async Task TestImage(string imageText, string filePath, string fileNamePrefix, int loopCount = 0)
        {
            List<string> instructions = new List<string>
            {

            };


            foreach (var instruction in instructions)
            {

                loopCount = loopCount < 1 ? 1 : loopCount;

                string dateStr = GetFormattedDateTime();

                // Create the OpenAI ImageClient
                ImageClient client = new(_configValues.OpenAISettings.OpenAI_ImageModel,
                    _configValues.OpenAISettings.OpenAI_ApiKey);


                for (int i = 0; i < loopCount; i++)
                {
                    // Generate the image
                    GeneratedImage generatedImage = await client.GenerateImageAsync(imageText,
                        new OpenAI.Images.ImageGenerationOptions
                        {
                            //Size = GeneratedImageSize.W1024xH1024
                            //a
                            //ResponseFormat = GeneratedImageFormat.Bytes,
                            //Size = GeneratedImageSize.W1792xH1024,
                            //Style = GeneratedImageStyle.Natural,
                            //Quality = GeneratedImageQuality.High
                        });
                    var bytes = generatedImage.ImageBytes;
                    var byteArr = bytes.ToArray();
                    var safePath = GetSafePath(filePath, fileNamePrefix, i, dateStr);
                    File.WriteAllBytes(safePath, byteArr);
                }
            }
        }

        private string GetFormattedDateTime()
        {
            return DateTimeOffset.Now.ToString("yyyyMMdd_HHmmss");
        }

        public async Task TestAudioAsync(string audioText, string filePath, string fileName, ITextToAudioService textToAudioService, int loopCount = 0)
        {
            loopCount = loopCount < 1 ? 1 : loopCount;

            string dateStr = GetFormattedDateTime();

            for (int i = 0; i < loopCount; i++)
            {
                // Set execution settings (optional)
                OpenAITextToAudioExecutionSettings executionSettings = new()
                {
                    Voice = "shimmer", // The voice to use when generating the audio.
                                       // Supported voices are alloy, echo, fable, onyx, nova, and shimmer.
                    ResponseFormat = "mp3", // The format to audio in.
                                            // Supported formats are mp3, opus, aac, and flac.
                    Speed = 1.0f // The speed of the generated audio.
                                 // Select a value from 0.25 to 4.0. 1.0 is the default.
                };

                var safeFilePath = GetSafePath(filePath, fileName, i, dateStr);

                // Convert text to audio
                AudioContent audioContent = await textToAudioService.GetAudioContentAsync(audioText, executionSettings);

                if (audioContent.Data.HasValue)
                {
                    File.WriteAllBytes(safeFilePath, audioContent.Data.Value.ToArray());
                }
            }
        }

        public async Task TestLocalRAGAsync(string inputFilePath, IChatCompletionService chatCompletionService)
        {
            var chatHistory = new ChatHistory("You are a friendly assistant.");

            var ragQuestion = "";
            for (int i = 0; i < 1; i++)
            {
                var fileBytes = File.ReadAllBytes(inputFilePath);

                ragQuestion = "Convert this file to text that will be uploaded to a RAG database.  The output needs to include as much information as possible.  Take your time and make sure that all the main points in the image are covered.  The image will be a PDF describing a product called ShipExec.  Do not include any helping text or questions, output just the results of the summary and nothing more.";
                var withFile = true;

                if (withFile)
                {
                    chatHistory.AddUserMessage([
                        new TextContent(ragQuestion),
                        new BinaryContent(fileBytes, "application/pdf")
                        ]);
                }
                else
                {
                    chatHistory.AddUserMessage(
                    [
                        new TextContent(ragQuestion),
                    ]);
                }


                var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                var stop = 1;
            }
        }

        private string GetSafePath(string filePath, string fileNamePrefix, int i, string dateString)
        {
            var filePathDash = filePath.EndsWith("\\") ? filePath : filePath + "\\";
            var fileName = string.Format(@"{2}_{0}{1}.png", fileNamePrefix, i, dateString);
            var fullPath = string.Format("{0}{1}", filePathDash, fileName);
            return fullPath;
        }

        public async Task UploadAndSearchAsync(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            // The data model
            var vectorStore = new QdrantVectorStore(
                new QdrantClient("localhost"),
                ownsClient: true,
                new QdrantVectorStoreOptions
                {
                    EmbeddingGenerator = embeddingGenerator
                });

            var collection = vectorStore.GetCollection<Guid, FinanceInfo>("finances");
            await collection.EnsureCollectionExistsAsync();

            // Create some test data.
            string[] budgetInfo =
            {
                "The budget for 2020 is EUR 100 000",
                "The budget for 2021 is EUR 120 000",
                "The budget for 2022 is EUR 150 000",
                "The budget for 2023 is EUR 200 000",
                "The budget for 2024 is EUR 364 000"
            };

            // Embeddings are generated automatically on upsert.
            var records = budgetInfo.Select((input, index) => new FinanceInfo { Key = Guid.NewGuid(), Text = input });
            await collection.UpsertAsync(records);

            // Embeddings for the search is automatically generated on search.
            var searchResult = collection.SearchAsync(
                "What is my budget for 2024?",
                top: 1);

            // Output the matching result.
            await foreach (var result in searchResult)
            {
                Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Text}");
            }
        }


        public async Task SearchOnExistingAsync(QdrantVectorStore vectorStore)
        {
            var collection = vectorStore.GetCollection<Guid, FinanceInfo>("finances");
            await collection.EnsureCollectionExistsAsync();

            // Embeddings for the search is automatically generated on search.
            var searchResult = collection.SearchAsync(
                "What is my budget for 2024?",
                top: 1);

            // Output the matching result.
            await foreach (var result in searchResult)
            {
                Console.WriteLine($"Key: {result.Record.Key}, Text: {result.Record.Text}");
            }
        }
    }
}

