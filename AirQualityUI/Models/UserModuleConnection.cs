using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirQualityUI.Models;

public partial class UserModuleConnection
{
    //Modules adattábla alapján legenerált objektum. Ahol a JsonIgnore tulajdonság megadásra került, az adott paraméter nem kerül bele a JSON formátumba való formázás során.
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ModuleId { get; set; }
    [JsonIgnore]
    public virtual Module Module { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
