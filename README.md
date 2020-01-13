# Serilog.Sinks.Humio2
A Serilog sink for [Humio](https://www.humio.com).
## How to use
1. Grab the ingest token from Humio. (inside settings => Ingest => API Tokens)
1. Install the [Serilog.Sinks.Humio2](https://www.nuget.org/packages/serilog.sinks.humio2) package from NuGet.
1. If you are using the cloud version of Humio:
```csharp
var log = new LoggerConfiguration()
	.WriteTo.Humio(ingestToken: "yourIngestTokenHere")
	.CreateLogger();
```
4. If you use Humio on-prem:
```csharp
var log = new LoggerConfiguration()
	.WriteTo.Humio(url:"https://url.onprem.com", ingestToken: "yourIngestTokenHere")
	.CreateLogger();
```
5. If you configure the solution via appsettings in web.config:
```xml
<appSettings>
	<add key="serilog:using:Humio" value="Serilog.Sinks.Humio" />
	<add key="serilog:write-to:Humio" />
	<add key="serilog:write-to:Humio.ingestToken" value="yourIngestTokenHere" />
</appSettings>
```
## Credits

This work is derived from the following users GitHub gists:
 - @philijhale https://gist.github.com/philjhale/5eea539ec7e83f9b4264e59e45848673
 - @rasmuskl https://gist.github.com/rasmuskl/b280b6fe4c6942ad6ec29244666cfc07
