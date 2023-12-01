using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirQualityUI.Models;

public partial class Module
{
    //Modules adattábla alapján legenerált objektum. Ahol a JsonIgnore tulajdonság megadásra került, az adott paraméter nem kerül bele a JSON formátumba való formázás során.
    public int Id { get; set; }

    public string ModuleName { get; set; } = null!;

    public string? Description { get; set; }

    [JsonIgnore]
    public virtual ICollection<SensorValue> SensorValues { get; set; } = new List<SensorValue>();
    [JsonIgnore]
    public virtual ICollection<UserModuleConnection> UserModuleConnections { get; set; } = new List<UserModuleConnection>();
}
