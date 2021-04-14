using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Responders;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal sealed class DNSCriticalDependency : ICriticalDependency
	{
		string ICriticalDependency.Name
		{
			get
			{
				return "DNSCriticalDependency";
			}
		}

		TimeSpan ICriticalDependency.RetestDelay
		{
			get
			{
				return DNSCriticalDependency.RetestDelay;
			}
		}

		string ICriticalDependency.EscalationService
		{
			get
			{
				return "Exchange";
			}
		}

		string ICriticalDependency.EscalationTeam
		{
			get
			{
				return "Networking";
			}
		}

		public bool TestCriticalDependency()
		{
			return NetworkAdapterProbe.GetNetworkInterfaceSetting(TracingContext.Default, null, CancellationToken.None);
		}

		public bool FixCriticalDependency()
		{
			return NetworkAdapterRecoveryResponder.FixAdapterSettings(NetworkAdapterProbe.MissingEntriesInNetworkAdapter.DnsAddresses, TracingContext.Default);
		}

		private const string Name = "DNSCriticalDependency";

		private const string escalationService = "Exchange";

		private const string escalationTeam = "Networking";

		private static TimeSpan RetestDelay = TimeSpan.FromSeconds(5.0);
	}
}
