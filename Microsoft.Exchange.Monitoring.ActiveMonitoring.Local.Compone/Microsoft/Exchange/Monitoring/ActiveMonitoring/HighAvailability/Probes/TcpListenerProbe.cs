using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	public class TcpListenerProbe : ReplicationHealthChecksProbeBase
	{
		public static ProbeDefinition CreateDefinition(string probeNamePrefix, int recurrenceInterval)
		{
			return TcpListenerProbe.CreateDefinition(HighAvailabilityConstants.ServiceName, "TCP", probeNamePrefix, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public static ProbeDefinition CreateDefinition(string serviceName, string targetResource, string probeName, int recurrenceInterval, int timeout, int maxRetry)
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				ServiceName = serviceName,
				TypeName = typeof(TcpListenerProbe).FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = recurrenceInterval,
				TimeoutSeconds = timeout,
				MaxRetryAttempts = maxRetry,
				TargetResource = targetResource
			};
		}

		protected override Type GetCheckType()
		{
			return typeof(TcpListenerCheck);
		}
	}
}
