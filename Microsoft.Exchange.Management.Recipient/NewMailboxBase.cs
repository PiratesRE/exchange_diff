using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning.LoadBalancing;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class NewMailboxBase : NewUserBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Shared" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxShared(base.Name.ToString(), this.Shared.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("Room" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxResource(base.Name.ToString(), ExchangeResourceType.Room.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("Equipment" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxResource(base.Name.ToString(), ExchangeResourceType.Equipment.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("Linked" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxLinked(base.Name.ToString(), this.LinkedMasterAccount.ToString(), this.LinkedDomainController.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("LinkedWithSyncMailbox" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxLinkedWithSyncMailbox(base.Name.ToString(), this.DataObject.MasterAccountSid.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("WindowsLiveID" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxWithWindowsLiveId(base.Name.ToString(), base.WindowsLiveID.SmtpAddress.ToString(), base.RecipientContainerId.ToString());
				}
				if ("Arbitration" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxArbitration(base.Name.ToString(), this.database.ToString(), this.Arbitration.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("PublicFolder" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxPublicFolder(base.Name.ToString(), this.database.ToString(), this.PublicFolder.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("Discovery" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxDiscovery(base.Name.ToString(), this.database.ToString(), this.Discovery.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if ("AuditLog" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxAuditLog(base.Name.ToString(), this.database.ToString(), this.AuditLog.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
				}
				if (this.Archive.IsPresent)
				{
					return Strings.ConfirmationMessageNewMailboxWithArchive(base.Name.ToString());
				}
				if ("RemoteArchive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailboxWithRemoteArchive(base.Name.ToString(), this.ArchiveDomain.ToString());
				}
				return Strings.ConfirmationMessageNewMailboxUser(base.Name.ToString(), this.DataObject.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		protected virtual bool runUMSteps
		{
			get
			{
				return false;
			}
		}

		private ActiveManager ActiveManager
		{
			get
			{
				return RecipientTaskHelper.GetActiveManagerInstance();
			}
		}

		private bool IsCreatingResourceOrSharedMB
		{
			get
			{
				return "Room" == base.ParameterSetName || "LinkedRoomMailbox" == base.ParameterSetName || "EnableRoomMailboxAccount" == base.ParameterSetName || "Equipment" == base.ParameterSetName || base.ParameterSetName == "TeamMailboxIW" || base.ParameterSetName == "TeamMailboxITPro" || base.ParameterSetName == "GroupMailbox" || base.ParameterSetName == "Shared";
			}
		}

		private bool IsCreatingLogonDisabledTypeMB
		{
			get
			{
				return "Linked" == base.ParameterSetName || this.IsCreatingResourceOrSharedMB || "DisabledUser" == base.ParameterSetName || "LinkedWithSyncMailbox" == base.ParameterSetName || "Discovery" == base.ParameterSetName || "PublicFolder" == base.ParameterSetName || "AuxMailbox" == base.ParameterSetName;
			}
		}

		protected virtual bool ShouldGenerateWindowsLiveID
		{
			get
			{
				return this.IsCreatingResourceOrSharedMB && base.WindowsLiveID == null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled && !base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Discovery")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = true, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "AuxMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = true, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		public override SecureString Password
		{
			get
			{
				return base.Password;
			}
			set
			{
				base.Password = value;
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnableRoomMailboxAccount")]
		public SecureString RoomMailboxPassword
		{
			get
			{
				return base.Password;
			}
			set
			{
				base.Password = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)base.Fields[ADRecipientSchema.MailboxProvisioningConstraint];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningConstraint] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)base.Fields[ADRecipientSchema.MailboxProvisioningPreferences];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningPreferences] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = true, ParameterSetName = "AuditLog")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = true, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "Discovery")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "EnableRoomMailboxAccount")]
		[Parameter(Mandatory = false, ParameterSetName = "AuxMailbox")]
		public override string UserPrincipalName
		{
			get
			{
				return base.UserPrincipalName;
			}
			set
			{
				base.UserPrincipalName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields[ADMailboxRecipientSchema.Database];
			}
			set
			{
				base.Fields[ADMailboxRecipientSchema.Database] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Room")]
		[Parameter(Mandatory = true, ParameterSetName = "EnableRoomMailboxAccount")]
		public SwitchParameter Room
		{
			get
			{
				return (SwitchParameter)base.Fields["Room"];
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		public SwitchParameter LinkedRoom
		{
			get
			{
				return (SwitchParameter)base.Fields["Room"];
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnableRoomMailboxAccount")]
		public bool EnableRoomMailboxAccount
		{
			get
			{
				return base.Fields["EnableRoomMailboxAccount"] != null && (bool)base.Fields["EnableRoomMailboxAccount"];
			}
			set
			{
				base.Fields["EnableRoomMailboxAccount"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Equipment")]
		public SwitchParameter Equipment
		{
			get
			{
				return (SwitchParameter)base.Fields["Equipment"];
			}
			set
			{
				base.Fields["Equipment"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Shared")]
		public SwitchParameter Shared
		{
			get
			{
				return (SwitchParameter)base.Fields["Shared"];
			}
			set
			{
				base.Fields["Shared"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? false);
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Discovery")]
		public SwitchParameter Discovery
		{
			get
			{
				return (SwitchParameter)(base.Fields["Discovery"] ?? false);
			}
			set
			{
				base.Fields["Discovery"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DisabledUser")]
		public SwitchParameter AccountDisabled
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisabledUser"] ?? false);
			}
			set
			{
				base.Fields["DisabledUser"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields[MailboxSchema.LinkedMasterAccount];
			}
			set
			{
				base.Fields[MailboxSchema.LinkedMasterAccount] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields["LinkedDomainController"];
			}
			set
			{
				base.Fields["LinkedDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields["LinkedCredential"];
			}
			set
			{
				base.Fields["LinkedCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter RetentionPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[MailboxSchema.RetentionPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RetentionPolicy] = value;
			}
		}

		[Parameter]
		public MailboxPolicyIdParameter ActiveSyncMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[ADUserSchema.ActiveSyncMailboxPolicy];
			}
			set
			{
				base.Fields[ADUserSchema.ActiveSyncMailboxPolicy] = value;
			}
		}

		[Parameter]
		public AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return (AddressBookMailboxPolicyIdParameter)base.Fields[ADRecipientSchema.AddressBookPolicy];
			}
			set
			{
				base.Fields[ADRecipientSchema.AddressBookPolicy] = value;
			}
		}

		[Parameter]
		public ThrottlingPolicyIdParameter ThrottlingPolicy
		{
			get
			{
				return (ThrottlingPolicyIdParameter)base.Fields[MailboxSchema.ThrottlingPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.ThrottlingPolicy] = value;
			}
		}

		[Parameter]
		public SharingPolicyIdParameter SharingPolicy
		{
			get
			{
				return (SharingPolicyIdParameter)base.Fields[MailboxSchema.SharingPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.SharingPolicy] = value;
			}
		}

		[Parameter]
		public RemoteAccountPolicyIdParameter RemoteAccountPolicy
		{
			get
			{
				return (RemoteAccountPolicyIdParameter)base.Fields[MailboxSchema.RemoteAccountPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RemoteAccountPolicy] = value;
			}
		}

		[Parameter]
		public bool RemotePowerShellEnabled
		{
			get
			{
				return (bool)base.Fields[MailboxSchema.RemotePowerShellEnabled];
			}
			set
			{
				base.Fields[MailboxSchema.RemotePowerShellEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter RoleAssignmentPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[MailboxSchema.RoleAssignmentPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RoleAssignmentPolicy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		public override MailboxIdParameter ArbitrationMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields[ADRecipientSchema.ArbitrationMailbox];
			}
			set
			{
				base.Fields[ADRecipientSchema.ArbitrationMailbox] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		public override MultiValuedProperty<ModeratorIDParameter> ModeratedBy
		{
			get
			{
				return (MultiValuedProperty<ModeratorIDParameter>)base.Fields[ADRecipientSchema.ModeratedBy];
			}
			set
			{
				base.Fields[ADRecipientSchema.ModeratedBy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		public override bool ModerationEnabled
		{
			get
			{
				return this.DataObject.ModerationEnabled;
			}
			set
			{
				this.DataObject.ModerationEnabled = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "RemovedMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedWithSyncMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		[Parameter(Mandatory = false, ParameterSetName = "MailboxPlan")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "GroupMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		public override TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return this.DataObject.SendModerationNotifications;
			}
			set
			{
				this.DataObject.SendModerationNotifications = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool QueryBaseDNRestrictionEnabled
		{
			get
			{
				return this.DataObject.QueryBaseDNRestrictionEnabled;
			}
			set
			{
				this.DataObject.QueryBaseDNRestrictionEnabled = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "User", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesFederatedUser", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID", ValueFromPipeline = true)]
		[Parameter(Mandatory = true, ParameterSetName = "RemovedMailbox", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser", ValueFromPipeline = true)]
		public RemovedMailboxIdParameter RemovedMailbox
		{
			get
			{
				return (RemovedMailboxIdParameter)base.Fields["RemovedMailbox"];
			}
			set
			{
				base.Fields["RemovedMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AuxMailbox")]
		public SwitchParameter AuxMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuxMailbox"] ?? false);
			}
			set
			{
				base.Fields["AuxMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter ArchiveDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields[ADUserSchema.ArchiveDatabase];
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveDatabase] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		public SwitchParameter RemoteArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoteArchive"] ?? false);
			}
			set
			{
				base.Fields["RemoteArchive"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoteArchive")]
		public SmtpDomain ArchiveDomain
		{
			get
			{
				return base.Fields[ADUserSchema.ArchiveDomain] as SmtpDomain;
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveDomain] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? false);
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		public bool IsExcludedFromServingHierarchy
		{
			get
			{
				return (bool)base.Fields[ADRecipientSchema.IsExcludedFromServingHierarchy];
			}
			set
			{
				base.Fields[ADRecipientSchema.IsExcludedFromServingHierarchy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		public SwitchParameter HoldForMigration
		{
			get
			{
				return (SwitchParameter)(base.Fields["HoldForMigration"] ?? false);
			}
			set
			{
				base.Fields["HoldForMigration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid MailboxContainerGuid
		{
			get
			{
				if (this.DataObject.MailboxContainerGuid == null)
				{
					return Guid.Empty;
				}
				return this.DataObject.MailboxContainerGuid.Value;
			}
			set
			{
				this.DataObject.MailboxContainerGuid = new Guid?(value);
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] ?? false);
			}
			set
			{
				base.Fields["ForestWideDomainControllerAffinityByExecutingUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? false);
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AuditLog")]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		internal static bool IsNonApprovalArbitrationMailboxName(string mailboxName)
		{
			return "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}".Equals(mailboxName, StringComparison.OrdinalIgnoreCase) || "MigrationMailbox{24B27736-B069-46f1-B482-D6D9EAC9B053}".Equals(mailboxName, StringComparison.OrdinalIgnoreCase) || "Migration.8f3e7716-2011-43e4-96b1-aba62d229136".Equals(mailboxName, StringComparison.OrdinalIgnoreCase) || "FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042".Equals(mailboxName, StringComparison.OrdinalIgnoreCase);
		}

		protected override void StampDefaultValues(ADUser dataObject)
		{
			base.StampDefaultValues(dataObject);
			dataObject.StampDefaultValues(RecipientType.UserMailbox);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugStartInternalBeginProcessing);
			}
			base.InternalBeginProcessing();
			if ("PublicFolder" == base.ParameterSetName)
			{
				foreach (object obj in NewMailboxBase.InvalidPublicFolderParameters)
				{
					if (base.Fields.IsModified(obj))
					{
						string parameter = (obj is ADPropertyDefinition) ? ((ADPropertyDefinition)obj).Name : obj.ToString();
						base.WriteError(new TaskArgumentException(Strings.ErrorInvalidParameterForPublicFolderTasks(parameter, "PublicFolder")), ExchangeErrorCategory.Client, null);
					}
				}
			}
			if (base.CurrentOrganizationId != null && base.CurrentOrganizationId.ConfigurationUnit != null)
			{
				ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(this.ConfigurationSession, base.CurrentOrganizationId);
				if (exchangeConfigUnit != null && exchangeConfigUnit.UseServicePlanAsCounterInstanceName)
				{
					base.CurrentTaskContext.Items["TenantNameForMonitoring"] = exchangeConfigUnit.ServicePlan;
				}
			}
			if (!("Linked" == base.ParameterSetName))
			{
				if (!("LinkedRoomMailbox" == base.ParameterSetName))
				{
					goto IL_185;
				}
			}
			try
			{
				NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
				this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			}
			catch (PSArgumentException exception)
			{
				base.ThrowNonLocalizedTerminatingError(exception, ExchangeErrorCategory.Client, this.LinkedCredential);
			}
			IL_185:
			if (this.RetentionPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("RetentionPolicy")), ExchangeErrorCategory.Client, null);
				}
				RetentionPolicy retentionPolicy = (RetentionPolicy)base.GetDataObject<RetentionPolicy>(this.RetentionPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRetentionPolicyNotFound(this.RetentionPolicy.ToString())), new LocalizedString?(Strings.ErrorRetentionPolicyNotUnique(this.RetentionPolicy.ToString())), ExchangeErrorCategory.Client);
				this.retentionPolicyId = retentionPolicy.Id;
			}
			if (this.ActiveSyncMailboxPolicy != null)
			{
				MobileMailboxPolicy mobileMailboxPolicy = (MobileMailboxPolicy)base.GetDataObject<MobileMailboxPolicy>(this.ActiveSyncMailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotFound(this.ActiveSyncMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotUnique(this.ActiveSyncMailboxPolicy.ToString())), ExchangeErrorCategory.Client);
				this.mobileMailboxPolicyId = (ADObjectId)mobileMailboxPolicy.Identity;
			}
			if (this.AddressBookPolicy != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = (AddressBookMailboxPolicy)base.GetDataObject<AddressBookMailboxPolicy>(this.AddressBookPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotFound(this.AddressBookPolicy.ToString())), new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotUnique(this.AddressBookPolicy.ToString())), ExchangeErrorCategory.Client);
				this.addressbookMailboxPolicyId = (ADObjectId)addressBookMailboxPolicy.Identity;
			}
			if (this.ThrottlingPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorLinkOpOnDehydratedTenant("ThrottlingPolicy")), ExchangeErrorCategory.Context, this.DataObject.Identity);
				}
				ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)base.GetDataObject<ThrottlingPolicy>(this.ThrottlingPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorThrottlingPolicyNotFound(this.ThrottlingPolicy.ToString())), new LocalizedString?(Strings.ErrorThrottlingPolicyNotUnique(this.ThrottlingPolicy.ToString())), ExchangeErrorCategory.Client);
				this.throttlingPolicyId = (ADObjectId)throttlingPolicy.Identity;
			}
			if (this.SharingPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("SharingPolicy")), ExchangeErrorCategory.Client, null);
				}
				SharingPolicy sharingPolicy = (SharingPolicy)base.GetDataObject<SharingPolicy>(this.SharingPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorSharingPolicyNotFound(this.SharingPolicy.ToString())), new LocalizedString?(Strings.ErrorSharingPolicyNotUnique(this.SharingPolicy.ToString())), ExchangeErrorCategory.Client);
				this.sharingPolicyId = (ADObjectId)sharingPolicy.Identity;
			}
			if (this.RemoteAccountPolicy != null)
			{
				RemoteAccountPolicy remoteAccountPolicy = (RemoteAccountPolicy)base.GetDataObject<RemoteAccountPolicy>(this.RemoteAccountPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRemoteAccountPolicyNotFound(this.RemoteAccountPolicy.ToString())), new LocalizedString?(Strings.ErrorRemoteAccountPolicyNotUnique(this.RemoteAccountPolicy.ToString())), ExchangeErrorCategory.Client);
				this.remoteAccountPolicyId = (ADObjectId)remoteAccountPolicy.Identity;
			}
			IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
			if (this.PublicFolder)
			{
				MailboxTaskHelper.ValidatePublicFolderInformationWritable(tenantLocalConfigSession, this.HoldForMigration, new Task.ErrorLoggerDelegate(base.WriteError), this.Force);
			}
			if (this.RoleAssignmentPolicy == null)
			{
				if (!this.Arbitration && !this.Discovery && !this.PublicFolder && !this.AuditLog)
				{
					ADObjectId adobjectId = base.ProvisioningCache.TryAddAndGetOrganizationData<ADObjectId>(CannedProvisioningCacheKeys.DefaultRoleAssignmentPolicyId, base.CurrentOrganizationId, delegate()
					{
						RoleAssignmentPolicy roleAssignmentPolicy2 = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(tenantLocalConfigSession, new Task.ErrorLoggerDelegate(this.WriteError), Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
						if (roleAssignmentPolicy2 != null)
						{
							return roleAssignmentPolicy2.Id;
						}
						return null;
					});
					if (adobjectId != null)
					{
						this.defaultRoleAssignmentPolicyId = adobjectId;
					}
				}
			}
			else
			{
				RoleAssignmentPolicy roleAssignmentPolicy = (RoleAssignmentPolicy)base.GetDataObject<RoleAssignmentPolicy>(this.RoleAssignmentPolicy, tenantLocalConfigSession, null, new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotFound(this.RoleAssignmentPolicy.ToString())), new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotUnique(this.RoleAssignmentPolicy.ToString())), ExchangeErrorCategory.Client);
				this.userSpecifiedRoleAssignmentPolicyId = (ADObjectId)roleAssignmentPolicy.Identity;
			}
			if (base.BypassLiveId && this.RemovedMailbox != null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorBypassWLIDAndRemovedMailboxTogether), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			if (this.RemovedMailbox != null)
			{
				this.removedMailbox = MailboxTaskHelper.GetRemovedMailbox(base.DomainController, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, this.RemovedMailbox, new Task.ErrorLoggerDelegate(base.WriteError));
				if (this.removedMailbox.PreviousDatabase == null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorRemovedMailboxDoesNotHaveDatabase(this.removedMailbox.Name)), ExchangeErrorCategory.Client, null);
				}
				DatabaseIdParameter obj = new DatabaseIdParameter(this.removedMailbox.PreviousDatabase);
				if (this.Database != null && !this.Database.Equals(obj))
				{
					base.WriteError(new UserInputInvalidException(Strings.ErrorRemovedMailboxCannotBeUsedWithDatabaseParameter(this.Database.ToString())), ExchangeErrorCategory.Client, null);
				}
				this.Database = obj;
				if (base.WindowsLiveID == null)
				{
					if (this.removedMailbox.WindowsLiveID.IsValidAddress)
					{
						base.WindowsLiveID = new WindowsLiveId(this.removedMailbox.WindowsLiveID.ToString());
					}
					else if (string.IsNullOrEmpty(this.UserPrincipalName))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorParameterRequired("UserPrincipalName")), ExchangeErrorCategory.Client, null);
					}
				}
				if (string.IsNullOrEmpty(base.SamAccountName))
				{
					base.SamAccountName = this.removedMailbox.SamAccountName;
				}
			}
			if (this.isDatabaseRequired)
			{
				if (this.Database != null)
				{
					bool throwOnError = this.RemovedMailbox == null;
					this.ValidateAndSetDatabase(this.Database, throwOnError, ExchangeErrorCategory.Client);
					this.isDatabaseRequired = (null == this.database);
				}
				else if (!base.IsProvisioningLayerAvailable)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorAutomaticProvisioningFailedToFindDatabase("Database")), ExchangeErrorCategory.ServerOperation, null);
				}
			}
			if (this.ArchiveDatabase != null)
			{
				this.ValidateAndSetArchiveDatabase(this.ArchiveDatabase, true, ExchangeErrorCategory.Client);
			}
			if (this.removedMailbox != null)
			{
				this.databaseOwnerServer = this.database.GetServer();
				this.mapiAdministrationSession = new MapiAdministrationSession(this.databaseOwnerServer.ExchangeLegacyDN, Fqdn.Parse(this.databaseOwnerServer.Fqdn));
			}
			bool flag = (this.IsCreatingResourceOrSharedMB || base.ParameterSetName == "PublicFolder") && base.WindowsLiveID == null;
			if (flag && string.IsNullOrEmpty(base.Alias))
			{
				base.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, base.CurrentOrganizationId, string.IsNullOrEmpty(base.Name) ? base.SamAccountName : base.Name, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (string.IsNullOrEmpty(this.UserPrincipalName) && this.IsCreatingLogonDisabledTypeMB)
			{
				string text = flag ? base.Alias : base.Name;
				if (!WindowsLiveIDLocalPartConstraint.IsValidLocalPartOfWindowsLiveID(text))
				{
					CmdletLogger.SafeAppendGenericInfo(base.CurrentTaskContext.UniqueId, "GenerateDefaultUPN", text);
					text = "G" + Guid.NewGuid().ToString("N");
				}
				this.UserPrincipalName = RecipientTaskHelper.GenerateUniqueUserPrincipalName(base.TenantGlobalCatalogSession, text, this.ConfigurationSession.GetDefaultAcceptedDomain().DomainName.Domain, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (this.ShouldGenerateWindowsLiveID)
			{
				this.GenerateWindowsLiveID(base.Alias);
				if (this.Password == null)
				{
					this.Password = MailboxTaskUtilities.GetRandomPassword(base.Name, this.UserPrincipalName, 16);
					base.UserSpecifiedParameters["Password"] = this.Password;
				}
			}
			if (base.UserSpecifiedParameters["EnableRoomMailboxAccount"] != null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ValidateEnableRoomMailboxAccount.Enabled)
			{
				this.ValidateEnableRoomMailboxAccountParameter((bool)base.UserSpecifiedParameters["EnableRoomMailboxAccount"], (SecureString)base.UserSpecifiedParameters["RoomMailboxPassword"]);
			}
			base.InternalStateReset();
			if (base.SoftDeletedObject != null)
			{
				SmtpAddress windowsLiveID = this.DataObject.WindowsLiveID;
				NetID netID = this.DataObject.NetID;
				string name = this.DataObject.Name;
				string displayName = this.DataObject.DisplayName;
				ADObjectId mailboxPlan = this.DataObject.MailboxPlan;
				this.DataObject = SoftDeletedTaskHelper.GetSoftDeletedADUser(base.CurrentOrganizationId, (MailboxIdParameter)base.SoftDeletedObject, new Task.ErrorLoggerDelegate(base.WriteError));
				if (this.DataObject.WindowsLiveID != windowsLiveID)
				{
					this.DataObject.EmailAddressPolicyEnabled = false;
					this.DataObject.WindowsLiveID = windowsLiveID;
					this.DataObject.UserPrincipalName = windowsLiveID.ToString();
					this.DataObject.PrimarySmtpAddress = windowsLiveID;
				}
				if (this.DataObject.NetID != netID)
				{
					this.DataObject.NetID = netID;
				}
				if (!string.IsNullOrEmpty(name))
				{
					this.DataObject.Name = name;
				}
				this.DataObject.Name = SoftDeletedTaskHelper.GetUniqueNameForRecovery((IRecipientSession)base.DataSession, this.DataObject.Name, this.DataObject.Id);
				if (!string.IsNullOrEmpty(displayName))
				{
					this.DataObject.DisplayName = displayName;
				}
				if (!this.ValidateMailboxPlan(this.DataObject.MailboxPlan))
				{
					this.DataObject.MailboxPlan = mailboxPlan;
				}
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.PublicFolder)
			{
				if (!this.Force && !this.HoldForMigration && base.GlobalConfigSession.GetOrgContainer().DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty && MailboxTaskHelper.HasPublicFolderDatabases(new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<Server>), base.GlobalConfigSession))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorLegacyPublicFolderDatabaseExist), ExchangeErrorCategory.Client, null);
				}
				this.DisallowPublicFolderMailboxCreationDuringFinalization();
			}
			base.InternalValidate();
			if (this.database != null)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets", LoggerHelper.CmdletPerfMonitors))
				{
					MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.SessionSettings, this.database, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.archiveDatabase != null)
			{
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets", LoggerHelper.CmdletPerfMonitors))
				{
					MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.SessionSettings, this.archiveDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.removedMailbox != null)
			{
				if (!this.removedMailbox.StoreMailboxExists)
				{
					if (this.removedMailbox.ExchangeGuid == Guid.Empty)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorRemovedMailboxDoesNotHaveMailboxGuid(this.removedMailbox.Name)), ExchangeErrorCategory.Client, null);
					}
					else
					{
						using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "MailboxTaskHelper.GetDeletedStoreMailbox", LoggerHelper.CmdletPerfMonitors))
						{
							StoreMailboxIdParameter identity = new StoreMailboxIdParameter(this.removedMailbox.ExchangeGuid);
							using (MailboxTaskHelper.GetDeletedStoreMailbox(this.mapiAdministrationSession, identity, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(this.database), this.Database, new Task.ErrorLoggerDelegate(base.WriteError)))
							{
							}
						}
					}
				}
				if (((MailboxDatabase)this.database).Recovery)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxResidesInRDB(this.removedMailbox.Name)), ExchangeErrorCategory.Client, null);
				}
				if (!this.databaseOwnerServer.IsE14OrLater)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE14Server(this.Database.ToString())), ExchangeErrorCategory.Client, null);
				}
				if (this.archiveDatabase != null && !this.archiveDatabase.GetServer().IsE14Sp1OrLater)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE14Sp1Server(this.ArchiveDatabase.ToString())), ExchangeErrorCategory.Client, null);
				}
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "MailboxTaskHelper.ValidateMailboxIsDisconnected", LoggerHelper.CmdletPerfMonitors))
				{
					MailboxTaskHelper.ValidateMailboxIsDisconnected(RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, this.removedMailbox.Id), this.removedMailbox.ExchangeGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				}
				using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "ConnectMailbox.CheckLegacyDNNotInUse", LoggerHelper.CmdletPerfMonitors))
				{
					ConnectMailbox.CheckLegacyDNNotInUse(MailboxId.Parse(this.removedMailbox.ExchangeGuid.ToString()), this.removedMailbox.LegacyExchangeDN, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, this.removedMailbox.Id), new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			MailboxTaskHelper.ValidateRoomMailboxPasswordParameterCanOnlyBeUsedWithEnableRoomMailboxPassword(base.Fields.IsModified("RoomMailboxPassword"), base.Fields.IsModified("EnableRoomMailboxAccount"), new Task.ErrorLoggerDelegate(base.WriteError));
			if (base.ParameterSetName == "EnableRoomMailboxAccount")
			{
				this.ValidateEnableRoomMailboxAccountParameter(this.EnableRoomMailboxAccount, this.RoomMailboxPassword);
			}
			if (this.MailboxProvisioningPreferences != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(this.MailboxProvisioningPreferences, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			MailboxTaskHelper.EnsureUserSpecifiedDatabaseMatchesMailboxProvisioningConstraint(this.database, this.archiveDatabase, base.Fields, this.MailboxProvisioningConstraint, new Task.ErrorLoggerDelegate(base.WriteError), ADMailboxRecipientSchema.Database);
			TaskLogger.LogExit();
		}

		private void ValidateEnableRoomMailboxAccountParameter(bool enableRoomMailboxAccount, SecureString roomMailboxPassword)
		{
			if (enableRoomMailboxAccount && roomMailboxPassword == null)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorRoomPasswordMustBeSetWhenCreatingARoomADAccount), ExchangeErrorCategory.Client, null);
			}
			if (!enableRoomMailboxAccount && roomMailboxPassword != null)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorRoomMailboxPasswordCannotBeSetIfEnableRoomMailboxAccountIsFalse), ExchangeErrorCategory.Client, null);
			}
		}

		private void DisallowPublicFolderMailboxCreationDuringFinalization()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled && CommonUtils.IsPublicFolderMailboxesLockedForNewConnectionsFlagSet(base.CurrentOrganizationId))
			{
				base.WriteError(new RecipientTaskException(new LocalizedString(ServerStrings.PublicFolderMailboxesCannotBeCreatedDuringMigration)), ExchangeErrorCategory.Client, null);
			}
		}

		protected override void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareUserObject(user);
			if (base.SoftDeletedObject == null)
			{
				if (this.databaseLocationInfo != null)
				{
					user.Database = this.database.Id;
					user.ServerLegacyDN = this.databaseLocationInfo.ServerLegacyDN;
					if (!RecipientTaskHelper.IsE14OrLater(this.databaseLocationInfo.ServerVersion))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE14Server(this.database.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				if ("PublicFolder" == base.ParameterSetName)
				{
					user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.PublicFolderMailbox, false));
				}
				else if ("Arbitration" == base.ParameterSetName)
				{
					user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.ArbitrationMailbox, false));
				}
				else if ("AuditLog" == base.ParameterSetName)
				{
					user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.AuditLogMailbox, false));
				}
				else
				{
					user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.UserMailbox, false));
				}
				if (base.Fields.IsModified(ADRecipientSchema.ThrottlingPolicy))
				{
					user.ThrottlingPolicy = this.throttlingPolicyId;
				}
				if (base.Fields.IsModified(ADUserSchema.SharingPolicy))
				{
					user.SharingPolicy = this.sharingPolicyId;
				}
				if (base.Fields.IsModified(ADRecipientSchema.IsExcludedFromServingHierarchy))
				{
					user.IsExcludedFromServingHierarchy = this.IsExcludedFromServingHierarchy;
				}
				if (base.Fields.IsChanged(ADRecipientSchema.MailboxProvisioningConstraint))
				{
					user.MailboxProvisioningConstraint = this.MailboxProvisioningConstraint;
				}
				if (base.Fields.IsChanged(ADRecipientSchema.MailboxProvisioningPreferences))
				{
					user.MailboxProvisioningPreferences = this.MailboxProvisioningPreferences;
				}
				if (base.Fields.IsModified(ADUserSchema.RemoteAccountPolicy))
				{
					user.RemoteAccountPolicy = this.remoteAccountPolicyId;
				}
				if (base.Fields.IsModified(ADRecipientSchema.RemotePowerShellEnabled))
				{
					user.RemotePowerShellEnabled = this.RemotePowerShellEnabled;
				}
				else
				{
					user.RemotePowerShellEnabled = true;
				}
				if (!user.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					if (this.userSpecifiedRoleAssignmentPolicyId != null)
					{
						user.RoleAssignmentPolicy = this.userSpecifiedRoleAssignmentPolicyId;
					}
					else if (user.RoleAssignmentPolicy == null && this.defaultRoleAssignmentPolicyId != null)
					{
						user.RoleAssignmentPolicy = this.defaultRoleAssignmentPolicyId;
					}
				}
				user.ShouldUseDefaultRetentionPolicy = true;
				user.ElcMailboxFlags |= ElcMailboxFlags.ElcV2;
				if (base.Fields.IsModified(ADUserSchema.RetentionPolicy))
				{
					user.RetentionPolicy = this.retentionPolicyId;
				}
				if (base.Fields.IsModified(ADUserSchema.ActiveSyncMailboxPolicy))
				{
					user.ActiveSyncMailboxPolicy = this.mobileMailboxPolicyId;
				}
				if (base.Fields.IsModified(ADRecipientSchema.AddressBookPolicy))
				{
					user.AddressBookPolicy = this.addressbookMailboxPolicyId;
				}
				user.ExchangeUserAccountControl = UserAccountControlFlags.None;
				if ("LinkedRoomMailbox" == base.ParameterSetName)
				{
					user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Room);
					user.MasterAccountSid = this.linkedUserSid;
				}
				else if ("AuxMailbox" == base.ParameterSetName)
				{
					AuxMailboxTaskHelper.AuxMailboxStampDefaultValues(user);
				}
				else if ("Linked" == base.ParameterSetName)
				{
					user.MasterAccountSid = this.linkedUserSid;
				}
				else if ("Shared" == base.ParameterSetName || "GroupMailbox" == base.ParameterSetName || "TeamMailboxIW" == base.ParameterSetName || "TeamMailboxITPro" == base.ParameterSetName)
				{
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				}
				else if ("Room" == base.ParameterSetName || "EnableRoomMailboxAccount" == base.ParameterSetName)
				{
					user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Room);
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				}
				else if ("Equipment" == base.ParameterSetName)
				{
					user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Equipment);
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				}
				else if ("Arbitration" == base.ParameterSetName)
				{
					if (!user.IsModified(ADRecipientSchema.DisplayName))
					{
						user.DisplayName = Strings.ArbitrationMailboxDefaultDisplayName;
					}
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
					user.HiddenFromAddressListsEnabled = true;
					user.RequireAllSendersAreAuthenticated = true;
					user.ProhibitSendQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
					user.ProhibitSendReceiveQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
					user.IssueWarningQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
					user.UseDatabaseQuotaDefaults = new bool?(false);
					user.PersistedCapabilities.Add(Capability.OrganizationCapabilityPstProvider);
					ADObjectId childId;
					if (base.Organization == null)
					{
						childId = this.ConfigurationSession.GetOrgContainerId().GetChildId(ApprovalApplicationContainer.DefaultName);
					}
					else
					{
						childId = base.CurrentOrganizationId.ConfigurationUnit.GetChildId(ApprovalApplicationContainer.DefaultName);
					}
					if (this.ConfigurationSession.Read<ApprovalApplicationContainer>(childId) == null)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorRootContainerNotExist(childId.ToString())), ExchangeErrorCategory.Client, null);
					}
					if (!NewMailboxBase.IsNonApprovalArbitrationMailboxName(user.Name))
					{
						if (user.ManagedFolderMailboxPolicy == null && user.RetentionPolicy == null)
						{
							ADObjectId childId2;
							if (base.Organization == null)
							{
								childId2 = this.ConfigurationSession.GetOrgContainerId().GetChildId("Retention Policies Container").GetChildId("ArbitrationMailbox");
							}
							else
							{
								childId2 = base.CurrentOrganizationId.ConfigurationUnit.GetChildId("Retention Policies Container").GetChildId("ArbitrationMailbox");
							}
							RetentionPolicy retentionPolicy = this.ConfigurationSession.Read<RetentionPolicy>(childId2);
							if (retentionPolicy != null)
							{
								user.RetentionPolicy = retentionPolicy.Id;
							}
							else
							{
								this.WriteWarning(Strings.WarningArbitrationRetentionPolicyNotAvailable(childId2.ToString()));
							}
						}
						ApprovalApplication[] array = this.ConfigurationSession.Find<ApprovalApplication>(childId, QueryScope.SubTree, null, null, 0);
						List<ADObjectId> list = new List<ADObjectId>(array.Length);
						foreach (ApprovalApplication approvalApplication in array)
						{
							list.Add((ADObjectId)approvalApplication.Identity);
						}
						user[ADUserSchema.ApprovalApplications] = new MultiValuedProperty<ADObjectId>(list);
						if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SetDefaultProhibitSendReceiveQuota.Enabled)
						{
							user.ProhibitSendReceiveQuota = ByteQuantifiedSize.FromGB(10UL);
						}
					}
				}
				else if ("AuditLog" == base.ParameterSetName)
				{
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
					user.HiddenFromAddressListsEnabled = true;
					user.CalendarVersionStoreDisabled = true;
					user.UseDatabaseQuotaDefaults = new bool?(false);
					user.RecoverableItemsQuota = ByteQuantifiedSize.FromGB(50UL);
				}
				else if ("Discovery" == base.ParameterSetName)
				{
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
					user.ProhibitSendReceiveQuota = ByteQuantifiedSize.FromGB(50UL);
					user.ProhibitSendQuota = ByteQuantifiedSize.FromGB(50UL);
					user.CalendarVersionStoreDisabled = true;
					user.UseDatabaseQuotaDefaults = new bool?(false);
					user.HiddenFromAddressListsEnabled = true;
					user.MaxReceiveSize = ByteQuantifiedSize.FromMB(100UL);
					user.MaxSendSize = ByteQuantifiedSize.FromMB(100UL);
				}
				else if ("MailboxPlan" == base.ParameterSetName)
				{
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.NullSid, null);
				}
				else if ("PublicFolder" == base.ParameterSetName)
				{
					user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
					user.HiddenFromAddressListsEnabled = true;
					if (!RecipientTaskHelper.IsE15OrLater(this.databaseLocationInfo.ServerVersion))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(this.database.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				else if (base.ParameterSetName == "Monitoring")
				{
					user.HiddenFromAddressListsEnabled = true;
				}
				MailboxTaskHelper.StampMailboxRecipientTypes(user, base.ParameterSetName);
			}
			if (base.WindowsLiveID != null && base.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				user.EmailAddressPolicyEnabled = false;
				SmtpProxyAddress item = new SmtpProxyAddress(base.WindowsLiveID.SmtpAddress.ToString(), false);
				if (!user.EmailAddresses.Contains(item))
				{
					user.EmailAddresses.Add(item);
				}
			}
			if (base.ParameterSetName == "Monitoring")
			{
				user.EmailAddresses.Add(ProxyAddress.Parse("SIP:" + user.UserPrincipalName));
			}
			if ((this.Arbitration.IsPresent || "MailboxPlan" == base.ParameterSetName || "Discovery" == base.ParameterSetName) && string.IsNullOrEmpty(user.SamAccountName))
			{
				user.SamAccountName = "SM_" + Guid.NewGuid().ToString("N").Substring(0, 17);
			}
			if ((this.Archive.IsPresent || this.archiveDatabase != null) && "RemoteArchive" == base.ParameterSetName)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorArchiveRemoteArchiveCannotBeSpecifiedTogether), ExchangeErrorCategory.Client, null);
			}
			if (this.Archive.IsPresent || "RemoteArchive" == base.ParameterSetName)
			{
				if ("RemoteArchive" == base.ParameterSetName && this.databaseLocationInfo.ServerVersion < Server.E15MinVersion)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(this.database.ToString())), ExchangeErrorCategory.Client, null);
				}
				user.ArchiveGuid = Guid.NewGuid();
				user.ArchiveName = new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + (string.IsNullOrEmpty(user.DisplayName) ? user.Name : user.DisplayName));
				if ("RemoteArchive" == base.ParameterSetName)
				{
					user.ArchiveDomain = this.ArchiveDomain;
					user.RemoteRecipientType |= RemoteRecipientType.ProvisionArchive;
				}
				else
				{
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SetActiveArchiveStatus.Enabled)
					{
						user.ArchiveStatus |= ArchiveStatusFlags.Active;
					}
					if (this.archiveDatabase == null)
					{
						user.ArchiveDatabase = user.Database;
					}
					else
					{
						MailboxTaskHelper.BlockLowerMajorVersionArchive(this.archiveDatabaseLocationInfo.ServerVersion, user.Database.DistinguishedName, this.archiveDatabase.DistinguishedName, this.archiveDatabase.ToString(), user.Database, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<MailboxDatabase>), base.GlobalConfigSession, this.ActiveManager, new Task.ErrorLoggerDelegate(base.WriteError));
						user.ArchiveDatabase = this.archiveDatabase.Id;
					}
				}
				MailboxTaskHelper.ApplyDefaultArchivePolicy(user, this.ConfigurationSession);
			}
			if (MailboxTaskHelper.SupportsMailboxReleaseVersioning(user))
			{
				user.MailboxRelease = this.databaseLocationInfo.MailboxRelease;
				if (this.Archive.IsPresent)
				{
					user.ArchiveRelease = this.archiveDatabaseLocationInfo.MailboxRelease;
				}
			}
			if (base.SoftDeletedObject != null)
			{
				SoftDeletedTaskHelper.UpdateShadowWhenSoftDeletedProperty((IRecipientSession)base.DataSession, this.ConfigurationSession, base.CurrentOrganizationId, this.DataObject);
				this.DataObject.RecipientSoftDeletedStatus = 0;
				this.DataObject.WhenSoftDeleted = null;
				this.DataObject.InternalOnly = false;
			}
			NewMailboxBase.CopyRemovedMailboxData(user, this.removedMailbox);
			TaskLogger.LogExit();
		}

		protected override void StampChangesAfterSettingPassword()
		{
			if ("User" == base.ParameterSetName || "WindowsLiveID" == base.ParameterSetName || "WindowsLiveCustomDomains" == base.ParameterSetName || "ImportLiveId" == base.ParameterSetName || "FederatedUser" == base.ParameterSetName || "RemovedMailbox" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName || "MicrosoftOnlineServicesFederatedUser" == base.ParameterSetName || "MicrosoftOnlineServicesID" == base.ParameterSetName || ("EnableRoomMailboxAccount" == base.ParameterSetName && this.EnableRoomMailboxAccount))
			{
				this.DataObject.UserAccountControl = UserAccountControlFlags.NormalAccount;
				return;
			}
			if ("DisabledUser" == base.ParameterSetName || "PublicFolder" == base.ParameterSetName)
			{
				this.DataObject.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
				this.DataObject.ExchangeUserAccountControl |= UserAccountControlFlags.AccountDisabled;
				return;
			}
			if ("Monitoring" == base.ParameterSetName)
			{
				this.DataObject.UserAccountControl = (UserAccountControlFlags.NormalAccount | UserAccountControlFlags.DoNotExpirePassword);
				return;
			}
			this.DataObject.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
			this.DataObject.ExchangeUserAccountControl |= UserAccountControlFlags.AccountDisabled;
			if (!base.ResetPasswordOnNextLogon && (this.Password == null || this.Password.Length == 0) && "Arbitration" != base.ParameterSetName)
			{
				this.DataObject.UserAccountControl |= UserAccountControlFlags.DoNotExpirePassword;
			}
			MailboxTaskHelper.GrantPermissionToLinkedUserAccount(this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.VerboseSaveADSecurityDescriptor(this.DataObject.Id.ToString()));
			}
			this.DataObject.SaveSecurityDescriptor(((SecurityDescriptor)this.DataObject[ADObjectSchema.NTSecurityDescriptor]).ToRawSecurityDescriptor());
			if ("Discovery" == base.ParameterSetName || "MailboxPlan" == base.ParameterSetName)
			{
				this.DataObject.AcceptMessagesOnlyFrom.Add(this.DataObject.Id);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "NewMailboxBase.InternalProcessRecord", LoggerHelper.CmdletPerfMonitors))
			{
				if (base.WindowsLiveID != null && MailboxTaskHelper.IsReservedLiveId(base.WindowsLiveID.SmtpAddress))
				{
					return;
				}
				if (this.runUMSteps && this.DataObject.UMEnabled)
				{
					Utils.DoUMEnablingSynchronousWork(this.DataObject);
				}
				if (base.ParameterSetName == "EnableRoomMailboxAccount" && this.EnableRoomMailboxAccount)
				{
					this.Password = this.RoomMailboxPassword;
				}
				if (this.DataObject.IsInLitigationHoldOrInplaceHold)
				{
					RecoverableItemsQuotaHelper.IncreaseRecoverableItemsQuotaIfNeeded(this.DataObject);
				}
				base.InternalProcessRecord();
				bool flag = false;
				if (this.PublicFolder || this.DataObject.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox || this.DataObject.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || (this.DataObject.MailboxContainerGuid != null && this.DataObject.MailboxContainerGuid.Value != Guid.Empty))
				{
					IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, false, this.DataObject.OriginatingServer, null);
					Organization orgContainer = tenantLocalConfigSession.GetOrgContainer();
					if (this.PublicFolder)
					{
						if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty)
						{
							orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
							orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(this.DataObject.ExchangeGuid, this.HoldForMigration ? PublicFolderInformation.HierarchyType.InTransitMailboxGuid : PublicFolderInformation.HierarchyType.MailboxGuid);
							tenantLocalConfigSession.Save(orgContainer);
						}
						else
						{
							this.DataObject.IsHierarchyReady = false;
							base.DataSession.Save(this.DataObject);
						}
					}
					if (this.databaseLocationInfo == null)
					{
						this.databaseLocationInfo = MailboxTaskHelper.GetDatabaseLocationInfo(this.database, this.ActiveManager, new Task.ErrorLoggerDelegate(base.WriteError));
					}
					MailboxTaskHelper.PrepopulateCacheForMailbox(this.database, this.databaseLocationInfo.ServerFqdn, this.DataObject.OrganizationId, this.DataObject.LegacyExchangeDN, this.DataObject.ExchangeGuid, tenantLocalConfigSession.LastUsedDc, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
					this.lastUsedDc = tenantLocalConfigSession.LastUsedDc;
					flag = true;
				}
				if (this.removedMailbox != null)
				{
					ConnectMailbox.UpdateSDAndRefreshMailbox(this.mapiAdministrationSession, this.DataObject, (MailboxDatabase)this.database, this.removedMailbox.ExchangeGuid, this.removedMailbox.LegacyExchangeDN, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
				else if (base.SoftDeletedObject == null && this.databaseLocationInfo != null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.PrePopulateCacheForMailboxBasedOnDatabase.Enabled)
				{
					bool flag2 = PhysicalResourceLoadBalancing.IsDatabaseInLocalSite(this.databaseLocationInfo, delegate(string message)
					{
						base.WriteVerbose(new LocalizedString(message));
					});
					string text = null;
					if (flag2)
					{
						text = this.DataObject.OriginatingServer;
					}
					else if (!this.isMailboxForcedReplicationDisabled)
					{
						string[] array = ((IRecipientSession)base.DataSession).ReplicateSingleObject(this.DataObject, new ADObjectId[]
						{
							this.databaseLocationInfo.ServerSite
						});
						if (array == null || array.Length == 0 || string.IsNullOrEmpty(array[0]))
						{
							this.WriteWarning(Strings.ErrorFailedToReplicateMailbox(this.DataObject.Identity.ToString(), this.databaseLocationInfo.ServerSite.ToString()));
						}
						else
						{
							text = array[0];
							base.WriteVerbose(Strings.VerboseSucceededReplicatingObject(this.DataObject.Identity.ToString(), text));
						}
					}
					if (text != null && !flag && this.DataObject.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
					{
						using (new CmdletMonitoredScope(base.CurrentTaskContext.UniqueId, "BizLogic", "MailboxTaskHelper.PrepopulateCacheForMailbox", LoggerHelper.CmdletPerfMonitors))
						{
							MailboxTaskHelper.PrepopulateCacheForMailbox(this.database, this.databaseLocationInfo.ServerFqdn, base.CurrentOrganizationId, this.DataObject.LegacyExchangeDN, this.DataObject.ExchangeGuid, text, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
						}
					}
				}
				this.DisposeMapiSession();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStopProcessing()
		{
			base.InternalStopProcessing();
			this.DisposeMapiSession();
		}

		private static void CopyRemovedMailboxData(ADUser mailbox, RemovedMailbox removedMailbox)
		{
			if (removedMailbox != null && mailbox != null)
			{
				mailbox.ExchangeGuid = removedMailbox.ExchangeGuid;
				mailbox.LegacyExchangeDN = removedMailbox.LegacyExchangeDN;
				foreach (ProxyAddress proxyAddress in removedMailbox.EmailAddresses)
				{
					if (!proxyAddress.AddressString.Equals(removedMailbox.WindowsLiveID.ToString()) && !mailbox.EmailAddresses.Contains(proxyAddress))
					{
						mailbox.EmailAddresses.Add(proxyAddress);
					}
				}
			}
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(Mailbox).FullName;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.DisposeMapiSession();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Mailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void ValidateProvisionedProperties(IConfigurable dataObject)
		{
			if (this.isDatabaseRequired)
			{
				ADUser aduser = dataObject as ADUser;
				if (aduser != null && aduser.IsChanged(IADMailStorageSchema.Database))
				{
					if (aduser.DatabaseAndLocation == null)
					{
						this.ValidateAndSetDatabase(new DatabaseIdParameter(aduser.Database), false, ExchangeErrorCategory.ServerOperation);
						return;
					}
					MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = (MailboxDatabaseWithLocationInfo)aduser.DatabaseAndLocation;
					this.database = mailboxDatabaseWithLocationInfo.MailboxDatabase;
					this.databaseLocationInfo = mailboxDatabaseWithLocationInfo.DatabaseLocationInfo;
					aduser.DatabaseAndLocation = null;
					aduser.propertyBag.ResetChangeTracking(IADMailStorageSchema.DatabaseAndLocation);
					return;
				}
				else
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorParameterRequiredButNotProvisioned("Database")), ExchangeErrorCategory.ServerOperation, null);
				}
			}
		}

		private bool ValidateMailboxPlan(ADObjectId mailboxPlanId)
		{
			if (mailboxPlanId == null)
			{
				return false;
			}
			ADUser aduser = (ADUser)base.DataSession.Read<ADUser>(mailboxPlanId);
			return aduser != null && MailboxPlanRelease.NonCurrentRelease != (MailboxPlanRelease)aduser[ADRecipientSchema.MailboxPlanRelease];
		}

		private void ValidateAndSetDatabase(DatabaseIdParameter databaseId, bool throwOnError, ExchangeErrorCategory errorCategory)
		{
			this.InternalValidateAndSetArchiveDatabase(databaseId, Server.E15MinVersion, throwOnError, errorCategory, out this.database, out this.databaseLocationInfo);
		}

		private void ValidateAndSetArchiveDatabase(DatabaseIdParameter databaseId, bool throwOnError, ExchangeErrorCategory errorCategory)
		{
			this.InternalValidateAndSetArchiveDatabase(databaseId, Server.E15MinVersion, throwOnError, errorCategory, out this.archiveDatabase, out this.archiveDatabaseLocationInfo);
		}

		private void InternalValidateAndSetArchiveDatabase(DatabaseIdParameter databaseId, int minServerVersion, bool throwOnError, ExchangeErrorCategory errorCategory, out Database database, out DatabaseLocationInfo databaseLocationInfo)
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugStartSetDatabase);
			}
			database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseId, base.GlobalConfigSession, null, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseId.ToString())), errorCategory);
			databaseLocationInfo = MailboxTaskHelper.GetDatabaseLocationInfo(database, this.ActiveManager, new Task.ErrorLoggerDelegate(base.WriteError));
			LocalizedException ex = null;
			if (minServerVersion > databaseLocationInfo.ServerVersion)
			{
				ex = new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(database.ToString()));
			}
			else if (((MailboxDatabase)database).Recovery)
			{
				ex = new RecipientTaskException(Strings.ErrorRecoveryDatabase(database.Name));
			}
			if (ex != null)
			{
				if (throwOnError)
				{
					base.ThrowTerminatingError(ex, ExchangeErrorCategory.ServerOperation, null);
				}
				else
				{
					base.WriteError(ex, ExchangeErrorCategory.ServerOperation, null);
				}
			}
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugEndSetDatabase);
			}
		}

		private void GenerateWindowsLiveID(string preferedLocalPart)
		{
			string domainPartOfUserPrincalName = RecipientTaskHelper.GetDomainPartOfUserPrincalName(this.UserPrincipalName);
			WindowsLiveId windowsLiveID;
			if (!WindowsLiveIDLocalPartConstraint.IsValidLocalPartOfWindowsLiveID(preferedLocalPart) || !WindowsLiveId.TryParse(string.Format("{0}@{1}", preferedLocalPart, domainPartOfUserPrincalName), out windowsLiveID))
			{
				windowsLiveID = new WindowsLiveId(string.Format("{0}@{1}", "G" + Guid.NewGuid().ToString("N"), domainPartOfUserPrincalName));
			}
			base.WindowsLiveID = windowsLiveID;
			base.UserSpecifiedParameters["WindowsLiveID"] = base.WindowsLiveID;
		}

		private void DisposeMapiSession()
		{
			if (this.mapiAdministrationSession != null)
			{
				this.mapiAdministrationSession.Dispose();
				this.mapiAdministrationSession = null;
			}
		}

		private const string ForestWideDomainControllerAffinityByExecutingUserName = "ForestWideDomainControllerAffinityByExecutingUser";

		public const string TenantNameForMonitoringKey = "TenantNameForMonitoring";

		public const string Migration = "MigrationMailbox{24B27736-B069-46f1-B482-D6D9EAC9B053}";

		private const int RandomPasswordLengthForLiveID = 16;

		private static readonly object[] InvalidPublicFolderParameters = new object[]
		{
			"Archive",
			"AuxMailbox",
			ADUserSchema.ArchiveDatabase,
			ADUserSchema.PasswordLastSetRaw
		};

		private Database database;

		private Database archiveDatabase;

		private ADObjectId retentionPolicyId;

		private ADObjectId mobileMailboxPolicyId;

		private SecurityIdentifier linkedUserSid;

		private ADObjectId addressbookMailboxPolicyId;

		private ADObjectId throttlingPolicyId;

		private ADObjectId sharingPolicyId;

		private ADObjectId remoteAccountPolicyId;

		private ADObjectId userSpecifiedRoleAssignmentPolicyId;

		private ADObjectId defaultRoleAssignmentPolicyId;

		protected bool isDatabaseRequired = true;

		protected DatabaseLocationInfo databaseLocationInfo;

		private DatabaseLocationInfo archiveDatabaseLocationInfo;

		protected string lastUsedDc;

		private RemovedMailbox removedMailbox;

		private MapiAdministrationSession mapiAdministrationSession;

		private Server databaseOwnerServer;

		protected bool isMailboxForcedReplicationDisabled;
	}
}
