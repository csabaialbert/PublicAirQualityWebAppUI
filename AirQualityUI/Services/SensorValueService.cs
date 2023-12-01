using AirQualityUI.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using static MudBlazor.CategoryTypes;
using MudBlazor;
using DateRange = AirQualityUI.Models.DateRange;
using MudBlazor.Extensions;
using Azure;

namespace AirQualityUI.Services
{
    public class SensorValueService : ISensorValueService
    {
        //Szenzor értékeket kezelő szerviz.

        //AirQualityDbContext inicializálása, hogy elérhető legyen az adatbázis közvetlenül.
        private readonly AirQualityDbContext _dbContext;
        public SensorValueService(AirQualityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
       
        //A jelenlegi hét kezdő és befejező dátumát visszaadó függvény.
        public DateRange ThisWeek(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = date.Date.AddDays(-(int)date.DayOfWeek);
            range.End = range.Start.AddDays(7).AddSeconds(-1);

            return range;
        }

        //Az elmúlt hét kezdő és befejező dátumát visszaadó függvény.
        public DateRange LastWeek(DateTime date)
        {
            DateRange range = ThisWeek(date);

            range.Start = range.Start.AddDays(-7);
            range.End = range.End.AddDays(-7);

            return range;
        }

        //Az elmúlt héten, egy megadott modul által (modul azonosító alapján) rögzített adatokat visszaadó feladat.
        public async Task<List<SensorValue>> GetLastWeekValuesForModuleId(int moduleId) 
        {
            var lastweek = LastWeek(DateTime.Now);
            return await _dbContext.SensorValues.Where(value => value.ModuleId == moduleId && value.ReadDate >= lastweek.Start && value.ReadDate <= lastweek.End ).ToListAsync();
        }

        //Mai napra vonatkozó adatokat visszaadó feladat, egy megadott modulra (modul azonosjtó alapján).
        public async Task<List<SensorValue>> GetTodayValuesForModuleId(int moduleId)
        {
            try
            {
                var todayDate = DateTime.Now.Date;
                return await _dbContext.SensorValues.Where(value => value.ModuleId == moduleId && value.ReadDate >= todayDate).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<SensorValue>();
                throw;
            }
            
        }

        //Egy megadott dátumra, egy megadott modul aznapon rögzített adatait visszaadó feladat.
        public async Task<List<SensorValue>> GetValuesForSelectedDateByModuleId(int moduleId, DateTime selectedDate)
        {
            try
            {
                return await _dbContext.SensorValues.Where(value => value.ModuleId == moduleId && value.ReadDate >= selectedDate).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<SensorValue>();
                throw;
            }

        }

        //Összes mentett értéket visszaadó feladat.
        public async Task<IEnumerable<SensorValue>> GetValues()
        {
            return await _dbContext.SensorValues.ToListAsync();
        }

        //Egy modulhoz tartozó összes rögzített adatot visszaadó feladat. (modul azonosító alapján).
        public async Task<IEnumerable<SensorValue>> GetValuesByModuleId(long id)
        {
            return await _dbContext.SensorValues.Where(value => value.ModuleId == id).ToListAsync();
        }

        //Értesítő e-mail törzset generáló feladat.
        public async Task<string> createNotificationMailBody(SensorValue values) 
        {
            try
            {
                //HTML nyelven generálásra kerül az üzenettörzs.
                var module = await _dbContext.Modules.FirstOrDefaultAsync(m=>m.Id == values.ModuleId);
                string messageBody = "<font>Az alábbi listában szereplők adatok közül rendszerünk észlelt szennyezettségre utaló értékeket a(z) <strong> " + module.ModuleName + " </strong> névvel ellátott kliens felől.\nEzek az értékek piros színnel töréntek kiemelésre.\n <strong> Kérjük ellenőrizze az alábbi táblázatot!</strong> </font><br><br>";
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdOutlierStart = "<td style=\" border-color:#5c87b2; color:#FF0000; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Megnevezés" + htmlTdEnd;
                messageBody += htmlTdStart + "Érték" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;
                foreach (var propInfo in typeof(SensorValue).GetProperties())
                {
                    if (propInfo.Name.StartsWith("Mq") && !propInfo.Name.Contains("Raw") && Convert.ToInt32(propInfo.GetValue(values)) > 1)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdOutlierStart + propInfo.Name + " (ppm)" + htmlTdEnd;
                        messageBody = messageBody + htmlTdOutlierStart + propInfo.GetValue(values) + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                    else if(propInfo.Name.StartsWith("Mq") && !propInfo.Name.Contains("Raw"))
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + propInfo.Name + " (ppm)" + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + propInfo.GetValue(values) + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                    else if(propInfo.Name.StartsWith("Mq") && propInfo.Name.Contains("Raw"))
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + propInfo.Name + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + propInfo.GetValue(values) + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                
                }

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Hőmérséklet (°C):" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + values.Temperature.ToString() + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Páratartalom (%):" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + values.Humidity.ToString() + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTableEnd;

                messageBody = messageBody + "<br><br><br>";

                messageBody += "<font>Az alábbi táblázatban szereplő adatok tájékoztató jellegűek, melyek a https://www.cdc.gov/niosh/idlh/intridl4.html weboldalról származnak. </font><br><br>";
                messageBody += "<font>Magyarázat a lenti táblázathoz: \n </font><br>";
                messageBody += "<font>\t - IDLH: Azonnali végzetes kockázatot jelentő expozícióhoz tartozó koncentráció (Az életre vagy egészségre közvetlen veszélyt jelentő koncentráció értékei). \n </font><br>";
                messageBody += "<font>\t - CAS-szám: vegyi anyagok (kémiai elemek, vegyületek) azonosítására használt regisztrációs szám ( Chemical Abstracts Service ) \n </font><br>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Megnevezés" + htmlTdEnd;
                messageBody += htmlTdStart + "CAS-szám" + htmlTdEnd;
                messageBody += htmlTdStart + "IDLH érték" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Folyékony szénhidrogén gáz (LPG)" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "68476-85-7" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "2,000 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Szénmonoxid (CO)" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "630-08-0" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "1,200 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Széndioxid (CO2)" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "124-38-9" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "40,000 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Aceton" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "67-64-1" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "2,500 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Benzin" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "71-43-2" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "500 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Propán" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "74-98-6" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "2,100 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Toluol" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "108-88-3" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "500 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Ammónia (NH4)" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "7664-41-7" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "300 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + "Hexán" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "110-54-3" + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "1,100 ppm" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;

                messageBody = messageBody + htmlTableEnd;

                return messageBody; 
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //E-mailt küldő feladat, ami az előzőleg generált üzenetet elküldi a kapott felhasználók listájában minden személy részére.
        public async Task<string> sendMail(string messageBody, List<User> usersToSend) 
        {
            try
            {
                //email küldéshez szükséges adatok megadása.
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("sensorvaluealert@airqualitymonitoring.hu");

                //A listából az összes e-mail cím a címzettek közé kerül.
                foreach (var item in usersToSend)
                {
                    message.To.Add(new MailAddress(item.Email));
                }
                message.Subject = "Értesítés";
                message.IsBodyHtml = true;
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "localhost";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("sensorvaluealert@airqualitymonitoring.hu", "So59qoBgpH3yZR");
                
                //e-mail küldése.
                smtp.Send(message);
               
                return "ok";
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.InnerException.Message);
                return ex.InnerException.Message;
            }
        }

        //értékek beírása az adatbázisba
        public async Task<SensorValue> PostValues(SensorValue values)
        {
            try
            {
                await _dbContext.SensorValues.AddAsync(values);
                await _dbContext.SaveChangesAsync();
                return values;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
