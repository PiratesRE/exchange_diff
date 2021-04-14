using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.CalculatedCounters
{
	public class DiskFreeSpaceCalculatedCounter : ICalculatedCounter
	{
		public DiskFreeSpaceCalculatedCounter()
		{
			this.percentFreeSpaceValues = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
			this.freeMegabyteValues = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
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
					if (keyValuePair.Key.ObjectName.Equals("LogicalDisk", StringComparison.OrdinalIgnoreCase) && (keyValuePair.Key.InstanceName.IndexOf("ExchangeVolumes", StringComparison.OrdinalIgnoreCase) >= 0 || keyValuePair.Key.InstanceName.IndexOf("ExchangeDBs", StringComparison.OrdinalIgnoreCase) >= 0))
					{
						Dictionary<string, float> dictionary = null;
						if (keyValuePair.Key.CounterName.Equals("Free Megabytes", StringComparison.OrdinalIgnoreCase))
						{
							dictionary = this.freeMegabyteValues;
						}
						else if (keyValuePair.Key.CounterName.Equals("% Free Space", StringComparison.OrdinalIgnoreCase))
						{
							dictionary = this.percentFreeSpaceValues;
						}
						if (dictionary != null && keyValuePair.Value != null)
						{
							dictionary[keyValuePair.Key.InstanceName] = keyValuePair.Value.Value;
						}
					}
				}
				float num = 0f;
				float num2 = 0f;
				foreach (KeyValuePair<string, float> keyValuePair2 in this.freeMegabyteValues)
				{
					float num3;
					if (this.percentFreeSpaceValues.TryGetValue(keyValuePair2.Key, out num3) && num3 > 0f)
					{
						num2 += keyValuePair2.Value;
						num += keyValuePair2.Value / (num3 / 100f);
					}
				}
				float value = (num > 0f) ? ((float)Math.Round((double)(num2 / num * 100f))) : 0f;
				countersAndValues.Add(DiskFreeSpaceCalculatedCounter.SyntheticPercentFreeSpaceMeasure, new float?(value));
				countersAndValues.Add(DiskFreeSpaceCalculatedCounter.SyntheticFreeMegabytesMeasure, new float?(num2));
				this.percentFreeSpaceValues.Clear();
				this.freeMegabyteValues.Clear();
			}
		}

		public const string SyntheticDiskObjectName = "SyntheticDisk";

		public const string LogicalDiskObjectName = "LogicalDisk";

		public const string PercentFreeSpaceCounterName = "% Free Space";

		public const string FreeMegabytesCounterName = "Free Megabytes";

		private static readonly DiagnosticMeasurement SyntheticPercentFreeSpaceMeasure = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "SyntheticDisk", "% Free Space", string.Empty);

		private static readonly DiagnosticMeasurement SyntheticFreeMegabytesMeasure = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "SyntheticDisk", "Free Megabytes", string.Empty);

		private readonly Dictionary<string, float> percentFreeSpaceValues;

		private readonly Dictionary<string, float> freeMegabyteValues;
	}
}
