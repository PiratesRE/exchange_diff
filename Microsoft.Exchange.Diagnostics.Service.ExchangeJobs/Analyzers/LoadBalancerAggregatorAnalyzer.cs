using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Analyzers
{
	public class LoadBalancerAggregatorAnalyzer : PerfLogAggregatorAnalyzer
	{
		public LoadBalancerAggregatorAnalyzer(IJob job) : this(job, "LoadBalancerAggregatorAnalyzer")
		{
		}

		public LoadBalancerAggregatorAnalyzer(IJob job, string outputFile) : base(job, outputFile)
		{
			this.defaultInformation = MachineInformationSource.MachineInformation.GetCurrent();
			this.devices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		protected override void OutputDataInternal()
		{
			foreach (KeyValuePair<DiagnosticMeasurement, ValueStatistics> keyValuePair in base.CurrentValues)
			{
				DiagnosticMeasurement key = keyValuePair.Key;
				ValueStatistics value = keyValuePair.Value;
				if (value.SampleCount > 0 && !this.devices.Contains(key.MachineName))
				{
					MachineInformationSource.MachineInformation machineInformation = new MachineInformationSource.MachineInformation(key.MachineName, this.defaultInformation.ForestName, this.defaultInformation.SiteName, "LoadBalancer", this.defaultInformation.MaintenanceStatus, this.defaultInformation.MachineVersion);
					OutputStream outputStream = base.Job.GetOutputStream(this, "MachineInformation");
					string text = DateTimeUtils.Floor(DateTime.UtcNow, TimeSpan.FromMinutes(5.0)).ToString("O");
					outputStream.WriteLine("{0},{1}", new object[]
					{
						text,
						machineInformation.ToString()
					});
					this.devices.Add(key.MachineName);
				}
			}
			base.OutputDataInternal();
		}

		private readonly MachineInformationSource.MachineInformation defaultInformation;

		private readonly HashSet<string> devices;
	}
}
