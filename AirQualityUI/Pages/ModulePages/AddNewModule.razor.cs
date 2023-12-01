using AirQualityUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using AirQualityUI.Models;
using MudBlazor;

namespace AirQualityUI.Pages.ModulePages
{
    //Új modul hozzáadásához szükséges üggvényeket tartalmazó osztály.
    public partial class AddNewModule
    {
        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        IModuleService _ModuleService { get; set; }


        MudForm form;

        bool loadingDatas = true;
        bool submitDisabled = false;

        ModuleValidator moduleValidator = new ModuleValidator();

        Module module = new Module();

        string message = "";


        protected async override Task OnInitializedAsync()
        {

            loadingDatas = false;
            await base.OnInitializedAsync();


        }

        private async Task Submit()
        {
            //Létrehozás gomb megnyomásakor ellenőrzésre kerül, hogy van-e már ilyen névvel létrehozott modul, amennyiben nincs, hozzáadásra kerül.

            var res = await _ModuleService.GetModuleByName(module.ModuleName);
            if (res != null)
            {
                message = "A megadott modulnév már foglalt!";
                StateHasChanged();
                return;
            }

            await form.Validate();

            if (form.IsValid)
            {
                var result = await _ModuleService.PostNewModule(module);
                if (result != null)
                {
                    Snackbar.Add($"Modul sikeresen létrehozva!");
                    NavigationManager.NavigateTo("/addNewModule", true);
                }
                else
                {
                    message = "Modul létrehozása sikertelen!";
                    Snackbar.Add($"Modul létrehozása sikertelen!");
                }
            }
        }

        public class ModuleValidator : AbstractValidator<Module>
        {
            //Validáció elvégzésére szolgáló osztály.
            public ModuleValidator()
            {
                RuleFor(x => x.ModuleName)
                    .NotEmpty()
                    .Length(5, 20)
                    .WithName("modul név");

            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<Module>.CreateWithOptions((Module)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
