using AirQualityUI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AirQualityUI.Services
{
    //Az AuthenticationStateProvider beépített csomag felülírt funkcióit tartalmazó osztály.
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymus = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() 
        {
            //Az adott munkamenetben jelenlegi autentikációs státusz lekérésére szolgáló feladat.
            //Ellenőrzi, hogy be vannak-e írva a belépett felhasználó adatai, amennyiben igen, vissza is adja azokat ClaimsPrincipal objektumként.
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<User>("user");
                var user = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (user == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymus));
                }
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim (ClaimTypes.Name, user.UserName),
                    new Claim (ClaimTypes.Surname, user.LastName),
                    new Claim (ClaimTypes.GivenName, user.FirstName),
                    new Claim (ClaimTypes.Email, user.Email),
                    new Claim (ClaimTypes.Role, user.UserRole),
                }, "CustomAuth"));
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception)
            {
                return await Task.FromResult(new AuthenticationState(_anonymus));
            }
        }

        public async Task UpdateAuthenticationState(User user) 
        {
            //autentikációs státusz frissítésére szolgáló feladat.
            //amennyiben átadásra kerülnek a felhasználói adatok, azok beírásra kerülnek, és megtörténik a felhasználó beléptetése.
            //abban az esetben, hogyha nem kerülnek átadásra adatok, a belépett felhasznélói adatok kitörlésre kerülnek és megtörténik a kijelentkeztetés.
            ClaimsPrincipal claimsPrincipal;
            if (user != null)
            {
                await _sessionStorage.SetAsync("user", user);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim (ClaimTypes.Name, user.UserName),
                    new Claim (ClaimTypes.Surname, user.LastName),
                    new Claim (ClaimTypes.GivenName, user.FirstName),
                    new Claim (ClaimTypes.Email, user.Email),
                    new Claim (ClaimTypes.Role, user.UserRole),
                }));
            }
            else
            {
                await _sessionStorage.DeleteAsync("user");
                claimsPrincipal = _anonymus;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
