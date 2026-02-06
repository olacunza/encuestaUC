using AssesmentUC.Service;
using AssesmentUC.Infrastructure;
using AssesmentUC.Model.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

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

var googleOAuthOptions = builder.Configuration
    .GetSection(GoogleOAuthOptions.SectionName)
    .Get<GoogleOAuthOptions>();

if (googleOAuthOptions == null)
    throw new InvalidOperationException($"Sección '{GoogleOAuthOptions.SectionName}' no encontrada en appsettings.json");

if (string.IsNullOrWhiteSpace(googleOAuthOptions.ClientId))
    throw new InvalidOperationException($"ClientId no puede estar vacío en GoogleOAuth");

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

            ValidateLifetime = googleOAuthOptions.ValidateLifetime,

            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.FromMinutes(5),

            NameClaimType = "name"

        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                      .GetRequiredService<ILogger<Program>>();

                logger.LogError("Autenticación fallida: {Message}",
                  context.Exception?.Message);

                return Task.CompletedTask;
            },

            OnTokenValidated = context => {
                var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
                
                var principal = context.Principal;
                var email = principal?.FindFirst("email")?.Value;
                var sub = principal?.FindFirst("sub")?.Value;

                logger.LogInformation("Token validado - Email: {Email}, Sub: {Sub}", email, sub);
                
                return Task.CompletedTask;
            },

            OnMessageReceived = context => {
                var token = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token.Replace("Bearer ", "");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();