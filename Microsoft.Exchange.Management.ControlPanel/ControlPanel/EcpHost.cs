using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.ServiceModel;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpHost : RunspaceHost
	{
		public override void Activate()
		{
			this.runspaceActivationsTimer = EcpPerformanceData.ActiveRunspace.StartRequestTimer();
			EcpHost.activeRunspaceCounters.Increment();
			this.averageActiveRunspace.Start();
			this.webServiceOperation = OperationContext.Current;
			base.Activate();
		}

		public override void Deactivate()
		{
			this.averageActiveRunspace.Stop();
			EcpHost.activeRunspaceCounters.Decrement();
			this.webServiceOperation = null;
			base.Deactivate();
			this.runspaceActivationsTimer.Dispose();
			this.runspaceActivationsTimer = null;
		}

		public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			if (this.webServiceOperation != null)
			{
				return new Dictionary<string, PSObject>();
			}
			return base.Prompt(caption, message, descriptions);
		}

		public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			throw new ShouldContinueException(message);
		}

		public override void WriteWarningLine(string message)
		{
		}

		public override string Name
		{
			get
			{
				return "Exchange Control Panel";
			}
		}

		private const string RunspaceName = "Exchange Control Panel";

		private static PerfCounterGroup activeRunspaceCounters = new PerfCounterGroup(EcpPerfCounters.ActiveRunspaces, EcpPerfCounters.ActiveRunspacesPeak, EcpPerfCounters.ActiveRunspacesTotal);

		private OperationContext webServiceOperation;

		private AverageTimePerfCounter averageActiveRunspace = new AverageTimePerfCounter(EcpPerfCounters.AverageActiveRunspace, EcpPerfCounters.AverageActiveRunspaceBase);

		private IDisposable runspaceActivationsTimer;

		public static readonly PSHostFactory Factory = new EcpHost.EcpHostFactory();

		private class EcpHostFactory : PSHostFactory
		{
			public override PSHost CreatePSHost()
			{
				return new EcpHost();
			}
		}
	}
}
