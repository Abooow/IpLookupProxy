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
ClientConfigInfo.EnsureClientConfigsIsValid(configuredClients);

// Services.
builder.Services.AddSingleton(new ConfiguredClients(configuredClients));
builder.Services.AddSingleton<IpLookupClientFactory>();
builder.Services.AddSingleton<IpInfoService>();
builder.Services.AddSingleton<IClientLoadBalancer, FirstAvailableClientLoadBalancer>();

var app = builder.Build();

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
        return Results.Ok(ipInfo);
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