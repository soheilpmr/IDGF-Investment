using IdentityModel;
using IDGF.Core.Data;
using IDGF.Core.Infrastructure.UnitOfWork;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "IDGF-Core", Version = "v1" });

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

builder.Services.AddDbContext<CoreDbContext>();

builder.Services.AddScoped<ICoreUnitOfWork, CoreUnitOfWork>();
builder.Services.AddScoped<MandehtransactionService, MandehtransactionService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
          options.MapInboundClaims = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidIssuer = builder.Configuration["JWTBearerSettings:Issuer"],
              ValidateAudience = true,
              ValidAudience = builder.Configuration["JWTBearerSettings:Audience"],
              ValidateLifetime = true,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTBearerSettings:Key"])),
              ValidateIssuerSigningKey = true,
              ClockSkew = TimeSpan.Zero,
              RoleClaimType = JwtClaimTypes.Role
          };
      });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.EnvironmentName == "Docker")
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
