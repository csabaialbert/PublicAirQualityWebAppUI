namespace AirQualityUI.Models
{
    public class DateRange
    {
        //DateRange objektum, ami két dátumot tud magába foglalni (kezdő és befejező), ennek segítségével időintervallum tárolható el.
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
