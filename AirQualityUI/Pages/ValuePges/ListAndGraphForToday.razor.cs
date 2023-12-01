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
    public partial class ListAndGraphForToday
    {
        //Szenzor értékek megjelenítéséhez szükséges műveleteket végző függvényeket tartalmazó osztály. 
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

        //napi átlagokat megjelenítő diagramhoz szükséges tulajdonságok deklarálása.
        private readonly List<ChartSeries> _TempHumiditySeries = new();
        public double[] temperatureData = Array.Empty<double>();
        public double[] humidityData = Array.Empty<double>();
        public string[] tempHumidityXaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _thoptions = new();

        private readonly List<ChartSeries> _Mq2Series = new();
        public double[] mq2RawData = Array.Empty<double>();
        public double[] mq2LpgData = Array.Empty<double>();
        public double[] mq2CoData = Array.Empty<double>();
        public double[] mq2SmokeData = Array.Empty<double>();
        public double[] mq2PropaneData = Array.Empty<double>();
        public double[] mq2H2Data = Array.Empty<double>();
        public double[] mq2AlcoholData = Array.Empty<double>();
        public double[] mq2Ch4Data = Array.Empty<double>();
        public string[] mq2XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq2options = new();

        private readonly List<ChartSeries> _Mq3Series = new();
        public double[] mq3RawData = Array.Empty<double>();
        public double[] mq3AlcoholData = Array.Empty<double>();
        public double[] mq3BenzineData = Array.Empty<double>();
        public double[] mq3ExaneData = Array.Empty<double>();
        public double[] mq3LpgData = Array.Empty<double>();
        public double[] mq3CoData = Array.Empty<double>();
        public string[] mq3XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq3options = new();

        private readonly List<ChartSeries> _Mq4Series = new();
        public double[] mq4RawData = Array.Empty<double>();
        public double[] mq4Ch4Data = Array.Empty<double>();
        public double[] mq4LpgData = Array.Empty<double>();
        public double[] mq4H2Data = Array.Empty<double>();
        public double[] mq4SmokeData = Array.Empty<double>();
        public double[] mq4AlcoholData = Array.Empty<double>();
        public double[] mq4CoData = Array.Empty<double>();
        public string[] mq4XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq4options = new();

        private readonly List<ChartSeries> _Mq135Series = new();
        public double[] mq135RawData = Array.Empty<double>();
        public double[] mq135AcetonData = Array.Empty<double>();
        public double[] mq135ToluenoData = Array.Empty<double>();
        public double[] mq135AlcoholData = Array.Empty<double>();
        public double[] mq135Co2Data = Array.Empty<double>();
        public double[] mq135Nh4Data = Array.Empty<double>();
        public double[] mq135CoData = Array.Empty<double>();
        public string[] mq135XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq135options = new();

        private readonly List<ChartSeries> _Mq6Series = new();
        public double[] mq6RawData = Array.Empty<double>();
        public double[] mq6LpgData = Array.Empty<double>();
        public double[] mq6Ch4Data = Array.Empty<double>();
        public double[] mq6H2Data = Array.Empty<double>();
        public double[] mq6AlcoholData = Array.Empty<double>();
        public double[] mq6CoData = Array.Empty<double>();
        public string[] mq6XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq6options = new();

        private readonly List<ChartSeries> _Mq7Series = new();
        public double[] mq7RawData = Array.Empty<double>();
        public double[] mq7H2Data = Array.Empty<double>();
        public double[] mq7CoData = Array.Empty<double>();
        public double[] mq7LpgData = Array.Empty<double>();
        public double[] mq7Ch4Data = Array.Empty<double>();
        public double[] mq7AlcoholData = Array.Empty<double>();
        public string[] mq7XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq7options = new();

        private readonly List<ChartSeries> _Mq8Series = new();
        public double[] mq8RawData = Array.Empty<double>();
        public double[] mq8H2Data = Array.Empty<double>();
        public double[] mq8AlcoholData = Array.Empty<double>();
        public double[] mq8LpgData = Array.Empty<double>();
        public double[] mq8Ch4Data = Array.Empty<double>();
        public double[] mq8CoData = Array.Empty<double>();
        public string[] mq8XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq8options = new();

        private readonly List<ChartSeries> _Mq9Series = new();
        public double[] mq9RawData = Array.Empty<double>();
        public double[] mq9CoData = Array.Empty<double>();
        public double[] mq9LpgData = Array.Empty<double>();
        public double[] mq9Ch4Data = Array.Empty<double>();
        public string[] mq9XaxisLbl = Array.Empty<string>();
        private readonly ChartOptions _mq9options = new();

        public MudBlazor.DateRange _dateRange = new MudBlazor.DateRange(DateTime.Now.Date, DateTime.Now.Date);

        public TimeSpan? startTime = new TimeSpan(00, 00, 00);
        public TimeSpan? endTime = new TimeSpan(00, 00, 00);

        protected async override Task OnInitializedAsync()
        {
            //az oldal betöltésekor ellenőrzésre kerül a felhasználó jogosultsága, az alapján kerülnek beírásra a legördülő listába az elemek.
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
        }

        public async void onModuleSelectionChanged()
        {
            if (selectedModule.Id != 0)
            {
                _TempHumiditySeries.Clear();
                _Mq2Series.Clear();
                _Mq3Series.Clear();
                _Mq4Series.Clear();
                _Mq135Series.Clear();
                _Mq6Series.Clear();
                _Mq7Series.Clear();
                _Mq8Series.Clear();
                _Mq9Series.Clear();
                disableSubmit = false;
                valuesForSelection = await _SensorService.GetTodayValuesForModuleId(selectedModule.Id);

                for (int i = 0; i < 24; i++)
                {
                    //Napi adatok külön-külön listába rendezése, tulajdonságokként.
                    var temp = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Temperature).ToList();
                    var hum = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Humidity).ToList();

                    var mq2Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Raw).ToList();
                    var mq2Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Lpg).ToList();
                    var mq2Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Co).ToList();
                    var mq2Smoke = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Smoke).ToList();
                    var mq2Propane = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Propane).ToList();
                    var mq2H2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2H2).ToList();
                    var mq2Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Alcohol).ToList();
                    var mq2Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq2Ch4).ToList();

                    var mq3Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Raw).ToList();
                    var mq3Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Alcohol).ToList();
                    var mq3Benzine = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Benzine).ToList();
                    var mq3Exane = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Exane).ToList();
                    var mq3Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Lpg).ToList();
                    var mq3Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq3Co).ToList();

                    var mq4Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Raw).ToList();
                    var mq4Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Ch4).ToList();
                    var mq4Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Lpg).ToList();
                    var mq4H2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4H2).ToList();
                    var mq4Smoke = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Smoke).ToList();
                    var mq4Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Alcohol).ToList();
                    var mq4Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq4Co).ToList();

                    var mq135Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Raw).ToList();
                    var mq135Aceton = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Aceton).ToList();
                    var mq135Tolueno = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Tolueno).ToList();
                    var mq135Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Alcohol).ToList();
                    var mq135Co2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Co2).ToList();
                    var mq135Nh4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Nh4).ToList();
                    var mq135Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq135Co).ToList();

                    var mq6Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6Raw).ToList();
                    var mq6Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6Lpg).ToList();
                    var mq6Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6Ch4).ToList();
                    var mq6H2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6H2).ToList();
                    var mq6Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6Alcohol).ToList();
                    var mq6Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq6Co).ToList();

                    var mq7Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7Raw).ToList();
                    var mq7H2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7H2).ToList();
                    var mq7Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7Co).ToList();
                    var mq7Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7Lpg).ToList();
                    var mq7Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7Ch4).ToList();
                    var mq7Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq7Alcohol).ToList();

                    var mq8Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8Raw).ToList();
                    var mq8H2 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8H2).ToList();
                    var mq8Alcohol = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8Alcohol).ToList();
                    var mq8Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8Lpg).ToList();
                    var mq8Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8Ch4).ToList();
                    var mq8Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq8Co).ToList();

                    var mq9Raw = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq9Raw).ToList();
                    var mq9Co = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq9Co).ToList();
                    var mq9Lpg = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq9Lpg).ToList();
                    var mq9Ch4 = valuesForSelection.Where(item => item.ReadDate.ToString().Split(" ")[3].Split(":")[0] == i.ToString()).Select(it => it.Mq9Ch4).ToList();
                    
                    if (temp.Count > 0)
                    {
                        //értékek alapján átlagok kiszámítása
                        temperatureData = temperatureData.Append(Convert.ToDouble(temp.Average())).ToArray();
                        humidityData = humidityData.Append(Convert.ToDouble(hum.Average())).ToArray();

                        mq2RawData = mq2RawData.Append(Convert.ToDouble(mq2Raw.Average())).ToArray();
                        mq2LpgData = mq2LpgData.Append(Convert.ToDouble(mq2Lpg.Average())).ToArray();
                        mq2CoData = mq2CoData.Append(Convert.ToDouble(mq2Co.Average())).ToArray();
                        mq2SmokeData = mq2SmokeData.Append(Convert.ToDouble(mq2Smoke.Average())).ToArray();
                        mq2PropaneData = mq2PropaneData.Append(Convert.ToDouble(mq2Propane.Average())).ToArray();
                        mq2H2Data = mq2H2Data.Append(Convert.ToDouble(mq2H2.Average())).ToArray();
                        mq2AlcoholData = mq2AlcoholData.Append(Convert.ToDouble(mq2Alcohol.Average())).ToArray();
                        mq2Ch4Data = mq2Ch4Data.Append(Convert.ToDouble(mq2Ch4.Average())).ToArray();

                        mq3RawData = mq3RawData.Append(Convert.ToDouble(mq3Raw.Average())).ToArray();
                        mq3AlcoholData = mq3AlcoholData.Append(Convert.ToDouble(mq3Alcohol.Average())).ToArray();
                        mq3BenzineData = mq3BenzineData.Append(Convert.ToDouble(mq3Benzine.Average())).ToArray();
                        mq3ExaneData = mq3ExaneData.Append(Convert.ToDouble(mq3Exane.Average())).ToArray();
                        mq3LpgData = mq3LpgData.Append(Convert.ToDouble(mq3Lpg.Average())).ToArray();
                        mq3CoData = mq3CoData.Append(Convert.ToDouble(mq3Co.Average())).ToArray();

                        mq4RawData = mq4RawData.Append(Convert.ToDouble(mq4Raw.Average())).ToArray();
                        mq4Ch4Data = mq4Ch4Data.Append(Convert.ToDouble(mq4Ch4.Average())).ToArray();
                        mq4LpgData = mq4LpgData.Append(Convert.ToDouble(mq4Lpg.Average())).ToArray();
                        mq4H2Data = mq4H2Data.Append(Convert.ToDouble(mq4H2.Average())).ToArray();
                        mq4SmokeData = mq4SmokeData.Append(Convert.ToDouble(mq4Smoke.Average())).ToArray();
                        mq4AlcoholData = mq4AlcoholData.Append(Convert.ToDouble(mq4Alcohol.Average())).ToArray();
                        mq4CoData = mq4CoData.Append(Convert.ToDouble(mq4Co.Average())).ToArray();

                        mq135RawData = mq135RawData.Append(Convert.ToDouble(mq135Raw.Average())).ToArray();
                        mq135AcetonData = mq135AcetonData.Append(Convert.ToDouble(mq135Aceton.Average())).ToArray();
                        mq135ToluenoData = mq135ToluenoData.Append(Convert.ToDouble(mq135Tolueno.Average())).ToArray();
                        mq135AlcoholData = mq135AlcoholData.Append(Convert.ToDouble(mq135Alcohol.Average())).ToArray();
                        mq135Co2Data = mq135Co2Data.Append(Convert.ToDouble(mq135Co2.Average())).ToArray();
                        mq135Nh4Data = mq135Nh4Data.Append(Convert.ToDouble(mq135Nh4.Average())).ToArray();
                        mq135CoData = mq135CoData.Append(Convert.ToDouble(mq135Co.Average())).ToArray();

                        mq6RawData = mq6RawData.Append(Convert.ToDouble(mq6Raw.Average())).ToArray();
                        mq6LpgData = mq6LpgData.Append(Convert.ToDouble(mq6Lpg.Average())).ToArray();
                        mq6Ch4Data = mq6Ch4Data.Append(Convert.ToDouble(mq6Ch4.Average())).ToArray();
                        mq6H2Data = mq6H2Data.Append(Convert.ToDouble(mq6H2.Average())).ToArray();
                        mq6AlcoholData = mq6AlcoholData.Append(Convert.ToDouble(mq6Alcohol.Average())).ToArray();
                        mq6CoData = mq6CoData.Append(Convert.ToDouble(mq6Co.Average())).ToArray();

                        mq7RawData = mq7RawData.Append(Convert.ToDouble(mq7Raw.Average())).ToArray();
                        mq7H2Data = mq7H2Data.Append(Convert.ToDouble(mq7H2.Average())).ToArray();
                        mq7CoData = mq7CoData.Append(Convert.ToDouble(mq7Co.Average())).ToArray();
                        mq7LpgData = mq7LpgData.Append(Convert.ToDouble(mq7Lpg.Average())).ToArray();
                        mq7Ch4Data = mq7Ch4Data.Append(Convert.ToDouble(mq7Ch4.Average())).ToArray();
                        mq7AlcoholData = mq7AlcoholData.Append(Convert.ToDouble(mq7Alcohol.Average())).ToArray();

                        mq8RawData = mq8RawData.Append(Convert.ToDouble(mq8Raw.Average())).ToArray();
                        mq8H2Data = mq8H2Data.Append(Convert.ToDouble(mq8H2.Average())).ToArray();
                        mq8AlcoholData = mq8AlcoholData.Append(Convert.ToDouble(mq8Alcohol.Average())).ToArray();
                        mq8LpgData = mq8LpgData.Append(Convert.ToDouble(mq8Lpg.Average())).ToArray();
                        mq8Ch4Data = mq8Ch4Data.Append(Convert.ToDouble(mq8Ch4.Average())).ToArray();
                        mq8CoData = mq8CoData.Append(Convert.ToDouble(mq8Co.Average())).ToArray();

                        mq9RawData = mq9RawData.Append(Convert.ToDouble(mq9Raw.Average())).ToArray();
                        mq9CoData = mq9CoData.Append(Convert.ToDouble(mq9Co.Average())).ToArray();
                        mq9LpgData = mq9LpgData.Append(Convert.ToDouble(mq9Lpg.Average())).ToArray();
                        mq9Ch4Data = mq9Ch4Data.Append(Convert.ToDouble(mq9Ch4.Average())).ToArray();
                    }
                    else
                    {
                        //Abban az esetben, hogyha az adott órában nincs rögzített adat, nulla kerül beírásra.
                        temperatureData = temperatureData.Append(0).ToArray();
                        humidityData = humidityData.Append(0).ToArray();

                        mq2RawData = mq2RawData.Append(0).ToArray();
                        mq2LpgData = mq2LpgData.Append(0).ToArray();
                        mq2CoData = mq2CoData.Append(0).ToArray();
                        mq2SmokeData = mq2SmokeData.Append(0).ToArray();
                        mq2PropaneData = mq2PropaneData.Append(0).ToArray();
                        mq2H2Data = mq2H2Data.Append(0).ToArray();
                        mq2AlcoholData = mq2AlcoholData.Append(0).ToArray();
                        mq2Ch4Data = mq2Ch4Data.Append(0).ToArray();

                        mq3RawData = mq3RawData.Append(0).ToArray();
                        mq3AlcoholData = mq3AlcoholData.Append(0).ToArray();
                        mq3BenzineData = mq3BenzineData.Append(0).ToArray();
                        mq3ExaneData = mq3ExaneData.Append(0).ToArray();
                        mq3LpgData = mq3LpgData.Append(0).ToArray();
                        mq3CoData = mq3CoData.Append(0).ToArray();

                        mq4RawData = mq4RawData.Append(0).ToArray();
                        mq4Ch4Data = mq4Ch4Data.Append(0).ToArray();
                        mq4LpgData = mq4LpgData.Append(0).ToArray();
                        mq4H2Data = mq4H2Data.Append(0).ToArray();
                        mq4SmokeData = mq4SmokeData.Append(0).ToArray();
                        mq4AlcoholData = mq4AlcoholData.Append(0).ToArray();
                        mq4CoData = mq4CoData.Append(0).ToArray();

                        mq135RawData = mq135RawData.Append(0).ToArray();
                        mq135AcetonData = mq135AcetonData.Append(0).ToArray();
                        mq135ToluenoData = mq135ToluenoData.Append(0).ToArray();
                        mq135AlcoholData = mq135AlcoholData.Append(0).ToArray();
                        mq135Co2Data = mq135Co2Data.Append(0).ToArray();
                        mq135Nh4Data = mq135Nh4Data.Append(0).ToArray();
                        mq135CoData = mq135CoData.Append(0).ToArray();

                        mq6RawData = mq6RawData.Append(0).ToArray();
                        mq6LpgData = mq6LpgData.Append(0).ToArray();
                        mq6Ch4Data = mq6Ch4Data.Append(0).ToArray();
                        mq6H2Data = mq6H2Data.Append(0).ToArray();
                        mq6AlcoholData = mq6AlcoholData.Append(0).ToArray();
                        mq6CoData = mq6CoData.Append(0).ToArray();

                        mq7RawData = mq7RawData.Append(0).ToArray();
                        mq7H2Data = mq7H2Data.Append(0).ToArray();
                        mq7CoData = mq7CoData.Append(0).ToArray();
                        mq7LpgData = mq7LpgData.Append(0).ToArray();
                        mq7Ch4Data = mq7Ch4Data.Append(0).ToArray();
                        mq7AlcoholData = mq7AlcoholData.Append(0).ToArray();

                        mq8RawData = mq8RawData.Append(0).ToArray();
                        mq8H2Data = mq8H2Data.Append(0).ToArray();
                        mq8AlcoholData = mq8AlcoholData.Append(0).ToArray();
                        mq8LpgData = mq8LpgData.Append(0).ToArray();
                        mq8Ch4Data = mq8Ch4Data.Append(0).ToArray();
                        mq8CoData = mq8CoData.Append(0).ToArray();

                        mq9RawData = mq9RawData.Append(0).ToArray();
                        mq9CoData = mq9CoData.Append(0).ToArray();
                        mq9LpgData = mq9LpgData.Append(0).ToArray();
                        mq9Ch4Data = mq9Ch4Data.Append(0).ToArray();

                    }
                    //X tengelyeken megjelenő aadatok megadása (0-23)
                    tempHumidityXaxisLbl = tempHumidityXaxisLbl.Append(i.ToString()).ToArray();
                    mq2XaxisLbl = mq2XaxisLbl.Append(i.ToString()).ToArray();
                    mq3XaxisLbl = mq3XaxisLbl.Append(i.ToString()).ToArray();
                    mq4XaxisLbl = mq4XaxisLbl.Append(i.ToString()).ToArray();
                    mq135XaxisLbl = mq135XaxisLbl.Append(i.ToString()).ToArray();
                    mq6XaxisLbl = mq6XaxisLbl.Append(i.ToString()).ToArray();
                    mq7XaxisLbl = mq7XaxisLbl.Append(i.ToString()).ToArray();
                    mq8XaxisLbl = mq8XaxisLbl.Append(i.ToString()).ToArray();

                }
                //diagram tulajdonságok listákba való rendezése
                _TempHumiditySeries.Add(new ChartSeries { Name = "Hőmérséklet [°C]", Data = temperatureData });
                _TempHumiditySeries.Add(new ChartSeries { Name = "Páratartalom [%]", Data = humidityData });
                _thoptions.LineStrokeWidth = 5;

                _Mq2Series.Add(new ChartSeries { Name = "Mq2 nyers érték", Data = mq2RawData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 LPG érték [ppm]", Data = mq2LpgData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 CO érték [ppm]", Data = mq2CoData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 füst érték [ppm]", Data = mq2SmokeData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 propán érték [ppm]", Data = mq2PropaneData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 hidrogén(H2) érték [ppm]", Data = mq2H2Data });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 alkohol érték [ppm]", Data = mq2AlcoholData });
                _Mq2Series.Add(new ChartSeries { Name = "Mq2 metán érték [ppm]", Data = mq2Ch4Data });
                _mq2options.LineStrokeWidth = 1;

                _Mq3Series.Add(new ChartSeries { Name = "Mq3 nyers érték", Data = mq3RawData });
                _Mq3Series.Add(new ChartSeries { Name = "Mq3 alkohol érték [ppm]", Data = mq3AlcoholData });
                _Mq3Series.Add(new ChartSeries { Name = "Mq3 benzin érték [ppm]", Data = mq3BenzineData });
                _Mq3Series.Add(new ChartSeries { Name = "Mq3 hexán érték [ppm]", Data = mq3ExaneData });
                _Mq3Series.Add(new ChartSeries { Name = "Mq3 LPG érték [ppm]", Data = mq3LpgData });
                _Mq3Series.Add(new ChartSeries { Name = "Mq2 CO érték [ppm]", Data = mq3CoData });
                _mq3options.LineStrokeWidth = 1;

                _Mq4Series.Add(new ChartSeries { Name = "Mq4 nyers érték", Data = mq4RawData });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 metán érték [ppm]", Data = mq4Ch4Data });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 LPG érték [ppm]", Data = mq4LpgData });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 hidrogén(H2) érték [ppm]", Data = mq4H2Data });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 füst érték [ppm]", Data = mq4SmokeData });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 alkohol érték [ppm]", Data = mq4AlcoholData });
                _Mq4Series.Add(new ChartSeries { Name = "Mq4 CO érték [ppm]", Data = mq4CoData });
                _mq4options.LineStrokeWidth = 1;

                _Mq135Series.Add(new ChartSeries { Name = "Mq135 nyers érték", Data = mq135RawData });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 aceton érték [ppm]", Data = mq135AcetonData });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 toluol érték [ppm]", Data = mq135ToluenoData });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 alkohol érték [ppm]", Data = mq135AlcoholData });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 CO2 érték [ppm]", Data = mq135Co2Data });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 ammónia(NH4) érték [ppm]", Data = mq135Nh4Data });
                _Mq135Series.Add(new ChartSeries { Name = "Mq135 CO érték [ppm]", Data = mq135CoData });
                _mq135options.LineStrokeWidth = 1;

                _Mq6Series.Add(new ChartSeries { Name = "Mq6 nyers érték", Data = mq6RawData });
                _Mq6Series.Add(new ChartSeries { Name = "Mq6 LPG érték [ppm]", Data = mq6LpgData });
                _Mq6Series.Add(new ChartSeries { Name = "Mq6 metán érték [ppm]", Data = mq6Ch4Data });
                _Mq6Series.Add(new ChartSeries { Name = "Mq6 hidrogén(H2) érték [ppm]", Data = mq6H2Data });
                _Mq6Series.Add(new ChartSeries { Name = "Mq6 alkohol érték [ppm]", Data = mq6AlcoholData });
                _Mq6Series.Add(new ChartSeries { Name = "Mq6 CO érték [ppm]", Data = mq6CoData });
                _mq6options.LineStrokeWidth = 1;

                _Mq7Series.Add(new ChartSeries { Name = "Mq7 nyers érték", Data = mq7RawData });
                _Mq7Series.Add(new ChartSeries { Name = "Mq7 hidrogén(H2) érték [ppm]", Data = mq7H2Data });
                _Mq7Series.Add(new ChartSeries { Name = "Mq7 CO érték [ppm]", Data = mq7CoData });
                _Mq7Series.Add(new ChartSeries { Name = "Mq7 LPG érték [ppm]", Data = mq7LpgData });
                _Mq7Series.Add(new ChartSeries { Name = "Mq7 metán érték [ppm]", Data = mq7Ch4Data });
                _Mq7Series.Add(new ChartSeries { Name = "Mq7 alkohol érték [ppm]", Data = mq7AlcoholData });
                _mq7options.LineStrokeWidth = 1;

                _Mq8Series.Add(new ChartSeries { Name = "Mq8 nyers érték", Data = mq8RawData });
                _Mq8Series.Add(new ChartSeries { Name = "Mq8 hidrogén(H2) érték [ppm]", Data = mq8H2Data });
                _Mq8Series.Add(new ChartSeries { Name = "Mq8 alkohol érték [ppm]", Data = mq8AlcoholData });
                _Mq8Series.Add(new ChartSeries { Name = "Mq8 LPG érték [ppm]", Data = mq8LpgData });
                _Mq8Series.Add(new ChartSeries { Name = "Mq8 metán érték [ppm]", Data = mq8Ch4Data });
                _Mq8Series.Add(new ChartSeries { Name = "Mq8 CO érték [ppm]", Data = mq8CoData });
                _mq8options.LineStrokeWidth = 1;

                _Mq9Series.Add(new ChartSeries { Name = "Mq9 nyers érték", Data = mq9RawData });
                _Mq9Series.Add(new ChartSeries { Name = "Mq9 Co érték [ppm]", Data = mq9CoData });
                _Mq9Series.Add(new ChartSeries { Name = "Mq9 LPG érték [ppm]", Data = mq9LpgData });
                _Mq9Series.Add(new ChartSeries { Name = "Mq9 metán érték [ppm]", Data = mq9Ch4Data });
                _mq9options.LineStrokeWidth = 1;

                StateHasChanged();
            }

        }

        public void ListValues()
        {
            
        }
    }
}
