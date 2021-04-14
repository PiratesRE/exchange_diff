using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Initialize", "TenantConfigurationPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeTenantConfigurationPermissions : SetupTaskBase
	{
		[Parameter(Mandatory = true)]
		public override OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			SecurityIdentifier identity = new SecurityIdentifier("AU");
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
			list.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ListChildren, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			if (base.ShouldProcess(this.addressListsContainer.DistinguishedName, Strings.InfoProcessAction(this.addressListsContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.addressListsContainer, list.ToArray());
			}
			if (base.ShouldProcess(this.offlineAddressListsContainer.DistinguishedName, Strings.InfoProcessAction(this.offlineAddressListsContainer.DistinguishedName), null))
			{
				ActiveDirectoryAccessRule[] aces = new ActiveDirectoryAccessRule[]
				{
					new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.DownloadOABExtendedRightGuid, ActiveDirectorySecurityInheritance.All)
				};
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.offlineAddressListsContainer, aces);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.tenantCU = this.configurationSession.Read<ExchangeConfigurationUnit>(this.organization.ConfigurationUnit);
			if (this.tenantCU == null)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorOrganizationNotFound(this.organization.ConfigurationUnit.ToString())), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.tenantCU);
			ADObjectId childId = this.tenantCU.Id.GetChildId("Address Lists Container");
			this.addressListsContainer = this.configurationSession.Read<Container>(childId);
			if (this.addressListsContainer == null)
			{
				base.ThrowTerminatingError(new DirectoryObjectNotFoundException(childId.DistinguishedName), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.addressListsContainer);
			ADObjectId childId2 = childId.GetChildId("Offline Address Lists");
			this.offlineAddressListsContainer = this.configurationSession.Read<Container>(childId2);
			if (this.offlineAddressListsContainer == null)
			{
				base.ThrowTerminatingError(new DirectoryObjectNotFoundException(childId2.DistinguishedName), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.offlineAddressListsContainer);
			TaskLogger.LogExit();
		}

		private Container addressListsContainer;

		private Container offlineAddressListsContainer;

		private ExchangeConfigurationUnit tenantCU;
	}
}
