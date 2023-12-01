using AirQualityUI.Models;
using AirQualityUI.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Principal;

namespace AirQualityUI.Controllers
{
    //Az adatküldő modulokkal való kommunikáció megvalósítására szolgáló vezérlő.
    [Route("[controller]")]
    [ApiController]
    public class AirQualityController : ControllerBase
    {
        ISensorValueService _sensorValueService;
        IUserService _userService;
        IConfiguration _config;
        public AirQualityController(ISensorValueService sensorValuesService, IUserService userService, IConfiguration config)
        {
            //Elkészített szervizek inicializálása.
            _sensorValueService = sensorValuesService;
            _userService = userService;
            _config = config;
        }

        [AllowAnonymous]
        [Route("LoginModule")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] EmailPassw login)
        {
            //Modul beléptető adatait fogadó függvény
            //A deklarált response változó alapból a BadRequest értéket kapja meg.
            ActionResult response = BadRequest();
            //A modul bejelenzkeztetését elvégző függvény meghívása, ami a UserService elnevezésű szervizben található.
            var result = await _userService.AuthenticateModule(login);

            //A meghívott függvény boolean értéket ad vissza, és amennnyiben ez az érték igaz, meghívásra kerül a token generálást végző függvény,
            //majd a response változóba Ok státusz mellett elküldésre kerül a generált token.
            if (result)
            {
                var tokenString = GenerateJSONWebToken();
                response = Ok(new { token = tokenString });
            }
            //A válasz visszaküldése.
            return response;
        }

        private string GenerateJSONWebToken()
        {
            //JWT generálásáért felelős függvény.
            //kezdetben a konfigfájlból kiolvasásra kerülnek az eltárolt kulcsok, és ezek segítségével generálásra kerül a token.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              signingCredentials: credentials);

            //A legenerált token visszaadásra kerül.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<TokenValidationParameters> GetValidationParameters()
        {
            //A kapott token validálásához szükséges paraméterek megadására szolgáló függvény.
            return new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidateAudience = true,
                ValidateIssuer = true,  
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
            };
        }

        private async Task<bool> ValidateToken(string authToken)
        {
            //JWT ellenőrzésére szolgáló függvény
            try
            {
                //A JwtSecurityTokenHandler osztály egy új példányának inicializása,
                //valamint meghívásra kerül a token validációhoz szükséges paramétereket visszaadó függvény.
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = await GetValidationParameters();

                //A beépített funkciók segítségével elvégzésre kerül a validálás, majd sikeres végrehajtás esetén igaz értéket ad vissza a függvény.
                //Amennyiben nem elvégezhető a validáció, kivétel generálódik és hamis érték kerül visszaadásra.
                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            
        }

        [Route("PostValues")]
        [HttpPost]
        public async Task<ActionResult<SensorValue>> PostValues(SensorValue values)
        {
            //A modultól kapott értékek fogadására készült függvény.
            try
            {

                //Ellenőrzésre kerül, hogy a kapott token megfelelő-e.
                bool tokenIsValid = false;
                string key = "";
                foreach (var header in Request.Headers)
                {
                    if (header.Key == "Authorization")
                    {
                        key = header.Value.ToString().Split(' ')[1];
                        tokenIsValid = await ValidateToken(key);
                        break;
                    }
                }
                //Amennyiben nem megfelelő, Unauthorized hibakódot küld vissza a kliens felé válaszként.
                if (tokenIsValid == false)
                {
                    return Unauthorized("Insufficient credentials!");
                }

                //A kapott értékek egy új SensorValue objektum példányba kerülnek beírásra, az AutoMapper segítségével.
                SensorValue newValues = new SensorValue();
                values.Adapt(newValues);

                //Ellenőrzésre kerül, hogy az MQ szenzorok számolt értékei között van-e olyan, amely nagyobb mint egy.
                //Amennyiben van, meghívásra kerülnek az e-mail törzset generáló és e-mail küldő funkciók.
                //A folyamat végén pedig Ok válasz kerül visszaküldésre, amennyiben minden sikeres volt.
                bool outlierPresent = false;
                foreach (var propInfo in typeof(SensorValue).GetProperties())
                {
                    if (propInfo.Name.StartsWith("Mq") && !propInfo.Name.Contains("Raw"))
                    {
                        if (Convert.ToInt32(propInfo.GetValue(newValues)) > 1)
                        {
                            outlierPresent = true;
                        }
                    }
                }
                newValues.ReadDate = DateTime.Now;
                var postedValues = await _sensorValueService.PostValues(newValues);
                    
                string res = "";
                if (outlierPresent)
                {
                    var msgBody = await _sensorValueService.createNotificationMailBody(newValues);
                    var usersToSend = await _userService.GetUsersForModuleIds(newValues.ModuleId);

                    if (usersToSend.Count > 0)
                    {
                        res = await _sensorValueService.sendMail(msgBody, usersToSend);
                    }
                }
                return Ok("Success");
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
