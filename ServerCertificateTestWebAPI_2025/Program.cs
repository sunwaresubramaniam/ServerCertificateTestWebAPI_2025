using MTLS_Cert_Lib;
using ServerCertificateTestWebAPI_2025;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable HTTPS with certificate authentication
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

//Adding the Http Context Accessor to fetch thw client IP Address, Username and RequestPath
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HttpContextService>();

var app = builder.Build();

// Use the custom certificate validation middleware
app.UseMiddleware<CertificateValidationMiddleware>();

// Use the custom certificate authenicate middleware
app.UseMiddleware<CertificateAuthenicateMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
