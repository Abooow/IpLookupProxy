# IpLookupProxy 🕵️‍♂️🌍

IpLookupProxy is a proxy server that calls other IP lookup services and then caches the IP information in a database. The goal of this project is to help conserve API tokens and avoid exceeding the number of requests allowed by IP lookup services. By caching IP information, you can perform more lookups within the allowed limit.

## Why Use IpLookupProxy?
IpLookupProxy is useful if you want to cache IP information and avoid making multiple calls to IP lookup services. By caching the IP information, you can conserve your tokens and avoid exceeding the number of requests allowed by your API key. Additionally, you can create multiple API keys to the same service or combine different services to increase the number of IP lookups you can perform each month.

<br />

## Download and Run
1. Clone the repository using the following command:
```sh
git clone https://github.com/Abooow/IpLookupProxy.git
```

2. Navigate to the root directory of the project:
```sh
cd IpLookupProxy
```

3. Run the project using the .NET CLI: (You will need to have the .NET 7 SDK installed on your machine.)
```sh
dotnet run
```
Alternatively, you can use Docker to run the project. A `docker-compose.yml` file is available in the root directory, which can be used to start the service with the following command:
```sh
docker-compose up
```

Once the service is running, you can access it by navigating to `https://localhost:5000/api/:ipAddress` in your web browser or making an HTTP GET request to the same URL using a tool like Postman or cURL.

## Configuration
You will need to update some of some values inside `appsettings.json` with your own values in order for the application to work correctly.

### ApiServerSettings
* `RequireKey`: If set to true, the API server will require a valid API key to access the /api endpoints.
* `QueryName`: The name of the query parameter that should contain the API key.
* `Key`: The API key that should be used to access the /api endpoints.
* `AllowAnyRemote`: If set to true, the API server will allow requests from any remote host. If set to false, only requests from hosts in the AllowedRemotes list will be allowed.
* `AllowedRemotes`: A list of remote hosts that are allowed to access the service.

### Clients
This is an array of objects that describe the different IP lookup services that can be used by the application. Each object should contain the following properties:

* `Handler`: The name of the IP lookup service handler to use.
* `Name`: A descriptive name for the IP lookup service *(not required)*.
* `Site`: The base URL of the IP lookup service *(not required)*.
* `Enabled`: Whether or not this IP lookup service is enabled.
* `ApiKey`: The API key to use when accessing this IP lookup service.
* `RateLimitingRules`: An array of objects that describe the rate limiting rules for this IP lookup service. Each object should contain the following properties:
  * `Occurrences`: The number of times this IP lookup service can be called within the specified TimeUnit.
  * `TimeUnit`: The time unit for the rate limiting rule, expressed as a timespan (e.g. "1.00:00:00" for 1 day).

<br />

## Technologies Used
* C#
* .NET 7
* ASP.NET Core
* MongoDB
