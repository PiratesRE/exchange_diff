using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Undo", "SyncSoftDeletedMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "SoftDeletedMailbox")]
	public sealed class UndoSyncSoftDeletedMailbox : NewMailboxOrSyncMailbox
	{
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "SoftDeletedMailbox", ValueFromPipeline = true)]
		public new MailboxIdParameter SoftDeletedObject
		{
			get
			{
				return (MailboxIdParameter)base.SoftDeletedObject;
			}
			set
			{
				base.SoftDeletedObject = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailbox")]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailbox")]
		public new WindowsLiveId WindowsLiveID
		{
			get
			{
				return base.WindowsLiveID;
			}
			set
			{
				base.WindowsLiveID = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailbox")]
		public override SecureString Password
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

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailbox")]
		public new SwitchParameter BypassLiveId
		{
			get
			{
				return base.BypassLiveId;
			}
			set
			{
				base.BypassLiveId = value;
			}
		}

		protected override bool AllowBypassLiveIdWithoutWlid
		{
			get
			{
				return true;
			}
		}

		private new SwitchParameter AccountDisabled
		{
			get
			{
				return base.AccountDisabled;
			}
		}

		private new MailboxPolicyIdParameter ActiveSyncMailboxPolicy
		{
			get
			{
				return base.ActiveSyncMailboxPolicy;
			}
		}

		private new AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return base.AddressBookPolicy;
			}
			set
			{
				base.AddressBookPolicy = value;
			}
		}

		private new string Alias
		{
			get
			{
				return base.Alias;
			}
		}

		private new SwitchParameter Arbitration
		{
			get
			{
				return base.Arbitration;
			}
		}

		private new MailboxIdParameter ArbitrationMailbox
		{
			get
			{
				return base.ArbitrationMailbox;
			}
		}

		private new SwitchParameter Archive
		{
			get
			{
				return base.Archive;
			}
		}

		private new DatabaseIdParameter ArchiveDatabase
		{
			get
			{
				return base.ArchiveDatabase;
			}
		}

		private new SmtpDomain ArchiveDomain
		{
			get
			{
				return base.ArchiveDomain;
			}
		}

		private new DatabaseIdParameter Database
		{
			get
			{
				return base.Database;
			}
		}

		private new SwitchParameter Discovery
		{
			get
			{
				return base.Discovery;
			}
		}

		private new SwitchParameter Equipment
		{
			get
			{
				return base.Equipment;
			}
		}

		private new SwitchParameter EvictLiveId
		{
			get
			{
				return base.EvictLiveId;
			}
		}

		private new string ExternalDirectoryObjectId
		{
			get
			{
				return base.ExternalDirectoryObjectId;
			}
		}

		private new string FederatedIdentity
		{
			get
			{
				return base.FederatedIdentity;
			}
		}

		private new string FirstName
		{
			get
			{
				return base.FirstName;
			}
		}

		private new SwitchParameter Force
		{
			get
			{
				return base.Force;
			}
		}

		private new SwitchParameter ForestWideDomainControllerAffinityByExecutingUser
		{
			get
			{
				return base.ForestWideDomainControllerAffinityByExecutingUser;
			}
		}

		private new SwitchParameter HoldForMigration
		{
			get
			{
				return base.HoldForMigration;
			}
			set
			{
				base.HoldForMigration = value;
			}
		}

		private new string ImmutableId
		{
			get
			{
				return base.ImmutableId;
			}
		}

		private new SwitchParameter ImportLiveId
		{
			get
			{
				return base.ImportLiveId;
			}
		}

		private new string Initials
		{
			get
			{
				return base.Initials;
			}
		}

		private new string LastName
		{
			get
			{
				return base.LastName;
			}
		}

		private new PSCredential LinkedCredential
		{
			get
			{
				return base.LinkedCredential;
			}
		}

		private new string LinkedDomainController
		{
			get
			{
				return base.LinkedDomainController;
			}
		}

		private new UserIdParameter LinkedMasterAccount
		{
			get
			{
				return base.LinkedMasterAccount;
			}
		}

		private new MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return base.MailboxPlan;
			}
		}

		private new Guid MailboxContainerGuid
		{
			get
			{
				return base.MailboxContainerGuid;
			}
		}

		private new WindowsLiveId MicrosoftOnlineServicesID
		{
			get
			{
				return base.MicrosoftOnlineServicesID;
			}
		}

		private new MultiValuedProperty<ModeratorIDParameter> ModeratedBy
		{
			get
			{
				return base.ModeratedBy;
			}
		}

		private new bool ModerationEnabled
		{
			get
			{
				return base.ModerationEnabled;
			}
		}

		private new NetID NetID
		{
			get
			{
				return base.NetID;
			}
		}

		private new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
		}

		private new SwitchParameter OverrideRecipientQuotas
		{
			get
			{
				return base.OverrideRecipientQuotas;
			}
		}

		private new SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return base.PrimarySmtpAddress;
			}
		}

		private new SwitchParameter PublicFolder
		{
			get
			{
				return base.PublicFolder;
			}
			set
			{
				base.PublicFolder = value;
			}
		}

		private new bool QueryBaseDNRestrictionEnabled
		{
			get
			{
				return base.QueryBaseDNRestrictionEnabled;
			}
		}

		private new RemoteAccountPolicyIdParameter RemoteAccountPolicy
		{
			get
			{
				return base.RemoteAccountPolicy;
			}
		}

		private new SwitchParameter RemoteArchive
		{
			get
			{
				return base.RemoteArchive;
			}
		}

		private new bool RemotePowerShellEnabled
		{
			get
			{
				return base.RemotePowerShellEnabled;
			}
		}

		private new RemovedMailboxIdParameter RemovedMailbox
		{
			get
			{
				return base.RemovedMailbox;
			}
		}

		private new bool ResetPasswordOnNextLogon
		{
			get
			{
				return base.ResetPasswordOnNextLogon;
			}
		}

		private new MailboxPolicyIdParameter RetentionPolicy
		{
			get
			{
				return base.RetentionPolicy;
			}
		}

		private new MailboxPolicyIdParameter RoleAssignmentPolicy
		{
			get
			{
				return base.RoleAssignmentPolicy;
			}
		}

		private new SwitchParameter Room
		{
			get
			{
				return base.Room;
			}
		}

		private new string SamAccountName
		{
			get
			{
				return base.SamAccountName;
			}
		}

		private new TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return base.SendModerationNotifications;
			}
		}

		private new SwitchParameter Shared
		{
			get
			{
				return base.Shared;
			}
		}

		private new SharingPolicyIdParameter SharingPolicy
		{
			get
			{
				return base.SharingPolicy;
			}
		}

		private new bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
		}

		private new MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
		}

		private new Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
		}

		private new SwitchParameter TargetAllMDBs
		{
			get
			{
				return base.TargetAllMDBs;
			}
		}

		private new ThrottlingPolicyIdParameter ThrottlingPolicy
		{
			get
			{
				return base.ThrottlingPolicy;
			}
		}

		private new CountryInfo UsageLocation
		{
			get
			{
				return base.UsageLocation;
			}
		}

		private new SwitchParameter UseExistingLiveId
		{
			get
			{
				return base.UseExistingLiveId;
			}
		}

		private new string UserPrincipalName
		{
			get
			{
				return base.UserPrincipalName;
			}
		}

		public UndoSyncSoftDeletedMailbox()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfUndoSyncSoftDeletedMailboxCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulUndoSyncSoftDeletedMailboxCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalUndoSyncSoftDeletedMailboxResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.UndoSyncSoftDeletedMailboxCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.UndoSyncSoftDeletedMailboxCacheActivePercentageBase;
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			ADUser aduser = (ADUser)result;
			if (this.mailboxPlanObject != null)
			{
				aduser.MailboxPlanName = this.mailboxPlanObject.DisplayName;
			}
			aduser.ResetChangeTracking();
			SyncMailbox result2 = new SyncMailbox(aduser);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncMailbox.FromDataObject((ADUser)dataObject);
		}
	}
}
