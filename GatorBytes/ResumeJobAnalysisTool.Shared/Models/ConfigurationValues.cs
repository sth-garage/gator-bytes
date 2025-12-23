namespace ResumeJobAnalysisTool.Shared.Models;

public class ConfigurationValues
{
    public OpenAISettings OpenAISettings { get; set; } = new OpenAISettings();

    public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
}

public class OpenAISettings
{
    public string OpenAI_ApiKey { get; set; } = "";

    public string OpenAI_Model { get; set; } = "";

    public string OpenAI_ApiUrl { get; set; } = "";

    public string OpenAI_EmbeddingModel { get; set; } = "";

    public string OpenAI_TextToAudioModel { get; set; } = "";

    public string OpenAI_ImageModel { get; set; } = "";
}


public class ConnectionStrings
{
    public string ConnectionString_ResumeJobAnalysisTool { get; set; } = "";
}

