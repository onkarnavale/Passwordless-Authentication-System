using Fido2Apis.Domain.Fido2Auth;
using Fido2Apis.Domain.User;
using Fido2Apis.Infra.Db;
using Fido2Apis.Infra.Jwt;
using Fido2Apis.Infra.ObjectMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

// Dependency
builder.Services.AddScoped<IFido2RegistrationService, Fido2RegistrationService>();
builder.Services.AddScoped<IFido2AuthenticationService, Fido2AuthenticationService>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// NLog
builder.Host.UseNLog();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddFido2(options =>
{
    options.ServerDomain = builder.Configuration["fido2:serverDomain"];
    options.ServerName = "FIDO2 Implementation";
    options.Origin = builder.Configuration["fido2:origin"];
    options.TimestampDriftTolerance = builder.Configuration.GetValue<int>("fido2:timestampDriftTolerance");
}
).AddCachedMetadataService(config =>
{
    config.AddFidoMetadataRepository(httpClientBuilder =>
    {

    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
