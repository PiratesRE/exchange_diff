using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "ExchangeUniversalGroups", SupportsShouldProcess = true)]
	public sealed class UninstallExchangeUniversalGroups : SetupTaskBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.RemoveGroupByWKGuid(WellKnownGuid.ExSWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.MaSWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EraWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EmaWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EpaWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.E3iWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EwpWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EtsWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EahoWkGuid);
			this.RemoveGroupByWKGuid(WellKnownGuid.EfomgWkGuid);
			foreach (RoleGroupDefinition roleGroupDefinition in InitializeExchangeUniversalGroups.RoleGroupsToCreate())
			{
				if (!roleGroupDefinition.RoleGroupGuid.Equals(WellKnownGuid.EoaWkGuid))
				{
					this.RemoveGroupByWKGuid(roleGroupDefinition.RoleGroupGuid);
				}
			}
			try
			{
				this.RemoveGroupByWKGuid(WellKnownGuid.EoaWkGuid);
			}
			catch (ADOperationException ex)
			{
				this.WriteWarning(Strings.NeedManuallyRemoveEOA(ex.Message));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.configContainer = this.configurationSession.Read<ConfigurationContainer>(this.configurationSession.ConfigurationNamingContext);
			if (this.configContainer == null)
			{
				base.ThrowTerminatingError(new ConfigurationContainerNotFoundException(), ErrorCategory.InvalidData, null);
			}
			this.exchangeConfigContainer = this.configurationSession.GetExchangeConfigurationContainer();
			if (this.exchangeConfigContainer == null)
			{
				base.ThrowTerminatingError(new MicrosoftExchangeContainerNotFoundException(), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}

		private void RemoveGroupByWKGuid(Guid wkGuid)
		{
			ADGroup adgroup = base.ResolveExchangeGroupGuid<ADGroup>(wkGuid);
			DNWithBinary dnwithBinary = DirectoryCommon.FindWellKnownObjectEntry(this.exchangeConfigContainer.OtherWellKnownObjects, wkGuid);
			if (dnwithBinary != null && this.exchangeConfigContainer.OtherWellKnownObjects.Remove(dnwithBinary))
			{
				this.configurationSession.Save(this.exchangeConfigContainer);
			}
			dnwithBinary = DirectoryCommon.FindWellKnownObjectEntry(this.configContainer.OtherWellKnownObjects, wkGuid);
			if (dnwithBinary != null && this.configContainer.OtherWellKnownObjects.Remove(dnwithBinary))
			{
				this.configurationSession.Save(this.configContainer);
			}
			if (adgroup != null)
			{
				adgroup.Session.Delete(adgroup);
			}
		}

		private ConfigurationContainer configContainer;

		private ExchangeConfigurationContainer exchangeConfigContainer;
	}
}
