using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("initialize", "DomainPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeDomainPermissions : DomainSetupTaskBase
	{
		[Parameter(Mandatory = false)]
		public bool CreateTenantRoot
		{
			get
			{
				return (bool)(base.Fields[false] ?? false);
			}
			set
			{
				base.Fields[false] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CreateMsoSyncRoot
		{
			get
			{
				return (bool)(base.Fields["CreateMsoSyncRoot"] ?? false);
			}
			set
			{
				base.Fields["CreateMsoSyncRoot"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsManagementForest
		{
			get
			{
				return (bool)(base.Fields["IsManagementForest"] ?? false);
			}
			set
			{
				base.Fields["IsManagementForest"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			SecurityIdentifier sid = this.exs.Sid;
			SecurityIdentifier identity = new SecurityIdentifier("AU");
			SecurityIdentifier identity2 = new SecurityIdentifier("NS");
			List<SecurityIdentifier> list = new List<SecurityIdentifier>();
			if (this.epa != null)
			{
				list.Add(this.epa.Sid);
			}
			list.Add(this.ets.Sid);
			list.Add(this.eoa.Sid);
			Guid[] array = new Guid[]
			{
				WellKnownGuid.ExchangeInfoPropSetGuid,
				WellKnownGuid.ExchangePersonalInfoPropSetGuid,
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "legacyExchangeDN"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "displayName"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "displayNamePrintable"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "publicDelegates"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "garbageCollPeriod"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "textEncodedORAddress"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "showInAddressBook"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "proxyAddresses"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "mail"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "pFContacts"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "msDS-PhoneticDisplayName"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "cn"),
				DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "name")
			};
			foreach (ADDomain addomain in this.domains)
			{
				this.domainConfigurationSession.DomainController = null;
				ADContainer adminSDHolder = DirectoryCommon.GetAdminSDHolder(addomain, this.domainConfigurationSession);
				base.LogReadObject(adminSDHolder);
				List<ActiveDirectoryAccessRule> list2 = new List<ActiveDirectoryAccessRule>();
				List<ActiveDirectoryAccessRule> list3 = new List<ActiveDirectoryAccessRule>();
				List<ActiveDirectoryAccessRule> list4 = new List<ActiveDirectoryAccessRule>();
				List<ActiveDirectoryAccessRule> list5 = new List<ActiveDirectoryAccessRule>();
				ADGroup dnsadmins = DirectoryCommon.GetDNSAdmins(addomain, this.domainConfigurationSession);
				if (dnsadmins != null)
				{
					base.LogReadObject(dnsadmins);
					List<ActiveDirectoryAccessRule> list6 = new List<ActiveDirectoryAccessRule>();
					list6.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Deny, DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "member"), ActiveDirectorySecurityInheritance.All));
					list6.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Deny, DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "managedBy"), ActiveDirectorySecurityInheritance.All));
					if (!this.adSplitPermissionMode)
					{
						DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, dnsadmins, list6.ToArray());
					}
					else
					{
						DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, null, dnsadmins, list6.ToArray());
					}
					base.LogWriteObject(dnsadmins);
				}
				Guid schemaPropertyGuid = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "servicePrincipalName");
				list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Deny, schemaPropertyGuid, ActiveDirectorySecurityInheritance.All));
				list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, WellKnownGuid.ExchangeInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, WellKnownGuid.ExchangePersonalInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				list2.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, WellKnownGuid.ExchangeInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				foreach (string prop in DirectoryCommon.MailboxReadAttrs)
				{
					Guid schemaPropertyGuid2 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, prop);
					list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, schemaPropertyGuid2, ActiveDirectorySecurityInheritance.All));
				}
				foreach (string prop2 in DirectoryCommon.MailboxWriteAttrs)
				{
					Guid schemaPropertyGuid3 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, prop2);
					list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid3, ActiveDirectorySecurityInheritance.All));
				}
				if (!Datacenter.IsMultiTenancyEnabled())
				{
					list3.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ChangePasswordExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
					list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ChangePasswordExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				}
				else
				{
					list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ChangePasswordExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				}
				foreach (string prop3 in DirectoryCommon.ExtraWriteAttrsForEOA)
				{
					Guid schemaPropertyGuid4 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, prop3);
					list2.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid4, ActiveDirectorySecurityInheritance.All));
					list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid4, ActiveDirectorySecurityInheritance.All));
				}
				list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "memberOf"), ActiveDirectorySecurityInheritance.All));
				list2.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, WellKnownGuid.ExchangePersonalInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				Guid schemaClassGuid = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "user");
				Guid schemaClassGuid2 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "inetOrgPerson");
				Guid schemaClassGuid3 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchActiveSyncDevices");
				list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren, AccessControlType.Allow, schemaClassGuid3, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
				if (!Datacenter.IsMicrosoftHostedOnly(false))
				{
					list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren, AccessControlType.Allow, schemaClassGuid3, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid2));
				}
				list2.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.DsReplicationSynchronize, ActiveDirectorySecurityInheritance.None));
				Guid schemaClassGuid4 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchDynamicDistributionList");
				SecurityIdentifier[] array3 = new SecurityIdentifier[]
				{
					this.eoa.Sid,
					this.ets.Sid
				};
				foreach (SecurityIdentifier identity3 in array3)
				{
					list2.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
					list2.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.ExchangeInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
					list2.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.ExchangePersonalInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
					list2.Add(new ActiveDirectoryAccessRule(identity3, ActiveDirectoryRights.GenericAll, AccessControlType.Allow, schemaClassGuid4, ActiveDirectorySecurityInheritance.All));
				}
				Guid schemaClassGuid5 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "group");
				Guid schemaClassGuid6 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "contact");
				Guid schemaClassGuid7 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "organizationalUnit");
				Guid schemaClassGuid8 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "computer");
				list4.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid5));
				list3.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid5));
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid);
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid2);
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid5);
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid6);
				foreach (string prop4 in DirectoryCommon.TrustedSubsystemWriteAttrs)
				{
					Guid schemaPropertyGuid5 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, prop4);
					list3.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid5, ActiveDirectorySecurityInheritance.All));
					list4.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid5, ActiveDirectorySecurityInheritance.All));
					(this.adSplitPermissionMode ? list3 : list5).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, schemaPropertyGuid5, ActiveDirectorySecurityInheritance.All));
				}
				list3.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All, schemaClassGuid));
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.All, schemaClassGuid2));
				list4.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid2));
				list4.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.All));
				if (!this.adSplitPermissionMode)
				{
					list5.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
					list5.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.WriteDacl, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid2));
				}
				list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.PublicInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, WellKnownGuid.PersonalInfoPropSetGuid, ActiveDirectorySecurityInheritance.All));
				if (this.delegatedSetupRG != null)
				{
					list2.Add(new ActiveDirectoryAccessRule(this.delegatedSetupRG.Sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, WellKnownGuid.UserAccountRestrictionsPropSetGuid, ActiveDirectorySecurityInheritance.All));
				}
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid7);
				this.SetCreateDeletePermissionOnObjectClass(list3, list4, list5, schemaClassGuid8);
				list3.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				(this.adSplitPermissionMode ? list3 : list5).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All, schemaClassGuid));
				(this.adSplitPermissionMode ? list3 : list5).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.All, schemaClassGuid2));
				if (Datacenter.IsMicrosoftHostedOnly(false))
				{
					list2.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.DsReplicationGetChanges, ActiveDirectorySecurityInheritance.None));
				}
				foreach (Guid objectType in DirectoryCommon.PasswordExtendedRights)
				{
					list4.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, objectType, ActiveDirectorySecurityInheritance.All, schemaClassGuid));
					(this.adSplitPermissionMode ? list3 : list5).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, objectType, ActiveDirectorySecurityInheritance.All, schemaClassGuid));
				}
				if (base.ShouldProcess(addomain.DistinguishedName, Strings.InfoProcessAction(this.exs.DisplayName), null))
				{
					DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, null, addomain, list3.ToArray());
					base.LogWriteObject(addomain);
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, addomain, list2.ToArray());
					base.LogWriteObject(addomain);
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, addomain, list5.ToArray());
					base.LogWriteObject(addomain);
				}
				if (base.ShouldProcess(adminSDHolder.DistinguishedName, Strings.InfoProcessAction(this.exs.DisplayName), null))
				{
					if (!Datacenter.IsMultiTenancyEnabled())
					{
						DirectoryCommon.RemoveAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, null, adminSDHolder, list4.ToArray());
						base.LogWriteObject(adminSDHolder);
					}
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, adminSDHolder, list2.ToArray());
					base.LogWriteObject(adminSDHolder);
				}
				if (this.CreateTenantRoot)
				{
					this.CreateRootTenantOU(addomain);
				}
				if (this.CreateMsoSyncRoot)
				{
					this.CreateMsoSyncContainer(addomain);
				}
				MesoContainer mesoContainer = this.CreateOrFindMesoContainer(addomain);
				if (mesoContainer == null)
				{
					base.WriteError(new MesoContainerNotFoundException(addomain.DistinguishedName), ErrorCategory.InvalidData, null);
				}
				this.CreateMonitoringMailboxContainer(mesoContainer);
				Guid schemaPropertyGuid6 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "garbageCollPeriod");
				Guid schemaPropertyGuid7 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "adminDisplayName");
				Guid schemaPropertyGuid8 = DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, "modifyTimeStamp");
				Guid schemaClassGuid9 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "publicFolder");
				Guid schemaClassGuid10 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, "msExchSystemMailbox");
				List<ActiveDirectoryAccessRule> list7 = new List<ActiveDirectoryAccessRule>();
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.DeleteTree | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Deny, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, schemaClassGuid9, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid9));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, schemaClassGuid10, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid10));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, schemaClassGuid10, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				foreach (Guid objectType2 in DirectoryCommon.PasswordExtendedRights)
				{
					list7.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, objectType2, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
				}
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, schemaClassGuid10, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid10));
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid));
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, schemaClassGuid10, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadControl, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, schemaPropertyGuid6, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, schemaPropertyGuid7, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, schemaPropertyGuid8, ActiveDirectorySecurityInheritance.All));
				list7.Add(new ActiveDirectoryAccessRule(identity2, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				foreach (SecurityIdentifier identity4 in list)
				{
					foreach (Guid objectType3 in array)
					{
						list7.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, objectType3, ActiveDirectorySecurityInheritance.Descendents, schemaClassGuid9));
					}
					list7.Add(new ActiveDirectoryAccessRule(identity4, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				}
				if (base.ShouldProcess(mesoContainer.DistinguishedName, Strings.InfoProcessAction(this.exs.DisplayName), null))
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, mesoContainer, list7.ToArray());
					base.LogWriteObject(mesoContainer);
				}
				this.AddSaclRight(addomain, sid);
				ADObjectId deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(addomain.Id);
				if (base.ShouldProcess(deletedObjectsContainer.DistinguishedName, Strings.InfoProcessAction(this.exs.DisplayName), null))
				{
					DirectoryCommon.TakeOwnership(deletedObjectsContainer, null, this.domainConfigurationSession);
					ActiveDirectoryAccessRule activeDirectoryAccessRule = new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ListChildren, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
					ActiveDirectoryAccessRule activeDirectoryAccessRule2 = new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.domainConfigurationSession, deletedObjectsContainer, new ActiveDirectoryAccessRule[]
					{
						activeDirectoryAccessRule,
						activeDirectoryAccessRule2
					});
				}
				if (Datacenter.IsMicrosoftHostedOnly(false) && !this.IsManagementForest)
				{
					ADPasswordSettings adpasswordSettings = this.CreateOrFindExchangeDatacenterPSO(addomain);
					if (base.ShouldProcess(adpasswordSettings.Id.DistinguishedName, Strings.InfoProcessAction(this.exs.DisplayName), null))
					{
						ActiveDirectoryAccessRule activeDirectoryAccessRule3 = new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
						DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.domainConfigurationSession, adpasswordSettings.Id, new ActiveDirectoryAccessRule[]
						{
							activeDirectoryAccessRule3
						});
					}
				}
			}
			foreach (ADDomain addomain2 in this.domains)
			{
				MesoContainer mesoContainer2 = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindMesoContainer(addomain2);
				if (mesoContainer2 == null)
				{
					base.WriteError(new MesoContainerNotFoundException(addomain2.DistinguishedName), ErrorCategory.InvalidData, null);
				}
				this.recipientSession.DomainController = null;
				ADGroup adgroup = DirectoryCommon.FindE12DomainServersGroup(this.recipientSession, mesoContainer2);
				if (adgroup != null)
				{
					base.LogReadObject(adgroup);
				}
				else
				{
					adgroup = new ADGroup(this.recipientSession, "Exchange Install Domain Servers", mesoContainer2.Id, GroupTypeFlags.Global | GroupTypeFlags.SecurityEnabled);
					adgroup.DisplayName = "Exchange Install Domain Servers";
					MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
					multiValuedProperty.Add("This group is used during Exchange setup and is not intended to be used for other purposes.");
					adgroup[ADRecipientSchema.Description] = multiValuedProperty;
					if (base.ShouldProcess(Strings.InfoCreateE12DSGroup(adgroup.Id.DistinguishedName)))
					{
						adgroup.SetExchangeVersion(null);
						this.recipientSession.Save(adgroup);
						base.LogWriteObject(adgroup);
						adgroup = (ADGroup)this.recipientSession.Read(adgroup.Id);
						base.LogReadObject(adgroup);
					}
				}
				List<ActiveDirectoryAccessRule> list8 = new List<ActiveDirectoryAccessRule>();
				list8.Add(new ActiveDirectoryAccessRule(this.eoa.Sid, ActiveDirectoryRights.GenericAll, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				if (base.ShouldProcess(adgroup.DistinguishedName, Strings.InfoProcessAction(adgroup.DisplayName), null))
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, adgroup, list8.ToArray());
					base.LogWriteObject(adgroup);
				}
			}
			Thread.Sleep(15000);
			ADDomain[] array6 = this.domains;
			List<ADDomain> list9 = new List<ADDomain>();
			for (int k = 0; k < 2; k++)
			{
				foreach (ADDomain addomain3 in array6)
				{
					MesoContainer mesoContainer3 = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindMesoContainer(addomain3);
					if (mesoContainer3 == null)
					{
						base.WriteError(new MesoContainerNotFoundException(addomain3.DistinguishedName), ErrorCategory.InvalidData, null);
					}
					this.recipientSession.DomainController = null;
					ADGroup adgroup2 = DirectoryCommon.FindE12DomainServersGroup(this.recipientSession, mesoContainer3);
					base.LogReadObject(adgroup2);
					this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
					if (this.exs == null)
					{
						base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.InvalidData, null);
					}
					base.LogReadObject(this.exs);
					if (!this.exs.Members.Contains(adgroup2.Id))
					{
						this.exs.Members.Add(adgroup2.Id);
					}
					SecurityIdentifier sId = new SecurityIdentifier(WellKnownSidType.BuiltinAuthorizationAccessSid, addomain3.Sid);
					ADGroup adgroup3 = (ADGroup)this.recipientSession.FindBySid(sId);
					if (adgroup3 == null)
					{
						this.WriteWarning(Strings.ErrorWAAGNotExists(addomain3.Name));
					}
					else if (!adgroup3.Members.Contains(this.exs.Id))
					{
						adgroup3.Members.Add(this.exs.Id);
					}
					this.rootDomainRecipientSession.LinkResolutionServer = adgroup2.OriginatingServer;
					this.recipientSession.LinkResolutionServer = this.exs.OriginatingServer;
					try
					{
						if (base.ShouldProcess(Strings.InfoAddGroupToGroup(adgroup2.DistinguishedName, this.exs.DistinguishedName)))
						{
							SetupTaskBase.Save(this.exs, this.rootDomainRecipientSession);
							base.WriteVerbose(Strings.InfoAddedGroupToGroup(adgroup2.DistinguishedName, this.exs.DistinguishedName, this.exs.OriginatingServer, this.rootDomainRecipientSession.LinkResolutionServer));
						}
						if (adgroup3 != null && base.ShouldProcess(Strings.InfoAddGroupToGroup(this.exs.DistinguishedName, adgroup3.DistinguishedName)))
						{
							SetupTaskBase.Save(adgroup3, this.recipientSession);
							base.WriteVerbose(Strings.InfoAddedGroupToGroup(this.exs.DistinguishedName, adgroup3.DistinguishedName, adgroup3.OriginatingServer, this.recipientSession.LinkResolutionServer));
						}
						if (mesoContainer3.ObjectVersion < MesoContainer.DomainPrepVersion)
						{
							mesoContainer3.ObjectVersion = MesoContainer.DomainPrepVersion;
							if (base.ShouldProcess(mesoContainer3.DistinguishedName, Strings.InfoSetObjectVersion(MesoContainer.DomainPrepVersion), null))
							{
								this.domainConfigurationSession.Save(mesoContainer3);
								base.LogWriteObject(mesoContainer3);
							}
						}
					}
					catch (ADOperationException ex)
					{
						DirectoryOperationException ex2 = ex.InnerException as DirectoryOperationException;
						if (ex2 != null && ex2.Response != null && ex2.Response.ResultCode == System.DirectoryServices.Protocols.ResultCode.UnwillingToPerform)
						{
							if (k == 0)
							{
								this.WriteWarning(Strings.ErrorCouldNotAddCrossDomainGroupMember(adgroup2.DistinguishedName, this.exs.DistinguishedName, this.rootDomainRecipientSession.LinkResolutionServer, this.exs.OriginatingServer, ex2.Response.ErrorMessage));
								list9.Add(addomain3);
							}
							else
							{
								this.WriteWarning(Strings.ErrorFailedToAddCrossDomainGroupMember(adgroup2.DistinguishedName, this.exs.DistinguishedName, this.rootDomainRecipientSession.LinkResolutionServer, this.exs.OriginatingServer, ex2.Response.ErrorMessage));
								this.WriteError(new UnableToAddE12DStoExSReplicationException(addomain3.Name, ex), ErrorCategory.ObjectNotFound, null, false);
							}
						}
						else
						{
							if (ex2 == null || ex2.Response == null || ex2.Response.ResultCode != System.DirectoryServices.Protocols.ResultCode.InsufficientAccessRights)
							{
								throw;
							}
							this.WriteError(new UnableToAddE12DStoExSPermissionsException(addomain3.Name, adgroup2.DistinguishedName, this.exs.DistinguishedName, this.exs.Name, this.rootDomainRecipientSession.LinkResolutionServer, this.exs.OriginatingServer, ex), ErrorCategory.PermissionDenied, null, false);
						}
					}
					finally
					{
						this.rootDomainRecipientSession.LinkResolutionServer = null;
						this.recipientSession.LinkResolutionServer = null;
					}
				}
				if (list9.Count <= 0)
				{
					break;
				}
				array6 = list9.ToArray();
				list9.Clear();
				Thread.Sleep(30000);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
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
			this.ewp = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EwpWkGuid);
			if (this.ewp == null)
			{
				base.ThrowTerminatingError(new ExWindowsPermissionsGroupNotFoundException(WellKnownGuid.EwpWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.ewp);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.AccountDomainUsersSid, this.ewp.Sid.AccountDomainSid);
			this.domainUsersId = this.ResolveRecipientSidToObjectId(securityIdentifier);
			if (this.domainUsersId == null)
			{
				base.ThrowTerminatingError(new DomainUsersGroupNotFoundException(securityIdentifier.ToString()), ErrorCategory.InvalidData, null);
			}
			this.adSplitPermissionMode = base.GetADSplitPermissionMode(this.ets, this.ewp);
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
			TaskLogger.LogExit();
		}

		protected override string TaskName
		{
			get
			{
				return "PrepareDomain";
			}
		}

		private void CreateRootTenantOU(ADDomain domain)
		{
			ADOrganizationalUnit adorganizationalUnit = new ADOrganizationalUnit();
			adorganizationalUnit.SetId(domain.Id.GetChildId("OU", InitializeDomainPermissions.MultiTenancyOrganizationRootContainerName));
			try
			{
				this.domainConfigurationSession.Save(adorganizationalUnit);
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
			if (base.ShouldProcess(adorganizationalUnit.Id.DistinguishedName, Strings.InfoProcessAction(this.ets.DisplayName), null))
			{
				ActiveDirectoryAccessRule activeDirectoryAccessRule = new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.DeleteTree, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Children);
				DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.domainConfigurationSession, adorganizationalUnit.Id, new ActiveDirectoryAccessRule[]
				{
					activeDirectoryAccessRule
				});
			}
		}

		private void CreateMonitoringMailboxContainer(MesoContainer meso)
		{
			ADContainer adcontainer = new ADContainer();
			adcontainer.SetId(meso.Id.GetChildId("Monitoring Mailboxes"));
			try
			{
				this.domainConfigurationSession.Save(adcontainer);
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
		}

		private void CreateMsoSyncContainer(ADDomain domain)
		{
			ADContainer adcontainer = new ADContainer();
			adcontainer.SetId(domain.Id.GetChildId(SyncServiceInstance.MSOSyncRootContainer));
			try
			{
				this.domainConfigurationSession.Save(adcontainer);
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
			Guid schemaClassGuid = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, SyncServiceInstance.MostDerivedClass);
			Guid schemaClassGuid2 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, FailedMSOSyncObject.MostDerivedClass);
			Guid schemaClassGuid3 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, ADContainer.MostDerivedClass);
			Guid schemaClassGuid4 = DirectoryCommon.GetSchemaClassGuid(this.configurationSession, ForwardSyncCookie.MostDerivedClass);
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
			list.Add(new ActiveDirectoryAccessRule(this.exs.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.exs.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid3, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.exs.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.DeleteTree | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, schemaClassGuid3, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid2, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.exs.Sid, ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid4, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, schemaClassGuid4, ActiveDirectorySecurityInheritance.All));
			DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, this.domainConfigurationSession, adcontainer.Id, list.ToArray());
		}

		private MesoContainer CreateOrFindMesoContainer(ADDomain dom)
		{
			MesoContainer mesoContainer = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindMesoContainer(dom);
			if (mesoContainer != null)
			{
				base.LogReadObject(mesoContainer);
			}
			else
			{
				mesoContainer = new MesoContainer();
				mesoContainer.SetId(dom.Id.GetChildId("Microsoft Exchange System Objects"));
				if (base.ShouldProcess(Strings.InfoCreateMESOContainer(mesoContainer.Id.DistinguishedName)))
				{
					this.domainConfigurationSession.Save(mesoContainer);
					base.LogWriteObject(mesoContainer);
					mesoContainer = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindMesoContainer(dom);
					base.LogReadObject(mesoContainer);
				}
			}
			return mesoContainer;
		}

		private ADPasswordSettings CreateOrFindExchangeDatacenterPSO(ADDomain dom)
		{
			ADContainer adcontainer = this.domainConfigurationSession.ResolveWellKnownGuid<ADContainer>(WellKnownGuid.SystemWkGuid, dom.DistinguishedName);
			if (adcontainer == null)
			{
				throw new SystemContainerNotFoundException(dom.Fqdn, WellKnownGuid.SystemWkGuid);
			}
			ADObjectId descendantId = adcontainer.Id.GetDescendantId("Password Settings Container", "Microsoft Exchange Datacenter Password Settings", new string[0]);
			ADPasswordSettings adpasswordSettings = this.domainConfigurationSession.Read<ADPasswordSettings>(descendantId);
			if (adpasswordSettings != null)
			{
				base.LogReadObject(adpasswordSettings);
			}
			else
			{
				adpasswordSettings = new ADPasswordSettings();
				adpasswordSettings.SetId(descendantId);
				adpasswordSettings.AppliesTo.Add(this.domainUsersId);
				if (base.ShouldProcess(Strings.InfoCreatePasswordSettings(adpasswordSettings.Id.DistinguishedName)))
				{
					this.domainConfigurationSession.Save(adpasswordSettings);
					base.LogWriteObject(adpasswordSettings);
					adpasswordSettings = this.domainConfigurationSession.Read<ADPasswordSettings>(descendantId);
					base.LogReadObject(adpasswordSettings);
				}
			}
			if (!adpasswordSettings.AppliesTo.Contains(this.domainUsersId) && base.ShouldProcess(Strings.InfoCreatePasswordSettings(adpasswordSettings.Id.DistinguishedName)))
			{
				adpasswordSettings.AppliesTo.Add(this.domainUsersId);
				this.domainConfigurationSession.Save(adpasswordSettings);
				base.LogWriteObject(adpasswordSettings);
			}
			return adpasswordSettings;
		}

		private void AddSaclRight(ADDomain dom, SecurityIdentifier exsSid)
		{
			string originatingServer = dom.OriginatingServer;
			if (string.IsNullOrEmpty(originatingServer))
			{
				base.WriteError(new DomainControllerNotSpecifiedException(), ErrorCategory.NotSpecified, null);
			}
			LsaNativeMethods.SafeLsaUnicodeString safeLsaUnicodeString = new LsaNativeMethods.SafeLsaUnicodeString(originatingServer);
			SafeLsaPolicyHandle safeLsaPolicyHandle;
			using (safeLsaUnicodeString)
			{
				LsaNativeMethods.LsaObjectAttributes objectAttributes = new LsaNativeMethods.LsaObjectAttributes();
				int num = LsaNativeMethods.LsaOpenPolicy(safeLsaUnicodeString, objectAttributes, LsaNativeMethods.PolicyAccess.AllAccess, out safeLsaPolicyHandle);
				if (num != 0)
				{
					int num2 = LsaNativeMethods.LsaNtStatusToWinError(num);
					base.WriteError(new OpenPolicyFailedException((uint)num2, originatingServer, dom.Name), ErrorCategory.ReadError, null);
				}
			}
			using (safeLsaPolicyHandle)
			{
				int num3 = 0;
				byte[] array = new byte[exsSid.BinaryLength];
				exsSid.GetBinaryForm(array, 0);
				SafeLsaMemoryHandle safeLsaMemoryHandle;
				int num = LsaNativeMethods.LsaEnumerateAccountRights(safeLsaPolicyHandle, array, out safeLsaMemoryHandle, out num3);
				if (num != 0)
				{
					int num2 = LsaNativeMethods.LsaNtStatusToWinError(num);
					if (((long)num2 & (long)((ulong)-1)) != 2L && num2 != 0)
					{
						base.WriteError(new EnumerateRightsFailedException(exsSid.ToString(), (uint)num2), ErrorCategory.ReadError, null);
					}
				}
				bool flag = false;
				using (safeLsaMemoryHandle)
				{
					long num4 = safeLsaMemoryHandle.DangerousGetHandle().ToInt64();
					for (int i = 0; i < num3; i++)
					{
						LsaNativeMethods.LsaUnicodeString lsaUnicodeString = (LsaNativeMethods.LsaUnicodeString)Marshal.PtrToStructure(new IntPtr(num4), typeof(LsaNativeMethods.LsaUnicodeString));
						if (lsaUnicodeString.Value == "SeSecurityPrivilege")
						{
							flag = true;
							break;
						}
						num4 += (long)Marshal.SizeOf(lsaUnicodeString);
					}
				}
				if (flag)
				{
					base.WriteVerbose(Strings.InfoPrivilegeAlreadyPresent(exsSid.ToString(), "SeSecurityPrivilege"));
				}
				else if (base.ShouldProcess(exsSid.ToString(), Strings.InfoProcessAddRight("SeSecurityPrivilege", dom.Name), null))
				{
					LsaNativeMethods.SafeLsaUnicodeString safeLsaUnicodeString3 = new LsaNativeMethods.SafeLsaUnicodeString("SeSecurityPrivilege");
					using (safeLsaUnicodeString3)
					{
						LsaNativeMethods.LsaUnicodeStringStruct[] userRights = new LsaNativeMethods.LsaUnicodeStringStruct[]
						{
							new LsaNativeMethods.LsaUnicodeStringStruct(safeLsaUnicodeString3)
						};
						num = LsaNativeMethods.LsaAddAccountRights(safeLsaPolicyHandle, array, userRights, 1);
						if (num != 0)
						{
							int num2 = LsaNativeMethods.LsaNtStatusToWinError(num);
							base.WriteError(new AddAccountRightsFailedException(exsSid.ToString(), (uint)num2), ErrorCategory.WriteError, null);
						}
					}
				}
			}
		}

		private void SetCreateDeletePermissionOnObjectClass(List<ActiveDirectoryAccessRule> acesToRemove, List<ActiveDirectoryAccessRule> acesToRemoveFromAdminSDHolder, List<ActiveDirectoryAccessRule> acesForDomainOnly, Guid objectClass)
		{
			acesToRemove.Add(new ActiveDirectoryAccessRule(this.ets.Sid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, objectClass, ActiveDirectorySecurityInheritance.All));
			acesToRemoveFromAdminSDHolder.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, objectClass, ActiveDirectorySecurityInheritance.All));
			(this.adSplitPermissionMode ? acesToRemove : acesForDomainOnly).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.CreateChild, AccessControlType.Allow, objectClass, ActiveDirectorySecurityInheritance.All));
			acesToRemove.Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.DeleteChild, AccessControlType.Allow, objectClass, ActiveDirectorySecurityInheritance.All));
			(this.adSplitPermissionMode ? acesToRemove : acesForDomainOnly).Add(new ActiveDirectoryAccessRule(this.ewp.Sid, ActiveDirectoryRights.Delete, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Descendents, objectClass));
		}

		private ADObjectId ResolveRecipientSidToObjectId(SecurityIdentifier sid)
		{
			ADRawEntry adrawEntry = null;
			IEnumerable<PropertyDefinition> properties = new PropertyDefinition[]
			{
				ADObjectSchema.Id
			};
			try
			{
				adrawEntry = this.rootDomainRecipientSession.FindADRawEntryBySid(sid, properties);
			}
			catch (ADReferralException)
			{
			}
			if (adrawEntry == null)
			{
				bool useGlobalCatalog = this.recipientSession.UseGlobalCatalog;
				this.recipientSession.UseGlobalCatalog = true;
				try
				{
					adrawEntry = this.rootDomainRecipientSession.FindADRawEntryBySid(sid, properties);
				}
				finally
				{
					this.recipientSession.UseGlobalCatalog = useGlobalCatalog;
				}
			}
			if (adrawEntry == null)
			{
				return null;
			}
			base.LogReadObject(adrawEntry);
			return (ADObjectId)adrawEntry[ADObjectSchema.Id];
		}

		private const string ExchangeSetupServersDescription = "This group is used during Exchange setup and is not intended to be used for other purposes.";

		private const string paramCreateMsoSyncRoot = "CreateMsoSyncRoot";

		private const string paramIsManagementForest = "IsManagementForest";

		public static readonly string MultiTenancyOrganizationRootContainerName = "Microsoft Exchange Hosted Organizations";

		private ADGroup exs;

		private ADGroup eoa;

		private ADGroup epa;

		private ADGroup ets;

		private ADGroup ewp;

		private ADGroup delegatedSetupRG;

		private ADObjectId domainUsersId;

		private bool adSplitPermissionMode;
	}
}
