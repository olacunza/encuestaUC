using AssesmentUC.Service;
using AssesmentUC.Infrastructure;
using AssesmentUC.Model.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.Configure<GoogleOAuthOptions>(builder.Configuration.GetSection(GoogleOAuthOptions.SectionName));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
       {
           policy
               .AllowAnyOrigin()
               .AllowAnyMethod()
            .AllowAnyHeader();
       });
});

// Configurar autenticación JWT usando GoogleOAuthOptions
var googleOAuthOptions = builder.Configuration.GetSection(GoogleOAuthOptions.SectionName).Get<GoogleOAuthOptions>();

if (googleOAuthOptions == null)
    throw new InvalidOperationException($"Sección '{GoogleOAuthOptions.SectionName}' no encontrada en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = googleOAuthOptions.Authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = googleOAuthOptions.ValidateIssuer,
            ValidIssuer = googleOAuthOptions.ValidIssuer,
            ValidateAudience = googleOAuthOptions.ValidateAudience,
            ValidAudience = googleOAuthOptions.ClientId,
            ValidateLifetime = googleOAuthOptions.ValidateLifetime
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();