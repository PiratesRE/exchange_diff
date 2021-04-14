using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AvailabilityConfig", SupportsShouldProcess = true)]
	public sealed class NewAvailabilityConfig : NewMultitenancyFixedNameSystemConfigurationObjectTask<AvailabilityConfig>
	{
		[Parameter(Mandatory = true)]
		public SecurityPrincipalIdParameter OrgWideAccount
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["OrgWideAccount"];
			}
			set
			{
				base.Fields["OrgWideAccount"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			AvailabilityConfig availabilityConfig = null;
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			AvailabilityConfig[] array = configurationSession.Find<AvailabilityConfig>(orgContainerId, QueryScope.SubTree, ADObject.ObjectClassFilter("msExchAvailabilityConfig"), null, 1);
			if (array != null && array.Length == 1)
			{
				availabilityConfig = array[0];
			}
			if (availabilityConfig == null)
			{
				availabilityConfig = (AvailabilityConfig)base.PrepareDataObject();
			}
			availabilityConfig.Name = AvailabilityConfig.ContainerName;
			availabilityConfig.SetId((IConfigurationSession)base.DataSession, availabilityConfig.Name);
			availabilityConfig.OrgWideAccount = this.ValidateUser((SecurityPrincipalIdParameter)base.Fields["OrgWideAccount"]);
			ADDomain addomain = ADForest.GetLocalForest().FindRootDomain(true);
			if (addomain == null)
			{
				base.ThrowTerminatingError(new RootDomainNotFoundException(), ErrorCategory.InvalidData, null);
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(addomain.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 105, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Availability\\NewAvailabilityConfig.cs");
			IConfigurationSession configurationSession2 = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 111, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Availability\\NewAvailabilityConfig.cs");
			this.exchangeServerGroup = null;
			try
			{
				this.exchangeServerGroup = tenantOrRootOrgRecipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, configurationSession2.ConfigurationNamingContext);
			}
			catch (ADReferralException)
			{
			}
			if (this.exchangeServerGroup == null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession2 = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 131, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Availability\\NewAvailabilityConfig.cs");
				tenantOrRootOrgRecipientSession2.UseGlobalCatalog = true;
				this.exchangeServerGroup = tenantOrRootOrgRecipientSession2.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, configurationSession2.ConfigurationNamingContext);
			}
			if (this.exchangeServerGroup == null)
			{
				base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
			return availabilityConfig;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			AvailabilityConfig availabilityConfig = (AvailabilityConfig)base.DataSession.Read<AvailabilityConfig>(this.DataObject.Id);
			if (availabilityConfig == null)
			{
				base.InternalProcessRecord();
				if (base.HasErrors)
				{
					return;
				}
				availabilityConfig = (AvailabilityConfig)base.DataSession.Read<AvailabilityConfig>(this.DataObject.Id);
				if (availabilityConfig == null)
				{
					base.WriteError(new AvailabilityConfigReadException(this.DataObject.Id.ToDNString()), ErrorCategory.ReadError, this.DataObject.Identity);
					return;
				}
			}
			try
			{
				InstallAvailabilityConfig.SetAvailabilityAces(this.exchangeServerGroup.Sid, availabilityConfig, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			catch (SecurityDescriptorAccessDeniedException exception)
			{
				base.WriteError(exception, ErrorCategory.PermissionDenied, null);
			}
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAvailabilityConfig(this.OrgWideAccount.ToString());
			}
		}

		private ADObjectId ValidateUser(SecurityPrincipalIdParameter principalId)
		{
			if (principalId == null)
			{
				return null;
			}
			IEnumerable<ADRecipient> objects = principalId.GetObjects<ADRecipient>(null, base.TenantGlobalCatalogSession);
			ADObjectId result;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorUserNotFound(principalId.ToString())), ErrorCategory.ObjectNotFound, null);
				}
				ADObjectId adobjectId = (ADObjectId)enumerator.Current.Identity;
				if (enumerator.MoveNext())
				{
					base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorUserNotUnique(principalId.ToString())), ErrorCategory.InvalidData, null);
				}
				this.WriteWarning(Strings.AccountPrivilegeWarning);
				result = adobjectId;
			}
			return result;
		}

		private const string propOrgWideAccount = "OrgWideAccount";

		private ADGroup exchangeServerGroup;
	}
}
