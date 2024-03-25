using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System;

namespace Serilog.Sinks.Humio {
	public static class HumioSinkExtensions {
		public static LoggerConfiguration Humio(
				  this LoggerSinkConfiguration loggerConfiguration,
				  string ingestToken = null, string url = "https://cloud.humio.com", LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum) {
			var batchingOptions = new PeriodicBatchingSinkOptions
			{
				Period = TimeSpan.FromSeconds(5),
				BatchSizeLimit = 10,
			};
			var sink = new PeriodicBatchingSink(new Humio(ingestToken, url), batchingOptions);
			return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
		}
	}
}