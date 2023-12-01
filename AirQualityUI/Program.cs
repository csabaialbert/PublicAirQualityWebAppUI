using AirQualityUI.Services;
using AirQualityUI.Models;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using FluentValidation;
using System.Globalization;
using ApexCharts;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//valid�l� funkci� nyelv�nek magyarra t�rt�n� �ll�t�s�val, kiz�r�lag magyar nyelven jelennek meg a gener�lt �zenetek.
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("hu");

//Adatb�zishoz val� csatlakoz�shoz sz�ks�ges adatok beolvas�s�a az appsettings.json nev� konfigur�ci�s f�jlb�l.
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

//Fel�p�tend� szervizek megad�sa, amelyekre sz�ks�g van a program fut�sa k�zben.
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddMudServices();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ISensorValueService, SensorValueService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddDbContext<AirQualityDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<AirQualityDbContext>();
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

//Az alkalmaz�s build-el�se.
var app = builder.Build();

//K�l�nb�z� b�v�tm�nyek csatol�sa az alkalmaz�shoz.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//Az alkalmaz�s elind�t�sa.
app.Run();
