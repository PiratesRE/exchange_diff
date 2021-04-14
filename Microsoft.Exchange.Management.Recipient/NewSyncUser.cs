using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "SyncUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewSyncUser : NewGeneralRecipientObjectTask<ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewSyncUser(base.Name.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return this.DataObject.OnPremisesObjectId;
			}
			set
			{
				this.DataObject.OnPremisesObjectId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return this.DataObject.IsDirSynced;
			}
			set
			{
				this.DataObject.IsDirSynced = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return this.DataObject.DirSyncAuthorityMetadata;
			}
			set
			{
				this.DataObject.DirSyncAuthorityMetadata = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveID")]
		public SmtpAddress WindowsLiveID
		{
			get
			{
				return this.DataObject.WindowsLiveID;
			}
			set
			{
				this.DataObject.WindowsLiveID = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveID")]
		public NetID NetID
		{
			get
			{
				return this.DataObject.NetID;
			}
			set
			{
				this.DataObject.NetID = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo UsageLocation
		{
			get
			{
				return this.DataObject.UsageLocation;
			}
			set
			{
				this.DataObject.UsageLocation = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return this.DataObject.ReleaseTrack;
			}
			set
			{
				this.DataObject.ReleaseTrack = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return this.DataObject.RemoteRecipientType;
			}
			set
			{
				this.DataObject.RemoteRecipientType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ValidationOrganization
		{
			get
			{
				return (string)base.Fields["ValidationOrganization"];
			}
			set
			{
				base.Fields["ValidationOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AccountDisabled
		{
			get
			{
				return (SwitchParameter)base.Fields[SyncUserSchema.AccountDisabled];
			}
			set
			{
				base.Fields[SyncUserSchema.AccountDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return this.DataObject.StsRefreshTokensValidFrom;
			}
			set
			{
				this.DataObject.StsRefreshTokensValidFrom = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(user);
			if (user.IsModified(ADRecipientSchema.WindowsLiveID) && user.WindowsLiveID != SmtpAddress.Empty)
			{
				user.UserPrincipalName = user.WindowsLiveID.ToString();
			}
			if (!user.IsModified(ADRecipientSchema.DisplayName))
			{
				user.DisplayName = user.Name;
			}
			if (!user.IsModified(IADSecurityPrincipalSchema.SamAccountName))
			{
				user.SamAccountName = RecipientTaskHelper.GenerateUniqueSamAccountName(base.PartitionOrRootOrgGlobalCatalogSession, user.Id.DomainId, user.Name, false, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), true);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.DataSession.Save(this.DataObject);
			base.WriteVerbose(Strings.VerboseSettingPassword(this.DataObject.Id.ToString()));
			MailboxTaskHelper.SetMailboxPassword((IRecipientSession)base.DataSession, this.DataObject, null, new Task.ErrorLoggerDelegate(base.WriteError));
			this.DataObject = (ADUser)base.DataSession.Read<ADUser>(this.DataObject.Identity);
			this.DataObject[ADUserSchema.PasswordLastSetRaw] = new long?(-1L);
			this.DataObject.UserAccountControl = UserAccountControlFlags.NormalAccount;
			if (base.Fields.IsModified(SyncMailUserSchema.AccountDisabled))
			{
				SyncTaskHelper.SetExchangeAccountDisabled(this.DataObject, this.AccountDisabled);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncUser.FromDataObject((ADUser)dataObject);
		}
	}
}
