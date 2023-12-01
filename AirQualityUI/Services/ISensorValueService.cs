using AirQualityUI.Models;
using DateRange = AirQualityUI.Models.DateRange;

namespace AirQualityUI.Services
{
    public interface ISensorValueService
    {
        //Interfész a SensorValueService szerviz eléréséhez.
        DateRange LastWeek(DateTime date);
        Task<List<SensorValue>> GetLastWeekValuesForModuleId(int moduleId);
        Task<List<SensorValue>> GetTodayValuesForModuleId(int moduleId);
        Task<List<SensorValue>> GetValuesForSelectedDateByModuleId(int moduleId, DateTime selectedDate);
        Task<IEnumerable<SensorValue>> GetValues();
        Task<IEnumerable<SensorValue>> GetValuesByModuleId(long id);
        Task<SensorValue> PostValues(SensorValue values);
        Task<string> createNotificationMailBody(SensorValue values);
        Task<string> sendMail(string messageBody, List<User> usersToSend);
    }
}
