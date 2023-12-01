using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirQualityUI.Models;

public partial class User
{
    //Modules adattábla alapján legenerált objektum. Ahol a JsonIgnore tulajdonság megadásra került, az adott paraméter nem kerül bele a JSON formátumba való formázás során.
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string CreateDate { get; set; } = null!;

    public string Password { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<UserModuleConnection> UserModuleConnections { get; set; } = new List<UserModuleConnection>();
}
