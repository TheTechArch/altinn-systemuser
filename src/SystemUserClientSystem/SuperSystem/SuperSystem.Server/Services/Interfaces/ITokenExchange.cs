namespace SuperSystem.Server.Services.Interfaces
{
    public interface ITokenExchange
    {
        Task<string> ExhangeMaskinporten(string token);
    }
}
