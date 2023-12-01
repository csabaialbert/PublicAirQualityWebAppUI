using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirQualityUI.Models;

public partial class SensorValue
{
    //Sensorvalues adattábla alapján legenerált objektum. Ahol a JsonIgnore tulajdonság megadásra került, az adott paraméter nem kerül bele a JSON formátumba való formázás során.
    public long Id { get; set; }

    public decimal? Mq2Raw { get; set; }

    public decimal? Mq2Lpg { get; set; }

    public decimal? Mq2Co { get; set; }

    public decimal? Mq2Smoke { get; set; }

    public decimal? Mq2Propane { get; set; }

    public decimal? Mq2H2 { get; set; }

    public decimal? Mq2Alcohol { get; set; }

    public decimal? Mq2Ch4 { get; set; }

    public decimal? Mq3Raw { get; set; }

    public decimal? Mq3Alcohol { get; set; }

    public decimal? Mq3Benzine { get; set; }

    public decimal? Mq3Exane { get; set; }

    public decimal? Mq3Lpg { get; set; }

    public decimal? Mq3Co { get; set; }

    public decimal? Mq3Ch4 { get; set; }

    public decimal? Mq4Raw { get; set; }

    public decimal? Mq4Ch4 { get; set; }

    public decimal? Mq4Lpg { get; set; }

    public decimal? Mq4H2 { get; set; }

    public decimal? Mq4Smoke { get; set; }

    public decimal? Mq4Alcohol { get; set; }

    public decimal? Mq4Co { get; set; }

    public decimal? Mq135Raw { get; set; }

    public decimal? Mq135Aceton { get; set; }

    public decimal? Mq135Tolueno { get; set; }

    public decimal? Mq135Alcohol { get; set; }

    public decimal? Mq135Co2 { get; set; }

    public decimal? Mq135Nh4 { get; set; }

    public decimal? Mq135Co { get; set; }

    public decimal? Mq6Raw { get; set; }

    public decimal? Mq6Lpg { get; set; }

    public decimal? Mq6Ch4 { get; set; }

    public decimal? Mq6H2 { get; set; }

    public decimal? Mq6Alcohol { get; set; }

    public decimal? Mq6Co { get; set; }

    public decimal? Mq7Raw { get; set; }

    public decimal? Mq7H2 { get; set; }

    public decimal? Mq7Co { get; set; }

    public decimal? Mq7Lpg { get; set; }

    public decimal? Mq7Ch4 { get; set; }

    public decimal? Mq7Alcohol { get; set; }

    public decimal? Mq8Raw { get; set; }

    public decimal? Mq8H2 { get; set; }

    public decimal? Mq8Alcohol { get; set; }

    public decimal? Mq8Lpg { get; set; }

    public decimal? Mq8Ch4 { get; set; }

    public decimal? Mq8Co { get; set; }

    public decimal? Mq9Raw { get; set; }

    public decimal? Mq9Co { get; set; }

    public decimal? Mq9Lpg { get; set; }

    public decimal? Mq9Ch4 { get; set; }

    public decimal? Humidity { get; set; }

    public decimal? Temperature { get; set; }

    public int ModuleId { get; set; }

    public DateTime? ReadDate { get; set; }
    [JsonIgnore]
    public virtual Module Module { get; set; } = null!;
}
