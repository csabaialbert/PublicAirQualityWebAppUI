using AirQualityUI.Models;

namespace AirQualityUI.Services
{
    public interface IModuleService
    {
        //Interfész a ModuleService szerviz eléréséhez.
        Task<List<Module>> GetModules();
        Task<IEnumerable<UserModuleConnection>> GetConnections();
        Task<List<Module>> GetModulesByUserId(int id);
        Task<List<Module>> GetModulesByUserName(string uname);
        Task<Module> GetModuleByName(string name);
        Task<Module> GetModuleById(long id);
        Task<Module> PostNewModule(Module module);
        Task<Module> PutModifiedModule(Module module);
        Task<Module> DeleteModuleById(long Id);
    }
}
