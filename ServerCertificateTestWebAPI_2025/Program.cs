using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using MTLS_Cert_Lib;
using ServerCertificateTestWebAPI_2025;
using ServerCertificateTestWebAPI_2025.Interface;
using ServerCertificateTestWebAPI_2025.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<ITFLogger, TFLogger>();

var app = builder.Build();

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
