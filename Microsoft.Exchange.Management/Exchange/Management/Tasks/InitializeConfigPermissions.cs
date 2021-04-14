using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Initialize", "ExchangeConfigurationPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeConfigPermissions : SetupTaskBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IConfigurationSession session = (IConfigurationSession)ADSession.RescopeSessionToTenantSubTree(this.configurationSession);
			int count = DirectoryCommon.MailboxWriteAttrs.Count;
			SecurityIdentifier sid = this.exs.Sid;
			SecurityIdentifier sid2 = this.ets.Sid;
			this.configurationSession.GetOrgContainer();
			SecurityIdentifier[] array = new SecurityIdentifier[]
			{
				this.eoa.Sid,
				this.ets.Sid
			};
			SecurityIdentifier identity = new SecurityIdentifier("WD");
			SecurityIdentifier identity2 = new SecurityIdentifier("AN");
			SecurityIdentifier securityIdentifier = new SecurityIdentifier("AU");
			SecurityIdentifier securityIdentifier2 = new SecurityIdentifier("NS");
			SecurityIdentifier identity3 = new SecurityIdentifier("SY");
			SecurityIdentifier identity4 = new SecurityIdentifier(WellKnownSidType.AccountEnterpriseAdminsSid, this.rootDomain.Sid);
			SecurityIdentifier identity5 = new SecurityIdentifier(WellKnownSidType.AccountDomainAdminsSid, this.rootDomain.Sid);
			SecurityIdentifier identity6 = new SecurityIdentifier(WellKnownSidType.AccountSchemaAdminsSid, this.rootDomain.Sid);
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>(2);
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.DsReplicationSynchronize, ActiveDirectorySecurityInheritance.None));
			string distinguishedName = this.configurationSession.ConfigurationNamingContext.DistinguishedName;
			if (base.ShouldProcess(distinguishedName, Strings.InfoProcessAction(distinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, session, this.configurationSession.ConfigurationNamingContext, list.ToArray());
			}
			Guid schemaClassGuid = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchSmtpReceiveConnector");
			ADObjectId deletedObjectsContainer = this.configurationSession.DeletedObjectsContainer;
			base.WriteVerbose(Strings.InfoTakeOwnership(deletedObjectsContainer.DistinguishedName));
			DirectoryCommon.TakeOwnership(deletedObjectsContainer, null, session);
			List<ActiveDirectoryAccessRule> list2 = new List<ActiveDirectoryAccessRule>(100);
			list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list2.Add(new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.None));
			list2.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.GenericAll, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list2.Add(new ActiveDirectoryAccessRule(sid2, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.DeleteTree | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
			list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.GenericAll, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			if (this.epa != null)
			{
				list2.Add(new ActiveDirectoryAccessRule(this.epa.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			}
			if (this.delegatedSetupRG != null)
			{
				list2.Add(new ActiveDirectoryAccessRule(this.delegatedSetupRG.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			}
			if (base.ShouldProcess(this.meServicesContainer.DistinguishedName, Strings.InfoProcessAction(this.meServicesContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, session, this.meServicesContainer.Id, list2.ToArray());
				if (this.confUnitsContainer != null && !this.confUnitsContainer.Id.IsDescendantOf(this.configurationSession.ConfigurationNamingContext))
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.confUnitsContainer, list2.ToArray());
				}
			}
			Guid schemaClassGuid2 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchPrivateMDB");
			Guid schemaClassGuid3 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchPublicMDB");
			DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchAvailabilityConfig");
			Guid schemaClassGuid4 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchAvailabilityAddressSpace");
			Guid schemaPropertyGuid = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchAvailabilityUserPassword");
			Guid schemaClassGuid5 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "siteAddressing");
			Guid schemaClassGuid6 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "site");
			Guid schemaClassGuid7 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "siteLink");
			Guid schemaClassGuid8 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchEdgeSyncServiceConfig");
			Guid schemaClassGuid9 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchEdgeSyncMservConnector");
			Guid schemaClassGuid10 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchEdgeSyncEhfConnector");
			Guid schemaPropertyGuid2 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "siteFolderServer");
			Guid schemaPropertyGuid3 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "siteFolderGUID");
			Guid schemaPropertyGuid4 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchDatabaseCreated");
			Guid schemaPropertyGuid5 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchPatchMDB");
			Guid schemaPropertyGuid6 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchEDBOffline");
			Guid schemaPropertyGuid7 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchTransportSiteFlags");
			Guid schemaPropertyGuid8 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchPartnerId");
			Guid schemaPropertyGuid9 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchCost");
			Guid schemaPropertyGuid10 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchVersion");
			Guid schemaPropertyGuid11 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchLastAppliedRecipientFilter");
			Guid schemaPropertyGuid12 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchRecipientFilterFlags");
			Guid schemaPropertyGuid13 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "legacyExchangeDN");
			Guid schemaPropertyGuid14 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchOwningServer");
			Guid schemaPropertyGuid15 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchMinorPartnerId");
			Guid schemaPropertyGuid16 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msExchResponsibleForSites");
			List<ActiveDirectoryAccessRule> list3 = new List<ActiveDirectoryAccessRule>();
			list3.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			List<ActiveDirectoryAccessRule> list4 = new List<ActiveDirectoryAccessRule>();
			List<ActiveDirectoryAccessRule> list5 = new List<ActiveDirectoryAccessRule>();
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.SendAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.ReceiveAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreTransportAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreConstrainedDelegationExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadWriteAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.SendAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.EpiImpersonationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.TokenSerializationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.ReceiveAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreTransportAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreConstrainedDelegationExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadWriteAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list5.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreTransportAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.EpiImpersonationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity5, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.TokenSerializationRightGuid, ActiveDirectorySecurityInheritance.All));
			list5.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreConstrainedDelegationExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list5.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list5.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.StoreReadWriteAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ReadProperty, AccessControlType.Deny, schemaPropertyGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid4));
			list4.Add(new ActiveDirectoryAccessRule(securityIdentifier2, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.None));
			list4.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreatePublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreCreateNamedPropertiesExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid2));
			list4.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid3));
			list4.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreatePublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreCreateNamedPropertiesExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid2));
			list4.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid3));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.PublicInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.PersonalInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.ExchangeInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid5));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid3, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid2, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity6, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.EpiImpersonationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(identity6, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.TokenSerializationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.SendAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.ReceiveAsExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.TokenSerializationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Deny, WellKnownGuid.EpiImpersonationRightGuid, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreVisibleExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			if (this.epa != null)
			{
				list4.Add(new ActiveDirectoryAccessRule(this.epa.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreVisibleExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			}
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid4, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid6, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid5, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid13, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid14, ActiveDirectorySecurityInheritance.All));
			list4.Add(new ActiveDirectoryAccessRule(this.mas.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			List<SecurityIdentifier> list6 = new List<SecurityIdentifier>();
			list6.Add(this.eoa.Sid);
			if (this.epa != null)
			{
				list6.Add(this.epa.Sid);
			}
			foreach (SecurityIdentifier identity7 in list6)
			{
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.MailEnablePublicFolderGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreatePublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreateTopLevelPublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderACLExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderAdminACLExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderDeletedItemRetentionExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderExpiryExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderQuotasExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderReplicaListExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreAdminExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreCreateNamedPropertiesExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(identity7, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreVisibleExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			}
			for (int i = 0; i < count; i++)
			{
				Guid schemaPropertyGuid17 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, DirectoryCommon.MailboxWriteAttrs[i]);
				list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid17, ActiveDirectorySecurityInheritance.All));
			}
			if (base.ShouldProcess(this.autodiscoverContainer.DistinguishedName, Strings.InfoProcessAction(this.autodiscoverContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.autodiscoverContainer, list3.ToArray());
			}
			if (base.ShouldProcess(this.orgContainer.DistinguishedName, Strings.InfoProcessAction(this.orgContainer.DistinguishedName), null))
			{
				DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, null, this.orgContainer, list5.ToArray());
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.orgContainer, list4.ToArray());
			}
			if (this.confUnitsContainer != null && base.ShouldProcess(this.confUnitsContainer.DistinguishedName, Strings.InfoProcessAction(this.confUnitsContainer.DistinguishedName), null))
			{
				DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, null, this.confUnitsContainer, list5.ToArray());
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.confUnitsContainer, list4.ToArray());
			}
			ActiveDirectoryAccessRule[] aces = new ActiveDirectoryAccessRule[]
			{
				new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All)
			};
			if (base.ShouldProcess(this.addressingContainer.DistinguishedName, Strings.InfoProcessAction(this.addressingContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.addressingContainer, aces);
			}
			List<ActiveDirectoryAccessRule> list7 = new List<ActiveDirectoryAccessRule>();
			if (this.isMultiTenancy)
			{
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.ListChildren, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(this.era.Sid, ActiveDirectoryRights.ListChildren, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			}
			else
			{
				list7.Add(new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ListChildren, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
			}
			foreach (SecurityIdentifier identity8 in list6)
			{
				list7.Add(new ActiveDirectoryAccessRule(identity8, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid11, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity8, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid12, ActiveDirectorySecurityInheritance.All));
			}
			if (base.ShouldProcess(this.addressListsContainer.DistinguishedName, Strings.InfoProcessAction(this.addressListsContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.addressListsContainer, list7.ToArray());
			}
			if (this.isMultiTenancy)
			{
				base.ReplaceAddressListACEs(this.addressListsContainer.Id, securityIdentifier, new SecurityIdentifier[]
				{
					this.eoa.Sid,
					this.era.Sid
				});
			}
			if (base.ShouldProcess(this.offlineAddressListsContainer.DistinguishedName, Strings.InfoProcessAction(this.offlineAddressListsContainer.DistinguishedName), null))
			{
				ActiveDirectoryAccessRule[] aces2 = new ActiveDirectoryAccessRule[]
				{
					new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.DownloadOABExtendedRightGuid, ActiveDirectorySecurityInheritance.All)
				};
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.offlineAddressListsContainer, aces2);
			}
			List<ActiveDirectoryAccessRule> list8 = new List<ActiveDirectoryAccessRule>();
			foreach (SecurityIdentifier identity9 in list6)
			{
				list8.Add(new ActiveDirectoryAccessRule(identity9, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid11, ActiveDirectorySecurityInheritance.All));
				list8.Add(new ActiveDirectoryAccessRule(identity9, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid12, ActiveDirectorySecurityInheritance.All));
			}
			if (base.ShouldProcess(this.recipPoliciesContainer.DistinguishedName, Strings.InfoProcessAction(this.recipPoliciesContainer.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.recipPoliciesContainer, list8.ToArray());
			}
			List<ActiveDirectoryAccessRule> list9 = new List<ActiveDirectoryAccessRule>();
			Guid schemaClassGuid11 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchExchangeServer");
			foreach (SecurityIdentifier identity10 in list6)
			{
				list9.Add(new ActiveDirectoryAccessRule(identity10, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.RecipientUpdateExtendedRightGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid11));
			}
			if (base.ShouldProcess(this.administrativeGroup.Id.DistinguishedName, Strings.InfoProcessAction(this.administrativeGroup.Id.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.administrativeGroup, list9.ToArray());
			}
			List<ActiveDirectoryAccessRule> list10 = new List<ActiveDirectoryAccessRule>();
			for (int j = 0; j < array.Length; j++)
			{
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid7, ActiveDirectorySecurityInheritance.All, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid10, ActiveDirectorySecurityInheritance.All, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid8, ActiveDirectorySecurityInheritance.All, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid15, ActiveDirectorySecurityInheritance.All, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid16, ActiveDirectorySecurityInheritance.All, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid9, ActiveDirectorySecurityInheritance.All, schemaClassGuid7));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid10, ActiveDirectorySecurityInheritance.All, schemaClassGuid7));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid8, ActiveDirectorySecurityInheritance.Children, schemaClassGuid6));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid8));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid9, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid8));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid9));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid10, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid8));
				list10.Add(new ActiveDirectoryAccessRule(array[j], ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid10));
			}
			list10.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid8));
			list10.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid9));
			list10.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid10));
			list10.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid8));
			list10.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid9));
			list10.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid10));
			if (base.ShouldProcess(this.sitesContainer.Id.DistinguishedName, Strings.InfoProcessAction(this.sitesContainer.Id.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, session, this.sitesContainer.Id, list10.ToArray());
			}
			this.SetDeletedObjectsSecurityDescriptor(sid, ActiveDirectoryRights.ListChildren);
			this.SetDeletedObjectsSecurityDescriptor(sid2, ActiveDirectoryRights.GenericRead);
			this.SetDeletedObjectsSecurityDescriptor(this.eoa.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.WriteDacl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject);
			this.SetDeletedObjectsSecurityDescriptor(securityIdentifier2, ActiveDirectoryRights.ListChildren);
			ActiveDirectoryAccessRule[] aces3 = new ActiveDirectoryAccessRule[]
			{
				new ActiveDirectoryAccessRule(securityIdentifier, ActiveDirectoryRights.ListChildren, AccessControlType.Allow)
			};
			if (base.ShouldProcess(this.arraysContainer.Id.DistinguishedName, Strings.InfoProcessAction(this.arraysContainer.Id.DistinguishedName), null))
			{
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.arraysContainer, aces3);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (Datacenter.IsMultiTenancyEnabled())
			{
				this.isMultiTenancy = true;
			}
			else
			{
				this.isMultiTenancy = false;
			}
			this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
			if (this.exs == null)
			{
				base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.exs);
			this.eoa = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EoaWkGuid);
			if (this.eoa == null)
			{
				base.ThrowTerminatingError(new ExOrgAdminSGroupNotFoundException(WellKnownGuid.EoaWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.eoa);
			this.era = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EraWkGuid);
			if (this.era == null)
			{
				base.ThrowTerminatingError(new ExOrgReadAdminSGroupNotFoundException(WellKnownGuid.EraWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.eoa);
			this.mas = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.MaSWkGuid);
			if (this.mas == null)
			{
				base.ThrowTerminatingError(new ExOrgReadAdminSGroupNotFoundException(WellKnownGuid.MaSWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.mas);
			this.epa = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EpaWkGuid);
			if (this.epa != null)
			{
				base.LogReadObject(this.epa);
			}
			this.ets = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EtsWkGuid);
			if (this.ets == null)
			{
				base.ThrowTerminatingError(new ExTrustedSubsystemGroupNotFoundException(WellKnownGuid.EtsWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.ets);
			this.delegatedSetupRG = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.RgDelegatedSetupWkGuid);
			DelegatedSetupRoleGroupValueEnum delegatedSetupRoleGroupValue = VariantConfiguration.InvariantNoFlightingSnapshot.AD.DelegatedSetupRoleGroupValue.DelegatedSetupRoleGroupValue;
			if (delegatedSetupRoleGroupValue == DelegatedSetupRoleGroupValueEnum.NotExists || Datacenter.IsForefrontForOfficeDatacenter())
			{
				if (this.delegatedSetupRG != null)
				{
					base.ThrowTerminatingError(new ExRbacRoleGroupInMultiTenantException(WellKnownGuid.RgDelegatedSetupWkGuid, "Delegated Setup"), ErrorCategory.InvalidData, null);
				}
			}
			else if (delegatedSetupRoleGroupValue == DelegatedSetupRoleGroupValueEnum.Exists)
			{
				if (this.delegatedSetupRG == null)
				{
					base.ThrowTerminatingError(new ExRbacRoleGroupNotFoundException(WellKnownGuid.RgDelegatedSetupWkGuid, "Delegated Setup"), ErrorCategory.InvalidData, null);
				}
				base.LogReadObject(this.delegatedSetupRG);
			}
			this.autodiscoverContainer = this.ReadCriticalObjectFromRootOrg<ADContainer>(((ITopologyConfigurationSession)this.configurationSession).GetAutoDiscoverGlobalContainerId());
			this.orgContainer = this.configurationSession.GetOrgContainer();
			base.LogReadObject(this.orgContainer);
			if (Datacenter.IsMultiTenancyEnabled())
			{
				ADObjectId configurationUnitsRootForLocalForest = ADSession.GetConfigurationUnitsRootForLocalForest();
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(configurationUnitsRootForLocalForest), 1413, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InitializeExchangeConfigContainer.cs");
				this.confUnitsContainer = tenantConfigurationSession.Read<Container>(configurationUnitsRootForLocalForest);
				if (this.confUnitsContainer != null)
				{
					base.LogReadObject(this.confUnitsContainer);
				}
			}
			this.addressingContainer = this.ReadCriticalObject<Container>(this.orgContainer.Id.GetChildId("Addressing"));
			this.meServicesContainer = this.configurationSession.GetExchangeConfigurationContainer();
			base.LogReadObject(this.meServicesContainer);
			ADObjectId childId = this.orgContainer.Id.GetChildId("Address Lists Container");
			this.addressListsContainer = this.ReadCriticalObject<Container>(childId);
			this.offlineAddressListsContainer = this.ReadCriticalObject<Container>(childId.GetChildId("Offline Address Lists"));
			this.recipPoliciesContainer = this.ReadCriticalObject<RecipientPoliciesContainer>(this.orgContainer.Id.GetChildId("Recipient Policies"));
			this.sitesContainer = this.ReadCriticalObject<SitesContainer>(this.configurationSession.ConfigurationNamingContext.GetChildId("Sites"));
			this.administrativeGroup = ((ITopologyConfigurationSession)this.configurationSession).GetAdministrativeGroup();
			base.LogReadObject(this.administrativeGroup);
			this.arraysContainer = this.ReadCriticalObject<Container>(ClientAccessArray.GetParentContainer((ITopologyConfigurationSession)this.configurationSession));
			TaskLogger.LogExit();
		}

		private void SetDeletedObjectsSecurityDescriptor(SecurityIdentifier sid, ActiveDirectoryRights adr)
		{
			ADObjectId deletedObjectsContainer = this.configurationSession.DeletedObjectsContainer;
			if (base.ShouldProcess(deletedObjectsContainer.DistinguishedName, Strings.InfoProcessAction(sid.ToString()), null))
			{
				ActiveDirectoryAccessRule activeDirectoryAccessRule = new ActiveDirectoryAccessRule(sid, adr, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, (IConfigurationSession)ADSession.RescopeSessionToTenantSubTree(this.configurationSession), deletedObjectsContainer, new ActiveDirectoryAccessRule[]
				{
					activeDirectoryAccessRule
				});
			}
		}

		private TDirectoryObject ReadCriticalObjectFromRootOrg<TDirectoryObject>(ADObjectId id) where TDirectoryObject : ADConfigurationObject, new()
		{
			return this.ReadCriticalObject<TDirectoryObject>(id, this.rootOrgSession);
		}

		private TDirectoryObject ReadCriticalObject<TDirectoryObject>(ADObjectId id) where TDirectoryObject : ADConfigurationObject, new()
		{
			return this.ReadCriticalObject<TDirectoryObject>(id, this.configurationSession);
		}

		private TDirectoryObject ReadCriticalObject<TDirectoryObject>(ADObjectId id, IConfigurationSession session) where TDirectoryObject : ADConfigurationObject, new()
		{
			TDirectoryObject tdirectoryObject = session.Read<TDirectoryObject>(id);
			if (tdirectoryObject == null)
			{
				base.ThrowTerminatingError(new DirectoryObjectNotFoundException(id.DistinguishedName), (ErrorCategory)1001, null);
			}
			base.LogReadObject(tdirectoryObject);
			return tdirectoryObject;
		}

		private ADContainer autodiscoverContainer;

		private Organization orgContainer;

		private Container confUnitsContainer;

		private ExchangeConfigurationContainer meServicesContainer;

		private Container addressingContainer;

		private Container addressListsContainer;

		private Container offlineAddressListsContainer;

		private SitesContainer sitesContainer;

		private RecipientPoliciesContainer recipPoliciesContainer;

		private AdministrativeGroup administrativeGroup;

		private Container arraysContainer;

		private bool isMultiTenancy;

		private ADGroup exs;

		private ADGroup ets;

		private ADGroup eoa;

		private ADGroup era;

		private ADGroup epa;

		private ADGroup mas;

		private ADGroup delegatedSetupRG;
	}
}
