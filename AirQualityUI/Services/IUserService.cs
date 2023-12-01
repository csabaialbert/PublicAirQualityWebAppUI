using AirQualityUI.Models;

namespace AirQualityUI.Services
{
    public interface IUserService
    {
        //Interfész az UserService szerviz eléréséhez.
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(long id);
        Task<User> AddNewUser(User newUser, List<Module> modules);
        Task<User> UpdateUserDetails(User updatedUser, List<Module> modules);
        Task<IEnumerable<UserModuleConnection>> GetConnections();
        Task<List<User>> GetUsersForModuleIds(int moduleId);
        Task<bool> AuthenticateModule(EmailPassw cred);
    }
}
