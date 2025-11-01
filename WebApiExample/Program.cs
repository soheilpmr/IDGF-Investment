using IdentityModel;
using IDGF.Auth.Services.AdminServices;
using IDGFAuth.Data;
using IDGFAuth.Data.Configs;
using IDGFAuth.Data.Entities;
using IDGFAuth.Infrastructure.Initializer;
using IDGFAuth.Infrastructure.UnitOfWork;
using IDGFAuth.Services;
using IDGFAuth.Services.EmailService;
using IDGFAuth.Services.JWT;
using IDGFAuth.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region ConnectionStringHelper
static string getConnectionStringSQLServer()
{
    var environmentName =
      Environment.GetEnvironmentVariable(
          "ASPNETCORE_ENVIRONMENT");

    var config = new ConfigurationBuilder().AddJsonFile("appsettings" + (String.IsNullOrWhiteSpace(environmentName) ? "" : "." + environmentName) + ".json", false).Build();

    return config.GetConnectionString("DefaultConnectionSQLServer");
}
static string getConnectionStringOracle()
{
    var environmentName =
      Environment.GetEnvironmentVariable(
          "ASPNETCORE_ENVIRONMENT");

    var config = new ConfigurationBuilder().AddJsonFile("appsettings" + (String.IsNullOrWhiteSpace(environmentName) ? "" : "." + environmentName) + ".json", false).Build();

    return config.GetConnectionString("DefaultConnectionOracle");
}
#endregion
// Add services to the container.

#region JWT
//builder.Services.AddAuthentication(
//    options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(x =>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JWTBearerSettings:Issuer"],
        ValidAudience = builder.Configuration["JWTBearerSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTBearerSettings:Key"])),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = JwtClaimTypes.Role
        //NameClaimType = JwtClaimTypes.Name
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.Principal);

            if (user == null)
            {
                context.Fail("Unauthorized: user not found.");
                return;
            }

            var securityStampClaim = context.Principal.FindFirstValue("AspNet.Identity.SecurityStamp");

            if (securityStampClaim == null || user.SecurityStamp != securityStampClaim)
            {
                context.Fail("This token has been invalidated.");
            }
        }
    };
});
#endregion

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireClaim("scope", "facilityman");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "IDGF-Identity", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"Enter 'Bearer' [space] and your token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    },
                    Scheme="oauth2",
                    Name="Bearer",
                    In=ParameterLocation.Header
                },
                new List<string>()
            }

        });
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<IDGFAuthDbContextSQL>()
.AddDefaultTokenProviders();

#region OracleRegistration
//builder.Services.AddDbContext<WebApiDbContextOracle>((serviceProvider, options) =>
//{

//});
#endregion

builder.Services.Configure<ConnectionStringConfig>(builder.Configuration.GetSection("ConnectionStrings"));

#region SQLServerRegistration
builder.Services.AddDbContext<IDGFAuthDbContextSQL>(options =>
    options.UseSqlServer(
              builder.Configuration.GetConnectionString("DefaultConnectionSQLServer"),
        sql => sql.MigrationsAssembly("Migrations.SQL")
    ));


#endregion

builder.Services.AddScoped<IWebApiUnitOfWorkAsync, WebApiUnitOfWorkAsync>();
//builder.Services.AddScoped<WebServiceUserService, WebServiceUserService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped(typeof(IIdentityAdminService<,>), typeof(IdentityAdminService<,>));
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
// End Infrastructure Implemention
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var connectionStringConfig = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionStringConfig>>().Value;

    if (connectionStringConfig.OracleActivaityStatus == "true")
    {
        //var dataContextoracle = scope.ServiceProvider.GetRequiredService<IDGFAuthDbContextSQL>();
        //dataContextoracle.Database.Migrate();
    }
    if (connectionStringConfig.SQLServerActivaityStatus == "true")
    {
        var dataContextsql = scope.ServiceProvider.GetRequiredService<IDGFAuthDbContextSQL>();
        //dataContextsql.Database.Migrate();
    }

    var f = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    f.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//if (app.Environment.EnvironmentName == "Docker")
//{
	app.UseSwagger();
	app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
//app.UseMiddleware<JWTMiddleware>();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<JWTMiddleware>();
app.MapControllers();

app.Run();




















// void testconnecrtion()
//{
//    Console.WriteLine("Starting...");

//    // Replace with your actual connection string
//    using (var _db = new OracleConnection("User Id=hswitch;Password=hswitch;Data Source=10.9.12.40:1521/odb;"))
//    {
//        try
//        {
//            Console.WriteLine("Opening connection...");
//            _db.Open();
//            Console.WriteLine("Connected successfully!");

//            // Retrieve server version (optional)
//            var serverVersion = _db.ServerVersion;
//            Console.WriteLine($"Server version: {serverVersion}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }

//    Console.WriteLine("Press any key to exit...");
//}

//testconnecrtion();