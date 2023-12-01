using AirQualityUI.Services;

namespace AirQualityUI.Pages.UserPages
{
    public partial class LogoutPage
    {
        protected override void OnInitialized()
        {
            Logout();
        }
        public async Task Logout()
        {
            //A belépett felhasználó adatainak kitörlésére szolgáló feladat.
            var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            await customAuthStateProvider.UpdateAuthenticationState(null);
            NavigationManager.NavigateTo("/loggedOut", true);
        }
    }
}
