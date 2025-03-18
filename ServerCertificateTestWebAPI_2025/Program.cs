using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using MTLS_Cert_Lib;
using Serilog;
using ServerCertificateTestWebAPI_2025;
using ServerCertificateTestWebAPI_2025.Services;


var builder = WebApplication.CreateBuilder(args);

//// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("Logs/ServerCertificationTestWebAPI-.log", rollingInterval: RollingInterval.Day);
});

//// ✅ Configure Serilog
//builder.Host.UseSerilog((context, config) =>
//{
//    config.ReadFrom.Configuration(context.Configuration);
//});
builder.Services.AddLogging();

//// 1️⃣ Add Certificate Authentication
builder.Services
    .AddAuthentication("Certificate")
    .AddScheme<AuthenticationSchemeOptions, CertificateAuthenticationHandler>("Certificate", null);

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable HTTPS with certificate authentication
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = true;
});

//Adding the Http Context Accessor to fetch thw client IP Address, Username and RequestPath
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HttpContextService>();
// 🔹 Add TFLogger Service
//builder.Services.AddSingleton<ITFLogger, TFLogger>();

// Register ITFLogger
//builder.Services.AddSingleton<Microsoft.TeamFoundation.Common.ITFLogger, Microsoft.TeamFoundation.Common.TFLogger>(); // Replace TFLogger with your actual implementation


builder.Services.AddSingleton<MTLS_Cert_Lib.ITFLogger, MTLS_Cert_Lib.TFLogger>();
// ✅ Register any other dependent services
builder.Services.AddTransient<TFLoggerService>();


var app = builder.Build();

app.UseSerilogRequestLogging(); // Enables logging for HTTP requests

// Use the custom certificate authenicate middleware
app.UseMiddleware<CertificateAuthenicateMiddleware>();

// Use the custom certificate validation middleware
app.UseMiddleware<CertificateValidationMiddleware>();



app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapControllers();

app.Run();
