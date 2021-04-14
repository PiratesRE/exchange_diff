using System;
using System.ComponentModel;
using System.ServiceProcess;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplayServiceCheck : ReplicationCheck
	{
		public ReplayServiceCheck(string serverName, IEventManager eventManager, string momeventsource) : base("ReplayService", CheckId.ReplayService, Strings.ReplayServiceCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName)
		{
		}

		public ReplayServiceCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold) : base("ReplayService", CheckId.ReplayService, Strings.ReplayServiceCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.ReplayServiceCheckHasRun)
			{
				ReplicationCheckGlobals.ReplayServiceCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "ReplayServiceCheck skipping because it has already been run once.");
				base.Skip();
			}
			if ((ReplicationCheckGlobals.ServerConfiguration & ServerConfig.Stopped) == ServerConfig.Stopped)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Stopped server! Skipping {0}.", base.Title);
				base.Skip();
			}
			this.CheckServiceOnNode(base.ServerName);
		}

		private void CheckServiceOnNode(string machine)
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(machine))
			{
				text = Environment.MachineName;
			}
			else
			{
				text = machine;
			}
			ServiceController serviceController;
			if (Cluster.StringIEquals(text, Environment.MachineName))
			{
				serviceController = new ServiceController("msexchangerepl");
			}
			else
			{
				serviceController = new ServiceController("msexchangerepl", text);
			}
			using (serviceController)
			{
				try
				{
					if (serviceController.Status != ServiceControllerStatus.Running)
					{
						base.FailContinue(Strings.ReplayServiceNotRunning(text));
					}
				}
				catch (Win32Exception ex)
				{
					base.FailContinue(Strings.ErrorReadingServiceState(text, ex.Message));
				}
				catch (InvalidOperationException ex2)
				{
					base.FailContinue(Strings.ErrorReadingServiceState(text, ex2.Message));
				}
			}
		}

		private const string ReplayService = "msexchangerepl";
	}
}
