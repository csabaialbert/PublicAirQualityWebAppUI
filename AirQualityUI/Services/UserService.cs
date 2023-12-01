using AirQualityUI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace AirQualityUI.Services
{
    public class UserService : IUserService
    {
        //Felhasználói adatokat kezelő szerviz.

        //AirQualityDbContext inicializálása, hogy elérhető legyen az adatbázis közvetlenül.
        private readonly AirQualityDbContext _dbContext;
        public UserService(AirQualityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Összes felhasználót visszaadó feladat
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        //Felhasználó objektumot visszaadó feladat, e-mail cím alapján.
        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }

        }

        //Felhasználó objektumot visszaadó feladat, felhasználónév alapján.
        public async Task<User> GetUserByUsername(string username)
        {
            
            try
            {
                return await _dbContext.Users.FirstAsync(user => user.UserName == username);
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        //Felhasználó objektumot visszaadó feladat, felhasználó azonosító alapján.
        public async Task<User> GetUserById(long id)
        {
            return await _dbContext.Users.FirstAsync(user => user.Id == id);
        }

        //Egy adott modul azonosító alapján visszaadja a felhasználók listáját, akik hozzárendelésre kerültek a modulhoz.
        public async Task<List<User>> GetUsersForModuleIds(int moduleId)
        {
            try
            {
                //kapcsolatok lekérdezése, modul azonosító alapján.
                var connections = await _dbContext.UserModuleConnections.Where(i => i.ModuleId == moduleId).ToListAsync();

                //összes felhasználó lekérdezése
                var users = await GetUsers();

                //Azon felhasználók listába rendezése, akikhez az adott modul hozzá van rendelve.
                var moduleUsers = users.Where(usr => usr.UserModuleConnections.Any(i => i.ModuleId == moduleId)).ToList();

                //kapott lista visszaadása.
                return moduleUsers;
            }
            catch (Exception ex)
            {
                return new List<User>();
                throw ex;
            }
            
        }


        //Új felhasználó hozzáadását, valamint a modulokkal való kapcsolatok megvalósítását mevalósító feladat.
        public async Task<User> AddNewUser(User newUser, List<Module> modules)
        {
            try
            {
                if (!_dbContext.Users.Any(item => item.Email == newUser.Email || item.UserName == newUser.UserName))
                {
                    _dbContext.Users.Add(newUser);
                }
                await _dbContext.SaveChangesAsync();
                var nU = await _dbContext.Users.FirstAsync(user => user.Email == newUser.Email);
                var res = await CreateUserModuleConnections(nU, modules);
                return res;
            }
            catch (Exception)
            {
                return new User();
                throw;
            }
            
        }

        //Felhasználói adatok és a hozzá kapcsolódó modulkapcsolatok módosítása.
        public async Task<User> UpdateUserDetails(User updatedUser, List<Module> modules)
        {
            try
            {
                var userToUpdate = await _dbContext.Users.FirstAsync(item => item.Id == updatedUser.Id);
                userToUpdate.UserName = updatedUser.UserName;
                userToUpdate.UserRole = updatedUser.UserRole;
                userToUpdate.FirstName = updatedUser.FirstName;
                userToUpdate.LastName = updatedUser.LastName;
                userToUpdate.Password = updatedUser.Password;
                var existingConns = await _dbContext.UserModuleConnections.Where(i=>i.UserId == updatedUser.Id).ToListAsync();
                foreach (var module in modules) 
                {
                    if (!existingConns.Any(it=>it.ModuleId == module.Id))
                    {
                        UserModuleConnection nC = new UserModuleConnection
                        {
                            UserId = updatedUser.Id,
                            ModuleId = module.Id,
                        };
                        _dbContext.UserModuleConnections.Add(nC);
                    }
                }
                await _dbContext.SaveChangesAsync();
                return userToUpdate;
            }
            catch (Exception)
            {
                return new User();
                throw;
            }
        }

        //Modul belépési adatait ellenőrző feladat.
        public async Task<bool> AuthenticateModule(EmailPassw cred)
        {
            try
            {
                var res =  await _dbContext.Users.FirstAsync(c => c.Email == cred.Email);
                if (res!=null)
                {
                    if (res.Password == cred.Password)
                    {
                        return true;
                    }
                    return false;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }


        }

        //Felhasználó és modul közötti kapcsolatot elkészítő feladat.
        public async Task<User> CreateUserModuleConnections(User newUser, List<Module> modules)
        {
            try
            {
                foreach (var module in modules)
                {
                    UserModuleConnection nC = new UserModuleConnection 
                    {
                        UserId = newUser.Id,
                        ModuleId = module.Id,
                    };
                    _dbContext.UserModuleConnections.Add(nC);
                }
                await _dbContext.SaveChangesAsync();
                return newUser;
            }
            catch (Exception)
            {
                return new User();
                throw;
            }
            
            
        }

        //minden modul és felhasználó közötti kapcsolatot visszaadó feladat.
        public async Task<IEnumerable<UserModuleConnection>> GetConnections()
        {
            return await _dbContext.UserModuleConnections.ToListAsync();
        }
    }
}
