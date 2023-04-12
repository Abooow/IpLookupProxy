using IpLookupProxy.Api.DataAccess;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Middlewares;
using IpLookupProxy.Api.Models;
using IpLookupProxy.Api.Options;
using IpLookupProxy.Api.Services;
using IpLookupProxy.Api.Services.LoadBalancers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Database.
builder.Services.AddSingleton<IIpInfoRepository, MongoDbIpInfoRepository>(x =>
    new(builder.Configuration.GetConnectionString("MongoDb")!, builder.Configuration.GetConnectionString("MongoDbName")!));

// Server API configuration.
var apiServerSection = builder.Configuration.GetSection(nameof(ApiServerSettings));
builder.Services.Configure<ApiServerSettings>(apiServerSection);

// Clients configuration.
var configuredClients = builder.Configuration.GetSection("Clients").Get<ClientConfigInfo[]>() ?? throw new Exception("No Client configuration found");

// Services.
builder.Services.AddSingleton(x => new ConfiguredClients(configuredClients, x.GetRequiredService<ILogger<ConfiguredClients>>()));
builder.Services.AddSingleton<IpLookupClientFactory>();
builder.Services.AddSingleton<IpInfoService>();
builder.Services.AddSingleton<IClientLoadBalancer, SimpleRoundRobinClientLoadBalancer>();

var app = builder.Build();

// 'Warm up' ConfiguredClients service to validate ClientConfigs.
_ = app.Services.GetService<ConfiguredClients>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiServerKey();
app.UseHttpsRedirection();

// Endpoints.
app.MapGet("/", () => "Use /api/:ipAddress to get IP info");

app.MapGet("/api/{ipAddress}", async (string ipAddress, IpInfoService ipInfoService) =>
{
    try
    {
        var ipInfo = await ipInfoService.GetIpInfoAsync(ipAddress);
        return Results.Ok(new IpInfoResponse(ipInfo.Ip, ipInfo.City, ipInfo.Region, ipInfo.CountryCode, ipInfo.CountryName, ipInfo.Zip, ipInfo.Latitude, ipInfo.Longitude, ipInfo.Timezone, ipInfo.IsProxy));
    }
    catch (InvalidIpAddressException e)
    {
        return Results.BadRequest(new { ErrorMessage = e.Message });
    }
    catch (AllClientsThrottledException)
    {
        return new TooManyRequestsResult(new { ErrorMessage = "Too many requests, please try again later." });
    }
    catch (Exception)
    {
        if (app.Environment.IsDevelopment())
            throw;

        return Results.BadRequest(new { ErrorMessage = "An unexpected error has occurred." });
    }
});

app.Run();