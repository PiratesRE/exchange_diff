using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Search.Engine;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public sealed class SearchNumberOfParserServersDegradationProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Search Foundation for Exchange\\DocParser"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("MaxPoolSize", null);
					if (value != null)
					{
						int num = Convert.ToInt32(value);
						if (num != SearchConfig.Instance.SandboxMaxPoolSize)
						{
							throw new SearchProbeFailureException(Strings.SearchNumberOfParserServersDegradation(num, SearchConfig.Instance.SandboxMaxPoolSize, ((double)SearchMemoryModel.GetSearchMemoryUsage() / 1024.0 / 1024.0).ToString("0.00")));
						}
					}
				}
			}
		}

		private const string MaxPoolSizePath = "SOFTWARE\\Microsoft\\Search Foundation for Exchange\\DocParser";

		private const string MaxPoolSize = "MaxPoolSize";
	}
}
