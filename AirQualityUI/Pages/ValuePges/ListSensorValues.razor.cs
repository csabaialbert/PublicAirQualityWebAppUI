using AirQualityUI.Models;
using AirQualityUI.Services;
using AirQualityUI.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Extensions;

namespace AirQualityUI.Pages.ValuePges
{
    public partial class ListSensorValues
    {
        //Egy kiválasztott modul, megadott időintervallumon belül rögzített értékeinek kilistázásához szükséges függvényeket tartalmazó osztály.
        [Inject]
        ISensorValueService _SensorService { get; set; }

        [Inject]
        IModuleService _ModuleService { get; set; }

        [Inject]
        AuthenticationStateProvider authStateProvider { get; set; }

        bool loadingDatas = true;

        bool disableSubmit = true;

        IEnumerable<SensorValue> sensorValues { get; set; }
        List<SensorValue> valuesForSelection { get; set; } = new List<SensorValue>();
        List<Module> modules { get; set; }
        Module selectedModule { get; set; } = new Module();

        public MudBlazor.DateRange _dateRange = new MudBlazor.DateRange(DateTime.Now.Date, DateTime.Now.Date);

        public TimeSpan? startTime = new TimeSpan(00, 00, 00);
        public TimeSpan? endTime = new TimeSpan(00, 00, 00);

        protected async override Task OnInitializedAsync() 
        {
            //az oldal betöltésekor ellenőrzésre kerül a felhasználó jogosultsága, az alapján kerülnek beírásra a legördülő listába az elemek.
            var authState = await  authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            sensorValues = await _SensorService.GetValues();
            if (user.IsInRole("admin"))
            {
                modules = await _ModuleService.GetModules();
            }
            else if (user.IsInRole("user"))
            {
                modules = await _ModuleService.GetModulesByUserName(user.Identity.Name);
            }
            
            await base.OnInitializedAsync();

            if (modules.Count > 0)
            {
                loadingDatas = false;
            }
        }

        public void onModuleSelectionChanged()
        {
            //Modul kiválasztása után engedélyezésre kerül a listázás gomb megnoymása.
            disableSubmit = false;
        }

        public void ListValues() 
        {
            //Kritériumoknak megfelelő adatok lekérdezése az adatbázisból.
            valuesForSelection = new List<SensorValue>();
            var dtrStart = _dateRange.Start.ToIsoDateString();
            var dtrEnd = _dateRange.End.ToIsoDateString();
            var tsStart = startTime.ToString();
            var tsEnd = endTime.ToString();
            string start = dtrStart + " " + tsStart;
            string end = dtrEnd + " " + tsEnd;
            try
            {
                DateTime startDateTime = DateTime.ParseExact(start, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact(end, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                valuesForSelection = sensorValues.Where(item => item.ModuleId == selectedModule.Id && item.ReadDate >= startDateTime && item.ReadDate <= endDateTime).ToList();
                disableSubmit = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {

                throw;
            }
            

           
        }
    }
}
