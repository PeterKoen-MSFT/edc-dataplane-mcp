using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using DataPlane.McpServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration.GetSection("Authentication");
        options.Authority = config["Authority"];
        options.Audience = config["Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });

// Configure OpenTelemetry
var otelConfig = builder.Configuration.GetSection("OpenTelemetry");
builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddSource(otelConfig.GetValue<string>("ServiceName"))
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(otelConfig.GetValue<string>("ServiceName")))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

// Register Services
builder.Services.AddSingleton<IMcpToolService, MockMcpToolService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
