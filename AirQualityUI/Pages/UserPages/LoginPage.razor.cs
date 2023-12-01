using AirQualityUI.Models;
using AirQualityUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AirQualityUI.Pages.UserPages
{
    public partial class LoginPage
    {
        //Felhasználó beléptetéséért felelős osztály.
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IUserService _service { get; set; }

        MudForm form;

        UserValidator userValidator = new UserValidator();

        EmailPassw user = new EmailPassw();

        bool disableSubmit = false;

        string message = "";

        protected async override Task OnInitializedAsync()
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/", true);
            }
        }

        private async Task ClickSubmit()
        {
            //Belépés gombra való kattintáskor a gomb ismételt lenyomása letiltásra kerül, valamint megjelenik felette egy prograssbar, és meghívódik a Submit fuggvény.
            disableSubmit = true;
            StateHasChanged();
            Submit();
        }

        private async Task Submit()
        {
            //Ellenőrzésre kerülnek a belépési adatok
            if (form.IsValid && user.Email != null &&user.Password != null)
            {
                var result = await _service.GetUserByEmail(user.Email);
                string encryptedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
                EmailPassw userToLogin = new EmailPassw
                {
                    Email = user.Email,
                    Password = user.Password
                };

                if (result != null)
                {
                    bool verified = BCrypt.Net.BCrypt.EnhancedVerify(userToLogin.Password, result.Password);
                    //Sikeres ellenőrzés esetén az adatok beírásra kerülnek a munkamenethez, és a beléptetés megtörténik.
                    //Ellenkező esetben hibaüzenet kerül kiírásra.
                    if (verified)
                    {
                        var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
                        await customAuthStateProvider.UpdateAuthenticationState(
                            new User
                            {
                                UserName = result.UserName,
                                FirstName = result.FirstName,
                                LastName = result.LastName,
                                Email = result.Email,
                                UserRole = result.UserRole,
                            }
                        );
                        Snackbar.Add($"Sikeresen belépett {result.UserName}!");
                        NavigationManager.NavigateTo("/", true);
                    }
                    else
                    {
                        message = "Sikertelen belépési kísérlet. \n Kérjük ellenőrizze adatait!";
                        disableSubmit = false;
                        StateHasChanged();
                    }

                }
                else
                {
                    message = "Sikertelen belépési kísérlet. \n Kérjük ellenőrizze adatait!";
                    disableSubmit = false;
                    StateHasChanged();
                }

            }
            else
            {
                message = "Sikertelen belépési kísérlet. \n Kérjük adja meg belépési adatait!";
                disableSubmit = false;
            }
        }

        public void OnPbKeyPressed(KeyboardEventArgs e)
        {
            //Függvény, a jelszó bevitelében lenyomott enter billenytűvel való beléptetés kezdeményezésének megvalósításához.
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                disableSubmit = true;
                ClickSubmit();
            }
        }

        public class UserValidator : AbstractValidator<EmailPassw>
        {
            //Beírt adatok validálására szolgáló osztály.
            public UserValidator()
            {
                RuleFor(x => x.Email)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty()
                   .EmailAddress()
                   .WithName("e-mail cím mező");

                RuleFor(x => x.Password)
                   .NotEmpty()
                   .Length(8, 100)
                   .WithName("jelszó mező");

            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<EmailPassw>.CreateWithOptions((EmailPassw)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
