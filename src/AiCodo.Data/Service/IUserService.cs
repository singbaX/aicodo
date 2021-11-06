namespace AiCodo.Data
{
    public interface IUserService
    {
        IUser Login(string username, string password);
    }
}
