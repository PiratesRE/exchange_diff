using System;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "User", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetUser : SetADUserBase<UserIdParameter, User>
	{
		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter EnableAccount
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnableAccount"] ?? false);
			}
			set
			{
				base.Fields["EnableAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields[UserSchema.LinkedMasterAccount];
			}
			set
			{
				base.Fields[UserSchema.LinkedMasterAccount] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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
		public NetID BusinessNetID
		{
			get
			{
				return (NetID)base.Fields["BusinessNetID"];
			}
			set
			{
				base.Fields["BusinessNetID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter CopyShadowAttributes
		{
			get
			{
				return (SwitchParameter)(base.Fields["CopyShadowAttributes"] ?? false);
			}
			set
			{
				base.Fields["CopyShadowAttributes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter GenerateExternalDirectoryObjectId
		{
			get
			{
				return (SwitchParameter)(base.Fields["GenerateExternalDirectoryObjectId"] ?? false);
			}
			set
			{
				base.Fields["GenerateExternalDirectoryObjectId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LEOEnabled
		{
			get
			{
				return (bool)base.Fields[ADRecipientSchema.LEOEnabled];
			}
			set
			{
				base.Fields[ADRecipientSchema.LEOEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UpgradeMessage
		{
			get
			{
				return (string)base.Fields["UpgradeMessage"];
			}
			set
			{
				base.Fields["UpgradeMessage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UpgradeDetails
		{
			get
			{
				return (string)base.Fields["UpgradeDetails"];
			}
			set
			{
				base.Fields["UpgradeDetails"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)base.Fields["UpgradeStage"];
			}
			set
			{
				base.Fields["UpgradeStage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)base.Fields["UpgradeStageTimeStamp"];
			}
			set
			{
				base.Fields["UpgradeStageTimeStamp"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)base.Fields["MailboxRelease"], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				base.Fields["MailboxRelease"] = value.ToString();
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxRelease ArchiveRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)base.Fields["ArchiveRelease"], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				base.Fields["ArchiveRelease"] = value.ToString();
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.LinkedMasterAccount != null)
			{
				this.ValidateLinkedMasterAccount();
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADUser aduser = (ADUser)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(aduser, this.Arbitration) || MailboxTaskHelper.ExcludePublicFolderMailbox(aduser, this.PublicFolder) || MailboxTaskHelper.ExcludeMailboxPlan(aduser, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, this.Identity);
			}
			if (base.Fields.IsModified(UserSchema.LinkedMasterAccount))
			{
				aduser.MasterAccountSid = this.linkedUserSid;
				if (aduser.MasterAccountSid != null)
				{
					this.ResolveLinkedUser(aduser);
				}
				else
				{
					this.ResolveUnlinkedUser(aduser);
				}
			}
			return aduser;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			if (this.GenerateExternalDirectoryObjectId && string.IsNullOrEmpty(this.DataObject.ExternalDirectoryObjectId))
			{
				this.DataObject.ExternalDirectoryObjectId = Guid.NewGuid().ToString();
			}
			if (this.BusinessNetID != null)
			{
				this.DataObject.ConsumerNetID = this.DataObject.NetID;
				this.DataObject.NetID = this.BusinessNetID;
			}
			if (this.CopyShadowAttributes)
			{
				foreach (PropertyDefinition propertyDefinition in this.DataObject.Schema.AllProperties)
				{
					ADPropertyDefinition adpropertyDefinition = propertyDefinition as ADPropertyDefinition;
					if (adpropertyDefinition != null)
					{
						object value = null;
						if (adpropertyDefinition.ShadowProperty != null && this.DataObject.propertyBag.TryGetField(adpropertyDefinition, ref value))
						{
							this.DataObject.propertyBag[adpropertyDefinition.ShadowProperty] = value;
						}
					}
				}
			}
			if (this.EnableAccount.IsPresent && this.DataObject.UserAccountControl == (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.PasswordNotRequired | UserAccountControlFlags.NormalAccount))
			{
				this.DataObject.UserAccountControl = UserAccountControlFlags.NormalAccount;
				using (SecureString randomPassword = MailboxTaskUtilities.GetRandomPassword(this.DataObject.Name, this.DataObject.SamAccountName))
				{
					recipientSession.SetPassword(this.DataObject, randomPassword);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.LEOEnabled))
			{
				this.DataObject.LEOEnabled = this.LEOEnabled;
			}
			if (base.Fields.IsModified("UpgradeMessage"))
			{
				this.DataObject.UpgradeMessage = this.UpgradeMessage;
			}
			if (base.Fields.IsModified("UpgradeDetails"))
			{
				this.DataObject.UpgradeDetails = this.UpgradeDetails;
			}
			if (base.Fields.IsModified("UpgradeStage"))
			{
				this.DataObject.UpgradeStage = this.UpgradeStage;
			}
			if (base.Fields.IsModified("UpgradeStageTimeStamp"))
			{
				this.DataObject.UpgradeStageTimeStamp = this.UpgradeStageTimeStamp;
			}
			if (base.Fields.IsModified("MailboxRelease"))
			{
				this.DataObject.MailboxRelease = this.MailboxRelease;
			}
			if (base.Fields.IsModified("ArchiveRelease"))
			{
				this.DataObject.ArchiveRelease = this.ArchiveRelease;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			bool flag = base.CurrentTaskContext.ExchangeRunspaceConfig != null && base.CurrentTaskContext.ExchangeRunspaceConfig.IsAppPasswordUsed;
			if (base.UserSpecifiedParameters.IsModified(UserSchema.ResetPasswordOnNextLogon) && flag)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorChangePasswordForAppPasswordAccount), ExchangeErrorCategory.Client, this.Identity);
			}
			base.InternalValidate();
			if (this.GenerateExternalDirectoryObjectId && (RecipientTaskHelper.GetAcceptedRecipientTypes() & this.DataObject.RecipientTypeDetails) == RecipientTypeDetails.None)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotGenerateExternalDirectoryObjectIdOnInternalRecipientType(this.Identity.ToString(), this.DataObject.RecipientTypeDetails.ToString())), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		private void ValidateLinkedMasterAccount()
		{
			if (string.IsNullOrEmpty(this.LinkedDomainController))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMissLinkedDomainController), ErrorCategory.InvalidArgument, this.Identity);
			}
			try
			{
				NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
				this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			}
			catch (PSArgumentException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
			}
		}

		private void ResolveLinkedUser(ADUser user)
		{
			if (user.RecipientTypeDetails == RecipientTypeDetails.UserMailbox)
			{
				user.RecipientTypeDetails = RecipientTypeDetails.LinkedMailbox;
			}
			else if (user.RecipientTypeDetails == RecipientTypeDetails.RoomMailbox)
			{
				user.RecipientTypeDetails = RecipientTypeDetails.LinkedRoomMailbox;
			}
			this.GrantPermissionToLinkedUser(user);
			if (user.RecipientTypeDetails != RecipientTypeDetails.LinkedRoomMailbox)
			{
				user.RecipientDisplayType = new RecipientDisplayType?(SetUser.TryToSetACLableFlag(user.RecipientDisplayType.Value));
			}
		}

		private void ResolveUnlinkedUser(ADUser user)
		{
			if (user.RecipientTypeDetails == RecipientTypeDetails.LinkedMailbox)
			{
				user.RecipientTypeDetails = RecipientTypeDetails.UserMailbox;
			}
			if (this.IsAccountDisabled(user))
			{
				user.RecipientDisplayType = new RecipientDisplayType?(SetUser.TryToClearACLableFlag(user.RecipientDisplayType.Value));
			}
		}

		private bool IsAccountDisabled(ADUser user)
		{
			return (user.UserAccountControl & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.AccountDisabled;
		}

		private void GrantPermissionToLinkedUser(ADUser user)
		{
			if (this.IsAccountDisabled(user))
			{
				return;
			}
			user.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
			MailboxTaskHelper.GrantPermissionToLinkedUserAccount(user, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.VerboseSaveADSecurityDescriptor(user.Id.ToString()));
			}
			user.SaveSecurityDescriptor(((SecurityDescriptor)user[ADObjectSchema.NTSecurityDescriptor]).ToRawSecurityDescriptor());
		}

		private static RecipientDisplayType TryToSetACLableFlag(RecipientDisplayType displayType)
		{
			int num = 1073741824;
			if ((displayType & (RecipientDisplayType)num) == (RecipientDisplayType)num)
			{
				return displayType;
			}
			if (displayType == RecipientDisplayType.SyncedUSGasUDG)
			{
				return RecipientDisplayType.SyncedUSGasUSG;
			}
			if (displayType == RecipientDisplayType.DistributionGroup)
			{
				return RecipientDisplayType.SecurityDistributionGroup;
			}
			int num2 = (int)(displayType | (RecipientDisplayType)num);
			if (!Enum.IsDefined(typeof(RecipientDisplayType), num2))
			{
				return displayType;
			}
			return (RecipientDisplayType)num2;
		}

		private static RecipientDisplayType TryToClearACLableFlag(RecipientDisplayType displayType)
		{
			int num = 1073741824;
			if ((displayType & (RecipientDisplayType)num) != (RecipientDisplayType)num)
			{
				return displayType;
			}
			if (displayType == RecipientDisplayType.SyncedUSGasUSG)
			{
				return RecipientDisplayType.SyncedUSGasUDG;
			}
			if (displayType == RecipientDisplayType.SecurityDistributionGroup)
			{
				return RecipientDisplayType.DistributionGroup;
			}
			int num2 = (int)(displayType & (RecipientDisplayType)(~(RecipientDisplayType)num));
			if (!Enum.IsDefined(typeof(RecipientDisplayType), num2))
			{
				return displayType;
			}
			return (RecipientDisplayType)num2;
		}

		public const string UpgradeMessageParameter = "UpgradeMessage";

		public const string UpgradeDetailsParameter = "UpgradeDetails";

		public const string UpgradeStageParameter = "UpgradeStage";

		public const string UpgradeStageTimeStampParameter = "UpgradeStageTimeStamp";

		public const string MailboxReleaseParameter = "MailboxRelease";

		public const string ArchiveReleaseParameter = "ArchiveRelease";

		private SecurityIdentifier linkedUserSid;
	}
}
