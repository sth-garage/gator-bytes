using Microsoft.Extensions.Configuration;
using ResumeJobAnalysisTool.Shared.Models;

namespace ResumeJobAnalysisTool.Shared.Utility
{
    public class UserSecretManager
    {

        // dotnet user-secrets set "Movies:ServiceApiKey" "12345"
        // dotnet user-secrets set "ConnectionStrings:ConnectionString_ResumeJobAnalysisTool" "Data Source=127.0.0.1;Initial Catalog=ResumeJobAnalysisTool2;User Id=teachersPetSQLService;Password=Testing777!!;TrustServerCertificate=True"
        public static ConfigurationValues GetSecrets(IConfigurationRoot? configurationRoot)
        {
            var result = new ConfigurationValues();

            if (configurationRoot != null)
            {

                result = new ConfigurationValues
                {

                    OpenAISettings = new OpenAISettings
                    {
                        OpenAI_ApiKey = configurationRoot["OpenAI_ApiKey"],
                        OpenAI_ApiUrl = configurationRoot["OpenAI_ApiUrl"],
                        OpenAI_EmbeddingModel = configurationRoot["OpenAI_EmbeddingModel"],
                        OpenAI_ImageModel = configurationRoot["OpenAI_ImageModel"],
                        OpenAI_Model = configurationRoot["OpenAI_Model"],
                        OpenAI_TextToAudioModel = configurationRoot["OpenAI_TextToAudioModel"],
                    },
                    ConnectionStrings = new ConnectionStrings
                    {
                        ConnectionString_ResumeJobAnalysisTool = configurationRoot["ConnectionString_ResumeJobAnalysisTool"]
                    }
                };
            }

            return result;
        }
    }
}
