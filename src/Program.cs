using IpLookupProxy.Api;
using IpLookupProxy.Api.DataAccess.Repositories;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Middlewares;
using IpLookupProxy.Api.Models;
using IpLookupProxy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IIpRepository, MongoDbIpRepository>(x =>
    new(builder.Configuration.GetConnectionString("MongoDb"), builder.Configuration.GetConnectionString("MongoDbName")));

var apiServerKeySection = builder.Configuration.GetSection(nameof(ApiServerKeySettings));
builder.Services.Configure<ApiServerKeySettings>(apiServerKeySection);

var configuredClients = builder.Configuration.GetSection("Clients").Get<ClientConfigInfo[]>();
var clientRateLimiter = new ClientRateLimiter(configuredClients
    .Select(x => new KeyValuePair<string, IEnumerable<RateLimitRule>>(
        x.Name,
        x.RateLimitingRules.Select(y => new RateLimitRule(y.Occurrences, y.TimeUnit))
        )));

var clientsConfiguration = new ClientsConfiguration(configuredClients
    .Select(x => new KeyValuePair<string, ClientConfigInfo>(x.Name, x)));

builder.Services.AddSingleton(clientRateLimiter);
builder.Services.AddSingleton(clientsConfiguration);
builder.Services.AddSingleton<IpClientsFactory>();
builder.Services.AddTransient<IpInfoService>();

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
app.MapGet("/", () => "Yo! Use /api/:ip to get IP location");

app.MapGet("/api/{ip}", async (string ip, IpInfoService ipInfoService) =>
{
    try
    {
        var ipInfo = await ipInfoService.GetIpInfoAsync(ip);
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
        return Results.BadRequest(new { ErrorMessage = "An unexpected error has occurred." });
    }
});

app.Run();