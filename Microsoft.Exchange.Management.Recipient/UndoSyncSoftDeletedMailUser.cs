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
	[Cmdlet("Undo", "SyncSoftDeletedMailUser", SupportsShouldProcess = true, DefaultParameterSetName = "SoftDeletedMailUser")]
	public sealed class UndoSyncSoftDeletedMailUser : NewMailUserBase
	{
		public UndoSyncSoftDeletedMailUser()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfUndoSyncSoftDeletedMailuserCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulUndoSyncSoftDeletedMailuserCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalUndoSyncSoftDeletedMailuserResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.UndoSyncSoftDeletedMailuserCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.UndoSyncSoftDeletedMailuserCacheActivePercentageBase;
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "SoftDeletedMailUser", ValueFromPipeline = true)]
		public new MailUserIdParameter SoftDeletedObject
		{
			get
			{
				return (MailUserIdParameter)base.SoftDeletedObject;
			}
			set
			{
				base.SoftDeletedObject = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailUser")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailUser")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailUser")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SoftDeletedMailUser")]
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

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			SmtpAddress windowsLiveID = this.DataObject.WindowsLiveID;
			NetID netID = this.DataObject.NetID;
			string name = this.DataObject.Name;
			string displayName = this.DataObject.DisplayName;
			this.DataObject = SoftDeletedTaskHelper.GetSoftDeletedADUser(base.CurrentOrganizationId, this.SoftDeletedObject, new Task.ErrorLoggerDelegate(base.WriteError));
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
		}

		protected override void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareUserObject(user);
			if (this.WindowsLiveID != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				user.EmailAddressPolicyEnabled = false;
				SmtpProxyAddress item = new SmtpProxyAddress(this.WindowsLiveID.SmtpAddress.ToString(), false);
				if (!user.EmailAddresses.Contains(item))
				{
					user.EmailAddresses.Add(item);
				}
			}
			if (user.ExchangeGuid == SoftDeletedTaskHelper.PredefinedExchangeGuid)
			{
				user.ExchangeGuid = user.PreviousExchangeGuid;
				if (!RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(user, ADMailboxRecipientSchema.ExchangeGuid, user.ExchangeGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client))
				{
					user.ExchangeGuid = Guid.Empty;
				}
				user.PreviousExchangeGuid = Guid.Empty;
			}
			SoftDeletedTaskHelper.UpdateShadowWhenSoftDeletedProperty((IRecipientSession)base.DataSession, this.ConfigurationSession, base.CurrentOrganizationId, this.DataObject);
			this.DataObject.RecipientSoftDeletedStatus = 0;
			this.DataObject.WhenSoftDeleted = null;
			this.DataObject.InternalOnly = false;
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			SyncMailUser result2 = new SyncMailUser((ADUser)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(SyncMailUser).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncMailUser.FromDataObject((ADUser)dataObject);
		}

		private new string Alias
		{
			get
			{
				return base.Alias;
			}
		}

		private new MailboxIdParameter ArbitrationMailbox
		{
			get
			{
				return base.ArbitrationMailbox;
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

		private new ProxyAddress ExternalEmailAddress
		{
			get
			{
				return base.ExternalEmailAddress;
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

		private new MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return base.MacAttachmentFormat;
			}
		}

		private new MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return base.MessageBodyFormat;
			}
		}

		private new MessageFormat MessageFormat
		{
			get
			{
				return base.MessageFormat;
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

		private new bool RemotePowerShellEnabled
		{
			get
			{
				return base.RemotePowerShellEnabled;
			}
		}

		private new bool ResetPasswordOnNextLogon
		{
			get
			{
				return base.ResetPasswordOnNextLogon;
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

		private new bool UsePreferMessageFormat
		{
			get
			{
				return base.UsePreferMessageFormat;
			}
		}

		private new string UserPrincipalName
		{
			get
			{
				return base.UserPrincipalName;
			}
		}
	}
}
