using AirQualityUI.Models;
using Microsoft.EntityFrameworkCore;

namespace AirQualityUI.Services
{
    public class ModuleService : IModuleService
    {
        //Modulok adatait kezelő szerviz.

        //AirQualityDbContext inicializálása, hogy elérhető legyen az adatbázis közvetlenül.
        private readonly AirQualityDbContext _dbContext;
        public ModuleService(AirQualityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Összes modult visszaadó feladat.
        public async Task<List<Module>> GetModules()
        {
            return await _dbContext.Modules.ToListAsync();
        }

        //Megadott felhasználói azonosítóhoz tartozó modul listát visszaadó feladat.
        public async Task<List<Module>> GetModulesByUserId(int id)
        {
            List<Module> modules = new List<Module>();
            var usermodules = await _dbContext.UserModuleConnections.Where(item => item.UserId == id).ToListAsync();
            foreach (var item in usermodules)
            {
                var module = await _dbContext.Modules.FirstOrDefaultAsync(mod => mod.Id == item.ModuleId);
                if (module != null)
                {
                    modules.Add(module);
                }
            }
            return modules;
        }

        //Megadott felhasználónévhez tartozó modul listát visszaadó feladat.
        public async Task<List<Module>> GetModulesByUserName(string uname)
        {
            List<Module> modules = new List<Module>();
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == uname);
            var usermodules = await _dbContext.UserModuleConnections.Where(item => item.UserId == user.Id).ToListAsync();
            foreach (var item in usermodules)
            {
                var module = await _dbContext.Modules.FirstOrDefaultAsync(mod => mod.Id == item.ModuleId);
                if (module != null)
                {
                    modules.Add(module);
                }
            }
            return modules;
        }

        //Megadott modulnévhez tartozó objektumot visszaadó feladat.
        public async Task<Module> GetModuleByName(string name)
        {
            try
            {
                return await _dbContext.Modules.FirstAsync(module => module.ModuleName == name);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            
        }

        //Megadott azonosítóhoz tartozó modult visszaadó feladat.
        public async Task<Module> GetModuleById(long id)
        {
            return await _dbContext.Modules.FirstAsync(module => module.Id == id);
        }

        //Összes felhasználó és modul közötti kapcsolatokat visszaadó feladat.
        public async Task<IEnumerable<UserModuleConnection>> GetConnections()
        {
            return await _dbContext.UserModuleConnections.ToListAsync();
        }

        //Új modult rögzítő feladat.
        public async Task<Module> PostNewModule(Module module)
        {
            try
            {
                await _dbContext.Modules.AddAsync(module);
                await _dbContext.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
            
        }

        //Módosított modul adatainak eltárolására szolgáló feladat.
        public async Task<Module> PutModifiedModule(Module module)
        {
            try
            {
                Module moduleForModification = await _dbContext.Modules.FirstOrDefaultAsync(mod => mod.Id == module.Id);
                moduleForModification.ModuleName = module.ModuleName;
                await _dbContext.SaveChangesAsync();
                return moduleForModification;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }

        }

        //Megadott azonosítóval rendelkező modul törlését elvégző feladat (nem kerül felhasználásra).
        public async Task<Module> DeleteModuleById(long id)
        {
            try
            {
                Module moduleForDeletion = new Module();
                if (_dbContext.Modules.Any(module => module.Id == id))
                {
                    moduleForDeletion = await _dbContext.Modules.FirstOrDefaultAsync(module => module.Id == id);
                    _dbContext.Remove(moduleForDeletion);
                    _dbContext.SaveChangesAsync();
                }
                return moduleForDeletion;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
