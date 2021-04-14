using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "RemoteMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "EnabledUser")]
	public sealed class NewRemoteMailbox : NewMailUserBase
	{
		public NewRemoteMailbox()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfNewRemoteMailboxCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulNewRemoteMailboxCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageNewRemoteMailboxResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageNewRemoteMailboxResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageNewRemoteMailboxResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalNewRemoteMailboxResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.NewRemoteMailboxCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.NewRemoteMailboxCacheActivePercentageBase;
		}

		[Parameter]
		public ProxyAddress RemoteRoutingAddress
		{
			get
			{
				return this.ExternalEmailAddress;
			}
			set
			{
				this.ExternalEmailAddress = value;
			}
		}

		[Parameter]
		public OrganizationalUnitIdParameter OnPremisesOrganizationalUnit
		{
			get
			{
				return this.OrganizationalUnit;
			}
			set
			{
				this.OrganizationalUnit = value;
			}
		}

		[Parameter]
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

		[Parameter(Mandatory = true, ParameterSetName = "Room")]
		public SwitchParameter Room
		{
			get
			{
				return (SwitchParameter)(base.Fields["Room"] ?? false);
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Equipment")]
		public SwitchParameter Equipment
		{
			get
			{
				return (SwitchParameter)(base.Fields["Equipment"] ?? false);
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
				return (SwitchParameter)(base.Fields["Shared"] ?? false);
			}
			set
			{
				base.Fields["Shared"] = value;
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

		[Parameter(Mandatory = false)]
		public SwitchParameter ACLableSyncedObjectEnabled
		{
			get
			{
				return (SwitchParameter)(base.Fields["ACLableSyncedObjectEnabled"] ?? false);
			}
			set
			{
				base.Fields["ACLableSyncedObjectEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
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

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(RemoteMailbox).FullName;
			}
		}

		public override bool UsePreferMessageFormat
		{
			get
			{
				return base.UsePreferMessageFormat;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override MessageFormat MessageFormat
		{
			get
			{
				return base.MessageFormat;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return base.MessageBodyFormat;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return base.MacAttachmentFormat;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private new ProxyAddress ExternalEmailAddress
		{
			get
			{
				return base.ExternalEmailAddress;
			}
			set
			{
				base.ExternalEmailAddress = value;
			}
		}

		private new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return base.OrganizationalUnit;
			}
			set
			{
				base.OrganizationalUnit = value;
			}
		}

		private new WindowsLiveId WindowsLiveID
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

		private new WindowsLiveId MicrosoftOnlineServicesID
		{
			get
			{
				return base.MicrosoftOnlineServicesID;
			}
			set
			{
				base.MicrosoftOnlineServicesID = value;
			}
		}

		private new SwitchParameter UseExistingLiveId
		{
			get
			{
				return base.UseExistingLiveId;
			}
			set
			{
				base.UseExistingLiveId = value;
			}
		}

		private new NetID NetID
		{
			get
			{
				return base.NetID;
			}
			set
			{
				base.NetID = value;
			}
		}

		private new SwitchParameter ImportLiveId
		{
			get
			{
				return base.ImportLiveId;
			}
			set
			{
				base.ImportLiveId = value;
			}
		}

		private new SwitchParameter BypassLiveId
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

		private new SwitchParameter EvictLiveId
		{
			get
			{
				return base.EvictLiveId;
			}
			set
			{
				base.EvictLiveId = value;
			}
		}

		private new string FederatedIdentity
		{
			get
			{
				return base.FederatedIdentity;
			}
			set
			{
				base.FederatedIdentity = value;
			}
		}

		private new string ExternalDirectoryObjectId
		{
			get
			{
				return base.ExternalDirectoryObjectId;
			}
			set
			{
				base.ExternalDirectoryObjectId = value;
			}
		}

		private new OrganizationIdParameter Organization
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

		private new CountryInfo UsageLocation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRemoteMailbox(base.Name.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		private bool primarySmtpAddressAssginedByExternalEmailAddress { get; set; }

		protected override bool GetEmailAddressPolicyEnabledDefaultValue(IConfigurable dataObject)
		{
			return base.GetEmailAddressPolicyEnabledDefaultValue(dataObject) || this.primarySmtpAddressAssginedByExternalEmailAddress;
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			if (null == this.RemoteRoutingAddress)
			{
				if (this.remoteRoutingAddressGenerator == null)
				{
					this.remoteRoutingAddressGenerator = new RemoteRoutingAddressGenerator(this.ConfigurationSession);
				}
				user.ExternalEmailAddress = this.remoteRoutingAddressGenerator.GenerateRemoteRoutingAddress(user.Alias, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (user.ExternalEmailAddress != null)
			{
				bool flag = false;
				foreach (ProxyAddress a in user.EmailAddresses)
				{
					if (a == user.ExternalEmailAddress)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (user.PrimarySmtpAddress == SmtpAddress.Empty)
					{
						this.primarySmtpAddressAssginedByExternalEmailAddress = true;
					}
					user.EmailAddresses.Add(user.ExternalEmailAddress.ToSecondary());
				}
			}
			user.RemoteRecipientType = RemoteRecipientType.ProvisionMailbox;
			RemoteMailboxType remoteMailboxType = (RemoteMailboxType)((ulong)int.MinValue);
			if (this.Room.IsPresent)
			{
				remoteMailboxType = RemoteMailboxType.Room;
			}
			else if (this.Equipment.IsPresent)
			{
				remoteMailboxType = RemoteMailboxType.Equipment;
			}
			else if (this.Shared.IsPresent)
			{
				remoteMailboxType = RemoteMailboxType.Shared;
			}
			user.UpdateRemoteMailboxType(remoteMailboxType, this.ACLableSyncedObjectEnabled);
			if (this.Archive.IsPresent)
			{
				user.ArchiveGuid = Guid.NewGuid();
				user.ArchiveName = new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + (string.IsNullOrEmpty(user.DisplayName) ? user.Name : user.DisplayName));
				user.RemoteRecipientType |= RemoteRecipientType.ProvisionArchive;
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesAfterSettingPassword()
		{
			base.StampChangesAfterSettingPassword();
			if (base.ParameterSetName == "Room" || base.ParameterSetName == "Equipment" || base.ParameterSetName == "Shared")
			{
				this.DataObject.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
				if (!base.ResetPasswordOnNextLogon && (this.Password == null || this.Password.Length == 0))
				{
					this.DataObject.UserAccountControl |= UserAccountControlFlags.DoNotExpirePassword;
				}
			}
			if (this.DataObject.LegacyExchangeDN != null)
			{
				this.DataObject.LegacyExchangeDN = this.DataObject.LegacyExchangeDN.Replace("Exchange Administrative Group (FYDIBOHF23SPDLT)", "External (FYDIBOHF25SPDLT)");
			}
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			RemoteMailbox result2 = new RemoteMailbox((ADUser)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return RemoteMailbox.FromDataObject((ADUser)dataObject);
		}

		public const string ParameterACLableSyncedEnabled = "ACLableSyncedObjectEnabled";

		private RemoteRoutingAddressGenerator remoteRoutingAddressGenerator;
	}
}
