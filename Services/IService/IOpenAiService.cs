namespace ChatGPT_CSharp.Services.IService
{
    public interface IOpenAiService
    {
        Task<string> GetCompletionAsync(string prompt);
    }
}
