using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Humio {
	public class Humio : IBatchedLogEventSink {
		private readonly string _ingestToken;
		private readonly ITextFormatter _formatter;
		private static readonly HttpClient HttpClient = new HttpClient();
		private readonly Uri _uri;

		public Humio(string ingestToken, string url) {
			_ingestToken = ingestToken;
			_formatter = new JsonFormatter();
			_uri = new Uri(
				$"{url}/api/v1/ingest/humio-structured");
		}

		public async Task EmitBatchAsync(IEnumerable<LogEvent> events) {
			try {
				/*
				 * Example structured payload. See ingest structured data docs https://docs.humio.com/api/ingest-api/#structured-data
				 *
				 * Required fields
				 * - At least one tag must be specified
				 * - Events require timestamp
				 *
				 * [
					  {
						"tags": {
						  "host": "server1",
						  "source": "application.log"
						},
						"events": [
						  {
							"timestamp": "2016-06-06T12:00:00+02:00",
							"attributes": {
							  "key1": "value1",
							  "key2": "value2"
							}
						  },
						  {
							"timestamp": "2016-06-06T12:00:01+02:00",
							"attributes": {
							  "key1": "value1"
							}
						  }
						]
					  }
					]
				 */

				var eventsInHumioFormat = new List<object>();
				foreach (var logEvent in events) {
					var sw = new StringWriter();
					_formatter.Format(logEvent, sw);
					var formattedLogEvent = JObject.Parse(sw.ToString());
					eventsInHumioFormat.Add(new { timestamp = logEvent.Timestamp, attributes = formattedLogEvent });
				}

				// host and source tags should be altered to your needs
				var payload = new[] { new { tags = new { host = Environment.MachineName, source = "ServiceManager" }, events = eventsInHumioFormat } };

				var jsonContent = JsonConvert.SerializeObject(payload);
				var request = new HttpRequestMessage(HttpMethod.Post, _uri) { Content = new StringContent(jsonContent, Encoding.UTF8, "application/json") };
				request.Headers.Add("Authorization", $"Bearer {_ingestToken}");

				var response = await HttpClient.SendAsync(request);

				if (response.StatusCode != HttpStatusCode.OK) {
					Console.WriteLine($"Failed to ship logs to Humio: {response.StatusCode} - {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}");
				}
			} catch (Exception e) {
				Console.WriteLine($"Failed to ship logs to Humio: {e}");
			}
		}

        public Task OnEmptyBatchAsync()
        {
			return Task.CompletedTask;
        }
    }
}