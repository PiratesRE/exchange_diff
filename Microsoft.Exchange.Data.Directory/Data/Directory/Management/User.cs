using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class User : OrgPersonPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return User.schema;
			}
		}

		public User()
		{
			base.SetObjectClass("user");
		}

		public User(ADUser dataObject) : base(dataObject)
		{
		}

		internal static User FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new User(dataObject);
		}

		protected override IEnumerable<PropertyInfo> CloneableProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = User.cloneableProps) == null)
				{
					result = (User.cloneableProps = ADPresentationObject.GetCloneableProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableOnceProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = User.cloneableOnceProps) == null)
				{
					result = (User.cloneableOnceProps = ADPresentationObject.GetCloneableOnceProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableEnabledStateProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = User.cloneableEnabledStateProps) == null)
				{
					result = (User.cloneableEnabledStateProps = ADPresentationObject.GetCloneableEnabledStateProperties(this));
				}
				return result;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		public bool IsSecurityPrincipal
		{
			get
			{
				return (bool)this[UserSchema.IsSecurityPrincipal];
			}
		}

		[Parameter(Mandatory = false)]
		public string SamAccountName
		{
			get
			{
				return (string)this[UserSchema.SamAccountName];
			}
			set
			{
				this[UserSchema.SamAccountName] = value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[UserSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[UserSchema.SidHistory];
			}
		}

		[Parameter(Mandatory = false)]
		public string UserPrincipalName
		{
			get
			{
				return (string)this[UserSchema.UserPrincipalName];
			}
			set
			{
				this[UserSchema.UserPrincipalName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalCloneOnce(CloneSet.CloneLimitedSet)]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)this[UserSchema.ResetPasswordOnNextLogon];
			}
			set
			{
				this[UserSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<X509Identifier> CertificateSubject
		{
			get
			{
				return (MultiValuedProperty<X509Identifier>)this[ADUserSchema.CertificateSubject];
			}
			set
			{
				this[ADUserSchema.CertificateSubject] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemotePowerShellEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.RemotePowerShellEnabled];
			}
			set
			{
				this[ADRecipientSchema.RemotePowerShellEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.WindowsLiveID];
			}
			set
			{
				this[ADRecipientSchema.WindowsLiveID] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress MicrosoftOnlineServicesID
		{
			get
			{
				return this.WindowsLiveID;
			}
			set
			{
				this.WindowsLiveID = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NetID NetID
		{
			get
			{
				return (NetID)this[ADUserSchema.NetID];
			}
			set
			{
				this[ADUserSchema.NetID] = value;
			}
		}

		public NetID ConsumerNetID
		{
			get
			{
				return (NetID)this[ADUserSchema.ConsumerNetID];
			}
			set
			{
				this[ADUserSchema.ConsumerNetID] = value;
			}
		}

		public bool LEOEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.LEOEnabled];
			}
			set
			{
				this[ADRecipientSchema.LEOEnabled] = value;
			}
		}

		public UserAccountControlFlags UserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[ADUserSchema.UserAccountControl];
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[UserSchema.OrganizationalUnit];
			}
		}

		public bool IsLinked
		{
			get
			{
				return (bool)this[UserSchema.IsLinked];
			}
		}

		public string LinkedMasterAccount
		{
			get
			{
				return (string)this[UserSchema.LinkedMasterAccount];
			}
			internal set
			{
				this[UserSchema.LinkedMasterAccount] = value;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[UserSchema.ExternalDirectoryObjectId];
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[UserSchema.SKUAssigned];
			}
		}

		public bool IsSoftDeletedByRemove
		{
			get
			{
				return (bool)this[MailboxSchema.IsSoftDeletedByRemove];
			}
			set
			{
				this[MailboxSchema.IsSoftDeletedByRemove] = value;
			}
		}

		public bool IsSoftDeletedByDisable
		{
			get
			{
				return (bool)this[MailboxSchema.IsSoftDeletedByDisable];
			}
			set
			{
				this[MailboxSchema.IsSoftDeletedByDisable] = value;
			}
		}

		public DateTime? WhenSoftDeleted
		{
			get
			{
				return (DateTime?)this[MailboxSchema.WhenSoftDeleted];
			}
			set
			{
				this[MailboxSchema.WhenSoftDeleted] = value;
			}
		}

		public RecipientTypeDetails PreviousRecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[UserSchema.PreviousRecipientTypeDetails];
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return (UpgradeRequestTypes)this[UserSchema.UpgradeRequest];
			}
			set
			{
				this[UserSchema.UpgradeRequest] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeStatusTypes UpgradeStatus
		{
			get
			{
				return (UpgradeStatusTypes)this[UserSchema.UpgradeStatus];
			}
			set
			{
				this[UserSchema.UpgradeStatus] = value;
			}
		}

		public string UpgradeDetails
		{
			get
			{
				return (string)this[UserSchema.UpgradeDetails];
			}
		}

		public string UpgradeMessage
		{
			get
			{
				return (string)this[UserSchema.UpgradeMessage];
			}
		}

		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)this[UserSchema.UpgradeStage];
			}
		}

		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)this[UserSchema.UpgradeStageTimeStamp];
			}
		}

		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)this[UserSchema.MailboxProvisioningConstraint];
			}
		}

		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)this[UserSchema.MailboxProvisioningPreferences];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> InPlaceHoldsRaw
		{
			get
			{
				return (MultiValuedProperty<string>)this[UserSchema.InPlaceHoldsRaw];
			}
			set
			{
				this[UserSchema.InPlaceHoldsRaw] = value;
			}
		}

		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[UserSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[UserSchema.MailboxRelease] = value.ToString();
			}
		}

		public MailboxRelease ArchiveRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[UserSchema.ArchiveRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[UserSchema.ArchiveRelease] = value.ToString();
			}
		}

		private static UserSchema schema = ObjectSchema.GetInstance<UserSchema>();

		private static IEnumerable<PropertyInfo> cloneableProps;

		private static IEnumerable<PropertyInfo> cloneableOnceProps;

		private static IEnumerable<PropertyInfo> cloneableEnabledStateProps;
	}
}
