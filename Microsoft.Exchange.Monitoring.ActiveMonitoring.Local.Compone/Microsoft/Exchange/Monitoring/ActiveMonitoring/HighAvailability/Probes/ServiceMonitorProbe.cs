using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	public class ServiceMonitorProbe : ProbeWorkItem
	{
		public static int Suppression
		{
			get
			{
				int value = HighAvailabilityUtility.RegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "OneCopyMonitorStaleAlertInMins", 0);
				if (value > 0)
				{
					return value;
				}
				return 90;
			}
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(ServiceMonitorProbe).FullName,
				Name = name,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = timeout,
				MaxRetryAttempts = maxRetry,
				TargetResource = Environment.MachineName
			};
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, int recurrenceInterval)
		{
			return ServiceMonitorProbe.CreateDefinition(name, serviceName, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			base.Result.StateAttribute1 = base.Definition.Name;
			base.Result.StateAttribute2 = base.GetType().FullName;
			IADDatabase[] allLocalDatabases = CachedAdReader.Instance.AllLocalDatabases;
			bool flag = false;
			if (allLocalDatabases != null && allLocalDatabases.Length > 0)
			{
				foreach (IADDatabase iaddatabase in allLocalDatabases)
				{
					if (iaddatabase.ReplicationType == ReplicationType.Remote)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					base.Result.StateAttribute5 = string.Format("No replicated databases found on server {0} - List scanned={1}", Environment.MachineName, string.Join(",", (from o in allLocalDatabases
					select o.Name).ToArray<string>()));
					return;
				}
				string value = RegistryReader.Instance.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States", "OneCopyMonitorLastRun", string.Empty);
				TimeSpan currentSystemUpTime = this.GetCurrentSystemUpTime();
				if (!(currentSystemUpTime > TimeSpan.FromMinutes((double)ServiceMonitorProbe.Suppression)))
				{
					base.Result.StateAttribute3 = string.Format("System Up time is {0} minutes, which is less than {1}. Skipping...", currentSystemUpTime.TotalMinutes, ServiceMonitorProbe.Suppression);
					return;
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new HighAvailabilityMAProbeRedResultException("Last run time is empty!");
				}
				DateTime minValue = DateTime.MinValue;
				if (!DateTime.TryParse(value, out minValue) || minValue == DateTime.MinValue)
				{
					throw new HighAvailabilityMAProbeRedResultException(string.Format("Invalid last run time format - {0}", value));
				}
				TimeSpan t = (minValue - DateTime.UtcNow).Duration();
				base.Result.StateAttribute3 = minValue.ToString();
				base.Result.StateAttribute4 = t.TotalMinutes.ToString();
				if (t > TimeSpan.FromMinutes((double)ServiceMonitorProbe.Suppression))
				{
					throw new HighAvailabilityMAProbeRedResultException(string.Format("OneCopy service monitor loop hasn't been running for {0} mins! LastRunUtc={1}", ServiceMonitorProbe.Suppression, minValue.ToString()));
				}
			}
			else
			{
				base.Result.StateAttribute5 = string.Format("No databases found on server {0}", Environment.MachineName);
			}
		}

		private TimeSpan GetCurrentSystemUpTime()
		{
			long tickCount = NativeMethods.GetTickCount64();
			return TimeSpan.FromMilliseconds((double)tickCount);
		}
	}
}
