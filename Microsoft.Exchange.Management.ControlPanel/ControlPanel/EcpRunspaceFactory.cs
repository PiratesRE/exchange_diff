using System;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EcpRunspaceFactory : RbacRunspaceFactory
	{
		public EcpRunspaceFactory(InitialSessionStateSectionFactory issFactory) : base(issFactory)
		{
		}

		public EcpRunspaceFactory(InitialSessionStateSectionFactory issFactory, PSHostFactory hostFactory) : base(issFactory, hostFactory)
		{
		}

		internal override RunspaceServerSettings CreateRunspaceServerSettings()
		{
			string runspaceServerSettingsToken = this.GetRunspaceServerSettingsToken();
			if (runspaceServerSettingsToken == null)
			{
				return RunspaceServerSettings.CreateRunspaceServerSettings(false);
			}
			OrganizationId organizationId = RbacPrincipal.Current.RbacConfiguration.OrganizationId;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.OrgIdADSeverSettings.Enabled && organizationId != null && RbacPrincipal.Current.IsAdmin && !OrganizationId.ForestWideOrgId.Equals(organizationId) && !ADSessionSettings.IsForefrontObject(organizationId.PartitionId))
			{
				return RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(runspaceServerSettingsToken.ToLowerInvariant(), organizationId.PartitionId.ForestFQDN, false);
			}
			return RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(runspaceServerSettingsToken.ToLowerInvariant(), false);
		}

		protected override Runspace CreateRunspace(PSHost host)
		{
			Runspace result;
			using (new AverageTimePerfCounter(EcpPerfCounters.AveragePowerShellRunspaceCreation, EcpPerfCounters.AveragePowerShellRunspaceCreationBase, true))
			{
				using (EcpPerformanceData.CreateRunspace.StartRequestTimer())
				{
					Runspace runspace = base.CreateRunspace(host);
					EcpRunspaceFactory.runspaceCounters.Increment();
					result = runspace;
				}
			}
			return result;
		}

		protected override void OnRunspaceDisposed(Runspace runspace)
		{
			EcpRunspaceFactory.runspaceCounters.Decrement();
			base.OnRunspaceDisposed(runspace);
		}

		protected override void InitializeRunspace(Runspace runspace)
		{
			base.InitializeRunspace(runspace);
			runspace.SessionStateProxy.SetVariable("ExchangeDisableNotChangedWarning", true);
		}

		protected override string GetRunspaceServerSettingsToken()
		{
			if (Datacenter.IsMultiTenancyEnabled() && RbacPrincipal.Current.IsAdmin && !OrganizationId.ForestWideOrgId.Equals(RbacPrincipal.Current.RbacConfiguration.OrganizationId))
			{
				return RunspaceServerSettings.GetTokenForOrganization(RbacPrincipal.Current.RbacConfiguration.OrganizationId);
			}
			return base.GetRunspaceServerSettingsToken();
		}

		private static PerfCounterGroup runspaceCounters = new PerfCounterGroup(EcpPerfCounters.PowerShellRunspace, EcpPerfCounters.PowerShellRunspacePeak, EcpPerfCounters.PowerShellRunspaceTotal);
	}
}
