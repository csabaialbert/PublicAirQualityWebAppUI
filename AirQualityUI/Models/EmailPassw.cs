namespace AirQualityUI.Models
{
    public class EmailPassw
    {
        //Egy e-mail és jelszó mezőt tartalmazó objektum, ami akkor került használatban, amikor nem volt szükség az összes felhasználói adatra a művelethez,
        //elegendő volt ez a két adat.
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
