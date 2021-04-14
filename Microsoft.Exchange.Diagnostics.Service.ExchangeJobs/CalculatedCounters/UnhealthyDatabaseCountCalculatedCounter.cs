using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.CalculatedCounters
{
	public class UnhealthyDatabaseCountCalculatedCounter : ICalculatedCounter
	{
		public UnhealthyDatabaseCountCalculatedCounter()
		{
			this.databases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			this.IsValidRole = ServerRole.IsRole("Mailbox");
		}

		internal bool IsValidRole { get; set; }

		public void OnLogHeader(List<KeyValuePair<int, DiagnosticMeasurement>> currentInputCounters)
		{
		}

		public void OnLogLine(Dictionary<DiagnosticMeasurement, float?> countersAndValues, DateTime? timestamp = null)
		{
			if (this.IsValidRole)
			{
				foreach (KeyValuePair<DiagnosticMeasurement, float?> keyValuePair in countersAndValues)
				{
					if (keyValuePair.Key.ObjectName.Equals("MSExchange Replication", StringComparison.OrdinalIgnoreCase) && UnhealthyDatabaseCountCalculatedCounter.ReplicationCounters.Contains(keyValuePair.Key.CounterName) && !keyValuePair.Key.InstanceName.Equals("_Total", StringComparison.OrdinalIgnoreCase) && keyValuePair.Key.InstanceName.IndexOf("Mailbox Database", StringComparison.OrdinalIgnoreCase) < 0 && keyValuePair.Value != null && keyValuePair.Value.Value > 0f)
					{
						this.databases.Add(keyValuePair.Key.InstanceName);
					}
				}
				countersAndValues.Add(UnhealthyDatabaseCountCalculatedCounter.Measure, new float?((float)this.databases.Count));
				this.databases.Clear();
			}
		}

		public const string ReplicationObjectName = "MSExchange Replication";

		public const string UnhealthyCounterName = "Unhealthy";

		private static readonly HashSet<string> ReplicationCounters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Suspended",
			"Seeding",
			"Failed",
			"FailedSuspended"
		};

		private static readonly DiagnosticMeasurement Measure = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchange Replication", "Unhealthy", string.Empty);

		private readonly HashSet<string> databases;
	}
}
