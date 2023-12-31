global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using dotnet_rpg.Dtos.Character;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;
global using Swashbuckle.AspNetCore.Filters;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using System.Security.Claims;
global using dotnet_rpg.Services.FightService;
using Microsoft.OpenApi.Models;
using dotnet_rpg.Services.WeaponService;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {

   c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme 
  
   {
        Description = """Standard Authorisation header using the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<ICharacterService, CharacterService>();  
builder.Services.AddScoped<IAuthentificationRepository, AuthentificationRepository>(); 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{
    
    options.TokenValidationParameters = new TokenValidationParameters
         {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateTokenReplay = true
                
         };
         options.IncludeErrorDetails = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IWeaponService,WeaponService>(); 
builder.Services.AddScoped<IFightService, FightService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
