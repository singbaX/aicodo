namespace AiCodo.Services
{
    public interface ITokenService
    {
        string CreateToken(params string[] claimNameValues);
    }
}
