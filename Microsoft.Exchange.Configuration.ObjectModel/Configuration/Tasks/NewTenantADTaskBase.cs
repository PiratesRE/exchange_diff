using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewTenantADTaskBase<TDataObject> : NewTaskBase<TDataObject> where TDataObject : IConfigurable, new()
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		internal ADSessionSettings SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		internal LazilyInitialized<SharedTenantConfigurationState> CurrentOrgState { get; set; }

		protected override void InternalStateReset()
		{
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "ADSessionSettings.FromCustomScopeSet", LoggerHelper.CmdletPerfMonitors))
			{
				this.sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			}
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			this.CurrentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(() => SharedConfiguration.GetSharedConfigurationState(base.CurrentOrganizationId));
			TaskLogger.LogExit();
		}

		protected virtual OrganizationId ResolveCurrentOrganization()
		{
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			this.sessionSettings = null;
		}

		private ADSessionSettings sessionSettings;
	}
}
