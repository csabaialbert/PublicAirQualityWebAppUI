using AirQualityUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using AirQualityUI.Models;
using MudBlazor;

namespace AirQualityUI.Pages.ModulePages
{
    //Modul adatainak módosítására szolgáló osztály.
    public partial class ModifyModule
    {
        [Inject]
        ISnackbar Snackbar { get; set; }

        [Inject]
        IModuleService _ModuleService { get; set; }

        MudForm form;

        bool loadingDatas = true;
        bool submitDisabled = false;
        bool moduleIsSelected = false;

        ModuleValidator moduleValidator = new ModuleValidator();

        Module module = new Module();
        List<Module> modules { get; set; }
        string message = "";


        protected async override Task OnInitializedAsync()
        {
            modules = await _ModuleService.GetModules();
            if (modules.Count > 0)
            {
                loadingDatas = false;
            }
            await base.OnInitializedAsync();


        }

        public async Task onModuleSelectionChanged()
        {
            //Modul kiválasztásakor a moduleIsSelected boolean típusú változó igaz értéket kap, és a kiválasztott modul adatai betöltődnek a megjelenő űrlapba.
            if (module.Id != 0)
            {
                moduleIsSelected = true;
                StateHasChanged();
            }

        }

        private async Task Submit()
        {
            //mentés gomb megnyomása után ellenőrzésre kerülnek a beírt adatok, amennyiben megfelelőek, módosításra kerül az adott rekord az adatbázisban.
            await form.Validate();

            if (form.IsValid)
            {
                Module newModule = new Module
                {
                    Id = module.Id,
                    ModuleName = module.ModuleName,
                };
                var result = await _ModuleService.PutModifiedModule(newModule);
                if (result != null)
                {
                    Snackbar.Add($"Modul sikeresen módosítva!");
                    NavigationManager.NavigateTo("/modifyModule", true);
                }
                else
                {
                    message = "Modul módosítása sikertelen!";
                    Snackbar.Add($"Modul módosítása sikertelen!");
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
