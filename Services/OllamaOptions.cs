namespace elearn_server.Services;

public class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434/";

    public string Model { get; set; } = "llama3.2:3b";

    public bool EnableRecommendations { get; set; } = true;
}
