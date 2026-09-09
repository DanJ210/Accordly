using Accordly.Api.Hubs;
using Accordly.Api.Middleware;
using Accordly.Application;
using Accordly.Infrastructure;
using Carter;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("logs/accordly-.log", rollingInterval: RollingInterval.Day).CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCarter();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	var secret = builder.Configuration["Jwt:Secret"] ?? "CHANGE_ME_TO_A_32_CHAR_MIN_SECRET";
	options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "accordly", ValidAudience = builder.Configuration["Jwt:Audience"] ?? "accordly-client", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)) };
});
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); app.UseHangfireDashboard("/hangfire"); }
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
app.MapHub<AgreementsHub>("/hubs/agreements");
app.Run();

/// <summary>Application entry point marker used by integration tests.</summary>
public partial class Program { }
