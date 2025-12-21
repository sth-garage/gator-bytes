using Microsoft.Extensions.Configuration;
using GatorBytes.Shared.Models;

namespace GatorBytes.Shared.Utility
{
    public class UserSecretManager
    {

        // dotnet user-secrets set "LMStudio_Model" "12345"
        // dotnet user-secrets set "ConnectionStrings:ConnectionString_GatorBytes" "Data Source=127.0.0.1;Initial Catalog=GatorBytes2;User Id=teachersPetSQLService;Password=Testing777!!;TrustServerCertificate=True"
        public static ConfigurationValues GetSecrets(IConfigurationRoot? configurationRoot)
        {
            var result = new ConfigurationValues();

            if (configurationRoot != null)
            {

                result = new ConfigurationValues
                {

                    LMStudioSettings = new LMStudioSettings
                    {
                        LMStudio_ApiKey = configurationRoot["LMStudio_ApiKey"] ?? "",
                        LMStudio_ApiUrl = configurationRoot["LMStudio_ApiUrl"] ?? "",
                        LMStudio_Model = configurationRoot["LMStudio_Model"] ?? "",
                    },
                    ConnectionStrings = new ConnectionStrings
                    {
                        ConnectionString_GatorBytes = configurationRoot["ConnectionString_GatorBytes"] ?? ""
                    }
                };
            }

            return result;
        }
    }
}
