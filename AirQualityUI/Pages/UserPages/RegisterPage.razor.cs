using AirQualityUI.Models;
using AirQualityUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AirQualityUI.Pages.UserPages
{
    public partial class RegisterPage
    {
        //Az új felhasználó létrehozására szükséges függvényeket tartalmazó osztály.
        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        IUserService _service { get; set; }

        [Inject]
        IModuleService _ModuleService { get; set; }


        MudForm form;
        bool loadingDatas = true;
        bool submitDisabled = false;
        UserValidator userValidator = new UserValidator();
        User user = new User();
        string message = "";
        string SelectedRole = "user";

        List<Module> modules { get; set; }
        IEnumerable<Module> selectedModules { get; set; } = new HashSet<Module>();

        protected async override Task OnInitializedAsync()
        {

            modules = await _ModuleService.GetModules();
            if (modules.Count > 0)
            {
                loadingDatas = false;
            }

            await base.OnInitializedAsync();


        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            //Modul(-ok) kiválasztásakor visszaadja, hogy helenleg hány elem került kiválasztásra, és a visszaadott szöveg jelenik meg.
            if (selectedValues.Count > 0)
            {
                return $"{selectedValues.Count} modul került kiválasztásra.";
            }
            return $"Nincs modul kiválasztva.";
        }

        private async Task Submit()
        {
            //A mentés gombra való kattintáskor lefutó feladat, ami ellenőrzi, hogy a beírt email cím és felhasználónév nincs -e már használatban. 
            var moduleList = selectedModules.ToList();
            var res = await _service.GetUserByEmail(user.Email);
            var resu = await _service.GetUserByUsername(user.UserName);
            if (moduleList.Count < 1)
            {
                message = "Kérjük válasszon modult!";
                StateHasChanged();
                return;
            }
            else if (res != null && resu != null)
            {
                message = "A megadott e-mail cím és felhasználónév már foglalt!";
                StateHasChanged();
                return;
            }
            else if (res != null)
            {
                message = "A megadott e-mail cím már foglalt!";
                StateHasChanged();
                return;
            }
            else if (resu != null)
            {
                message = "A megadott felhasználónév cím már foglalt!";
                StateHasChanged();
                return;
            }

            //Abban az esetben, hogyha egyedi adatokat adtak meg, lefut a validáció, majd sikeres ellenőrzés után kódolásra kerül a megadott jelszó,
            //és az adatok beírásra kerülnek az adatbázisba.
            await form.Validate();

            if (form.IsValid && moduleList.Count > 0)
            {
                string encryptedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
                User userToRegister = new User
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = encryptedPass,
                    CreateDate = DateTime.Now.ToString(),
                    UserRole = SelectedRole
                };
                var result = await _service.AddNewUser(userToRegister, moduleList);
                if (result != null)
                {
                    //_datas.loggedInUser = await _service.GetUserByEmail(user.Email);

                    Snackbar.Add($"Sikeres regisztráció {result.UserName} számára!");
                    NavigationManager.NavigateTo("/", true);
                }
                else
                {
                    message = "Már létezik felhasználó a megadott e-mail címmel vagy felhasználónévvel!";
                    Snackbar.Add($"Sikertelen regisztráció!");
                }
            }
        }

        public class UserValidator : AbstractValidator<User>
        {
            //Megadott adatok validációját végző osztály. 
            [Inject]
            IUserService _service { get; set; }
            public UserValidator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .Length(5, 20)
                    .WithName("felhasználónév mező");

                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .Length(5, 30)
                    .WithName("keresztnév mező");

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .Length(5, 30)
                    .WithName("vezetéknév mező");

                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress()
                    .WithName("email cím mező");

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithName("jelszó mező")
                    .Length(8, 100);

            }


            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
