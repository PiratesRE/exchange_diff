using System;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class ReportingWebServiceRunspaceFactory : RunspaceFactory
	{
		public ReportingWebServiceRunspaceFactory() : base(new ReportingWebServiceInitialSessionStateFactory(), ReportingWebServiceHost.Factory)
		{
		}

		protected override Runspace CreateRunspace(PSHost host)
		{
			Runspace runspace2;
			using (new AverageTimePerfCounter(RwsPerfCounters.AveragePowerShellRunspaceCreation, RwsPerfCounters.AveragePowerShellRunspaceCreationBase, true))
			{
				Runspace runspace = null;
				ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.PowerShellCreateRunspaceLatency, delegate
				{
					runspace = this.<>n__FabricatedMethod5(host);
				});
				ReportingWebServiceRunspaceFactory.runspaceCounters.Increment();
				runspace2 = runspace;
			}
			return runspace2;
		}

		protected override void InitializeRunspace(Runspace runspace)
		{
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.InitializeRunspaceLatency, delegate
			{
				this.<>n__FabricatedMethod9(runspace);
				if (ReportingWebServiceRunspaceFactory.RunspaceServerSettingsEnabled.Value)
				{
					this.SetRunspaceServerSettings(runspace);
					return;
				}
				ElapsedTimeWatcher.WatchMessage("CRSS", "Skip");
			});
			ElapsedTimeWatcher.WatchMessage("TOKEN", this.GetUserToken());
		}

		protected override void ConfigureRunspace(Runspace runspace)
		{
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.ConfigureRunspaceLatency, delegate
			{
				this.<>n__FabricatedMethodd(runspace);
			});
		}

		protected override void OnRunspaceDisposed(Runspace runspace)
		{
			ReportingWebServiceRunspaceFactory.runspaceCounters.Decrement();
			base.OnRunspaceDisposed(runspace);
		}

		private void SetRunspaceServerSettings(Runspace runspace)
		{
			try
			{
				runspace.SessionStateProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, this.CreateRunspaceServerSettings());
			}
			catch (ADTransientException)
			{
				throw;
			}
			catch (ADExternalException)
			{
				throw;
			}
		}

		private RunspaceServerSettings CreateRunspaceServerSettings()
		{
			RunspaceServerSettings settings = null;
			string token = this.GetUserToken();
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.CreateRunspaceServerSettingsLatency, delegate
			{
				settings = ((token != null) ? RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(token.ToLowerInvariant(), false) : RunspaceServerSettings.CreateRunspaceServerSettings(false));
			});
			return settings;
		}

		private string GetUserToken()
		{
			if (RbacPrincipal.Current.ExecutingUserId == null)
			{
				return RbacPrincipal.Current.CacheKeys[0];
			}
			return RbacPrincipal.Current.ExecutingUserId.ToString();
		}

		private static readonly BoolAppSettingsEntry RunspaceServerSettingsEnabled = new BoolAppSettingsEntry("RunspaceServerSettingsEnabled", true, ExTraceGlobals.RunspaceConfigTracer);

		private static readonly PerfCounterGroup runspaceCounters = new PerfCounterGroup(RwsPerfCounters.PowerShellRunspace, RwsPerfCounters.PowerShellRunspacePeak, RwsPerfCounters.PowerShellRunspaceTotal);
	}
}
