using AirQualityUI.Models;
using AirQualityUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AirQualityUI.Pages.UserPages
{
    public partial class UpdateUserDetailsPage
    {
        //Felhasználói adatok módosítására szolgáló osztály.
        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        IUserService _service { get; set; }

        [Inject]
        IModuleService _ModuleService { get; set; }

        MudForm form;

        UserValidator userValidator = new UserValidator();

        List<User> users { get; set; }

        User SelectedUser = new User();
        string oldpassword = "";
        bool loadingDatas = true;
        bool submitDisabled = false;
        bool userIsSelected = false;
        string message = "";

        string SelectedRole = "user";

        List<Module> modules { get; set; }
        IEnumerable<Module> selectedModules { get; set; } = new HashSet<Module>();

        protected async override Task OnInitializedAsync()
        {

            modules = await _ModuleService.GetModules();
            var usrs = await _service.GetUsers();
            users = usrs.ToList();
            if (modules.Count > 0 && users.Count > 0)
            {
                loadingDatas = false;
            }

            await base.OnInitializedAsync();


        }

        public async Task onUserSelectionChanged()
        {
            //Amennyiben kiválastzásra kerül egy felhasználó, az adatai betöltődnek az űrlapba, a jelszót kivéve, ami a háttérben kerül eltárolásra.
            if (SelectedUser.Id != 0)
            {
                oldpassword = SelectedUser.Password;
                SelectedUser.Password = "";
                userIsSelected = true;
                SelectedRole = SelectedUser.UserRole;
                selectedModules = await _ModuleService.GetModulesByUserId(SelectedUser.Id);
                StateHasChanged();
            }

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
            //A mentés gombra való kattintás után ellenőrzésre kerül, hogy más felhasználónál nincs-e használatban az email cím és a felhasználónév, majd validáció következik.
            var moduleList = selectedModules.ToList();
            var res = await _service.GetUserByEmail(SelectedUser.Email);
            var resu = await _service.GetUserByUsername(SelectedUser.UserName);
            if (moduleList.Count < 1)
            {
                message = "Nincs modul kiválasztva!";
                StateHasChanged();
                return;
            }
            else if (res != null && resu != null)
            {
                if (res.Id != SelectedUser.Id && resu.Id != SelectedUser.Id)
                {
                    message = "A megadott e-mail cím és felhasználónév már foglalt!";
                    StateHasChanged();
                    return;
                }
            }
            else if (res != null)
            {
                if (res.Id != SelectedUser.Id)
                {
                    message = "A megadott e-mail cím már foglalt!";
                    StateHasChanged();
                    return;
                }
            }
            else if (resu != null)
            {
                if (resu.Id != SelectedUser.Id)
                {
                    message = "A megadott felhasználónév cím már foglalt!";
                    StateHasChanged();
                    return;
                }
            }

            await form.Validate();

            if (form.IsValid && moduleList.Count > 0)
            {
                //sikeres validáció esetén ellenőrzésre kerül, hogy volt-e jelszó megadva. Amennyiben nem,
                //a régi jelszó kerül eltárolásra az adatbázisba, amennyiben volt, az kódolásra, majd mentésre kerül.
                if (SelectedUser.Password != "")
                {
                    SelectedUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(SelectedUser.Password, 13);
                }
                else
                {
                    SelectedUser.Password = oldpassword;
                }
                User userToUpdate = new User
                {
                    Id = SelectedUser.Id,
                    UserName = SelectedUser.UserName,
                    FirstName = SelectedUser.FirstName,
                    LastName = SelectedUser.LastName,
                    Email = SelectedUser.Email,
                    Password = SelectedUser.Password,
                    CreateDate = DateTime.Now.ToString(),
                    UserRole = SelectedRole
                };
                var result = await _service.UpdateUserDetails(userToUpdate, moduleList);
                if (result != null)
                {
                    Snackbar.Add($"Sikeres adatmódosítás {result.UserName} számára!");
                    NavigationManager.NavigateTo("/updateUserDetails", true);
                }
                else
                {
                    Snackbar.Add($"Sikertelen módosítás!");
                }
            }
        }

        public class UserValidator : AbstractValidator<User>
        {
            //Megadott adatok validációját végző osztály.
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

                RuleFor(x => x.Password)
                    .Length(8, 100)
                    .WithName("jelszó mező")
                    .Unless(x => x.Password == "");

                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress()
                    .WithName("email cím mező");
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
