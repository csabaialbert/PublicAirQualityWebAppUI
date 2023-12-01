using AirQualityUI.Data;
using AirQualityUI.Models;
using AirQualityUI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;
using System.Linq;

namespace AirQualityUI.Pages.ValuePges
{
    public partial class ListAndGraphForLastWeek
    {
        //Elmúlt heti adatok betöltésére szolgáló osztály.
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

        private readonly List<ChartSeries> _TempHumiditySeries = new();
        public double[] temperatureData = Array.Empty<double>();
        public double[] humidityData = Array.Empty<double>();
        public string[] tempHumidityXaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _options = new();

        public MudBlazor.DateRange _dateRange = new MudBlazor.DateRange(DateTime.Now.Date, DateTime.Now.Date);

        public TimeSpan? startTime = new TimeSpan(00, 00, 00);
        public TimeSpan? endTime = new TimeSpan(00, 00, 00);

        protected async override Task OnInitializedAsync()
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
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
            //loadingDatas = false;
        }

        public async void onModuleSelectionChanged()
        {
            //Modul kiválasztása után, minden napra külön-külön listába helyezésre kerülnek az adatok.
            //A listákból pedig a ksizámított átlagok átadásra kerülnek a diagram adatait tartalmazó listának.
            _TempHumiditySeries.Clear();
            if (selectedModule.Id != 0)
            {
                disableSubmit = false;
                var lastweek = _SensorService.LastWeek(DateTime.Now);
                valuesForSelection = await _SensorService.GetLastWeekValuesForModuleId(selectedModule.Id);

                var FirstDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.ToString().Split(" ")[0]+ " " + lastweek.Start.ToString().Split(" ")[1] + " " + lastweek.Start.ToString().Split(" ")[2])).Select(i=>i.Temperature).ToList();
                var FirstDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.ToString().Split(" ")[0] + " " + lastweek.Start.ToString().Split(" ")[1] + " " + lastweek.Start.ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var SecondDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(1).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(1).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(1).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var SecondDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(1).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(1).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(1).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var ThirdDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(2).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(2).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(2).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var ThirdDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(2).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(2).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(2).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var FourthDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(3).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(3).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(3).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var FourthDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(3).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(3).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(3).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var FifthDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(4).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(4).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(4).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var FifthDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(4).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(4).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(4).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var SixthDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(5).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(5).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(5).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var SixthDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(5).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(5).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(5).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();
                var SeventhDayTemp = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(6).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(6).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(6).ToString().Split(" ")[2])).Select(i => i.Temperature).ToList();
                var SeventhDayHum = valuesForSelection.Where(item => item.ReadDate.ToString().Contains(lastweek.Start.AddDays(6).ToString().Split(" ")[0] + " " + lastweek.Start.AddDays(6).ToString().Split(" ")[1] + " " + lastweek.Start.AddDays(6).ToString().Split(" ")[2])).Select(i => i.Humidity).ToList();

                temperatureData = temperatureData.Append((FirstDayTemp.Count()!=0 ? Convert.ToDouble(FirstDayTemp.Average()) : 0)).ToArray();
                temperatureData = temperatureData.Append(SecondDayTemp.Count() !=0 ? Convert.ToDouble(SecondDayTemp.Average()) : 0).ToArray();
                temperatureData = temperatureData.Append(ThirdDayTemp.Count() != 0 ? Convert.ToDouble(ThirdDayTemp.Average()) : 0).ToArray();
                temperatureData = temperatureData.Append(FourthDayTemp.Count() != 0 ? Convert.ToDouble(FourthDayTemp.Average()) : 0).ToArray();
                temperatureData = temperatureData.Append(FifthDayTemp.Count() != 0 ? Convert.ToDouble(FifthDayTemp.Average()) : 0).ToArray();
                temperatureData = temperatureData.Append(SixthDayTemp.Count() != 0 ? Convert.ToDouble(SixthDayTemp.Average()) : 0).ToArray();
                temperatureData = temperatureData.Append(SeventhDayTemp.Count() != 0 ? Convert.ToDouble(SeventhDayTemp.Average()) : 0).ToArray();

                humidityData = humidityData.Append(FirstDayHum.Count() != 0 ? Convert.ToDouble(FirstDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(SecondDayHum.Count() != 0 ? Convert.ToDouble(SecondDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(ThirdDayHum.Count() != 0 ? Convert.ToDouble(ThirdDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(FourthDayHum.Count() != 0 ? Convert.ToDouble(FourthDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(FifthDayHum.Count() != 0 ? Convert.ToDouble(FifthDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(SixthDayHum.Count() != 0 ? Convert.ToDouble(SixthDayHum.Average()) : 0).ToArray();
                humidityData = humidityData.Append(SeventhDayHum.Count() != 0 ? Convert.ToDouble(SeventhDayHum.Average()) : 0).ToArray();


                for (int i = 0; i < 7; i++)
                {
                    //Az x-tengelyen megjelenő adatok átadásra kerülnek (az elmúlt hét napjai).
                    string date = Convert.ToString(lastweek.Start.AddDays(i)).Split(" ")[0] + Convert.ToString(lastweek.Start.AddDays(i)).Split(" ")[1] + Convert.ToString(lastweek.Start.AddDays(i)).Split(" ")[2];
                    tempHumidityXaxisLbl = tempHumidityXaxisLbl.Append(date).ToArray();
                }
                //A számított adatok mellett még az adatokat ábrázoló diagram megnevezése is átadásra kerül. 
                _TempHumiditySeries.Add(new ChartSeries { Name = "Hőmérséklet [°C]", Data = temperatureData });
                _TempHumiditySeries.Add(new ChartSeries { Name = "Páratartalom [%]", Data = humidityData });
                _options.LineStrokeWidth = 10;

                StateHasChanged();
            }
            
        }

        public void ListValues()
        {
            
        }
    }
}
