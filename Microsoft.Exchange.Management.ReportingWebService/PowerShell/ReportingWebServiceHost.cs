using System;
using System.Management.Automation.Host;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class ReportingWebServiceHost : RunspaceHost
	{
		public ReportingWebServiceHost() : base(true)
		{
		}

		public override string Name
		{
			get
			{
				return "Reporting Web Service";
			}
		}

		public override void Activate()
		{
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.ActivateReportingWebServiceHostLatency, delegate
			{
				ReportingWebServiceHost.activeRunspaceCounters.Increment();
				this.averageActiveRunspace.Start();
				this.<>n__FabricatedMethod1();
			});
		}

		public override void Deactivate()
		{
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.DeactivateReportingWebServiceHostLatency, delegate
			{
				this.averageActiveRunspace.Stop();
				ReportingWebServiceHost.activeRunspaceCounters.Decrement();
				this.<>n__FabricatedMethod3();
			});
		}

		private const string RunspaceName = "Reporting Web Service";

		public static readonly PSHostFactory Factory = new ReportingWebServiceHost.ReportingWebServiceHostFactory();

		private static readonly PerfCounterGroup activeRunspaceCounters = new PerfCounterGroup(RwsPerfCounters.ActiveRunspaces, RwsPerfCounters.ActiveRunspacesPeak, RwsPerfCounters.ActiveRunspacesTotal);

		private readonly AverageTimePerfCounter averageActiveRunspace = new AverageTimePerfCounter(RwsPerfCounters.AverageActiveRunspace, RwsPerfCounters.AverageActiveRunspaceBase);

		internal class ReportingWebServiceHostFactory : PSHostFactory
		{
			public override PSHost CreatePSHost()
			{
				ReportingWebServiceHost host = null;
				ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.CreatePSHostLatency, delegate
				{
					host = new ReportingWebServiceHost();
				});
				return host;
			}
		}
	}
}
