using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.Humio {
	public static class HumioSinkExtensions {
		public static LoggerConfiguration Humio(
				  this LoggerSinkConfiguration loggerConfiguration,
				  string ingestToken = null, string url = "https://cloud.humio.com", LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum) {
			return loggerConfiguration.Sink(new Humio(ingestToken, url), restrictedToMinimumLevel);
		}
	}
}