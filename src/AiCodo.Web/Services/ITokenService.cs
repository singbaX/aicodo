using AiCodo.Data;

namespace AiCodo.Web.Services
{
    public interface ITokenService
    {
        Token CreateToken(IUser user);
    }
}