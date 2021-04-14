using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Enable", "MailUser", SupportsShouldProcess = true, DefaultParameterSetName = "EnabledUser")]
	public sealed class EnableMailUser : EnableMailUserBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
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

		[Parameter(Mandatory = true, ParameterSetName = "Archive")]
		[ValidateNotEmptyGuid]
		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)(base.Fields[ADUserSchema.ArchiveGuid] ?? Guid.Empty);
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveGuid] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return base.Fields[ADUserSchema.ArchiveName] as MultiValuedProperty<string>;
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassModerationCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassModerationCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassModerationCheck"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress JournalArchiveAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields[ADRecipientSchema.JournalArchiveAddress] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields[ADRecipientSchema.JournalArchiveAddress] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)base.Fields[ADRecipientSchema.ExternalEmailAddress];
			}
			set
			{
				base.Fields[ADRecipientSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public override CountryInfo UsageLocation
		{
			get
			{
				return base.UsageLocation;
			}
			set
			{
				base.UsageLocation = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public bool UsePreferMessageFormat
		{
			get
			{
				return (bool)(base.Fields[ADRecipientSchema.UsePreferMessageFormat] ?? false);
			}
			set
			{
				base.Fields[ADRecipientSchema.UsePreferMessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public MessageFormat MessageFormat
		{
			get
			{
				return (MessageFormat)(base.Fields[ADRecipientSchema.MessageFormat] ?? MessageFormat.Mime);
			}
			set
			{
				base.Fields[ADRecipientSchema.MessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return (MessageBodyFormat)(base.Fields[ADRecipientSchema.MessageBodyFormat] ?? MessageBodyFormat.TextAndHtml);
			}
			set
			{
				base.Fields[ADRecipientSchema.MessageBodyFormat] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return (MacAttachmentFormat)(base.Fields[ADRecipientSchema.MacAttachmentFormat] ?? MacAttachmentFormat.BinHex);
			}
			set
			{
				base.Fields[ADRecipientSchema.MacAttachmentFormat] = value;
			}
		}

		[Parameter]
		public override Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				base.SKUCapability = value;
			}
		}

		[Parameter]
		public override MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				base.AddOnSKUCapability = value;
			}
		}

		[Parameter]
		public override bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				base.SKUAssigned = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedMailUser"] ?? false);
			}
			set
			{
				base.Fields["SoftDeletedMailUser"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PreserveEmailAddresses
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreserveEmailAddresses"] ?? false);
			}
			set
			{
				base.Fields["PreserveEmailAddresses"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				base.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Archive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailUserArchive(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageEnableMailUser(this.Identity.ToString(), this.ExternalEmailAddress.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.InternalProcessRecord();
			if (this.recoverArchive && this.DataObject.ArchiveDatabase != null)
			{
				MailboxDatabase database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(this.DataObject.ArchiveDatabase), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.DataObject.ArchiveDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.DataObject.ArchiveDatabase.ToString())));
				using (MapiAdministrationSession adminSession = MapiTaskHelper.GetAdminSession(RecipientTaskHelper.GetActiveManagerInstance(), this.DataObject.ArchiveDatabase.ObjectGuid))
				{
					ConnectMailbox.UpdateSDAndRefreshMailbox(adminSession, this.DataObject, database, this.DataObject.ArchiveGuid, null, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsModified(ADUserSchema.ArchiveGuid) && this.ArchiveGuid != Guid.Empty)
			{
				RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(this.DataObject, ADUserSchema.ArchiveGuid, this.ArchiveGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
		}

		protected override bool IsValidUser(ADUser user)
		{
			return ("Archive" != base.ParameterSetName && RecipientType.User == user.RecipientType) || ("Archive" == base.ParameterSetName && RecipientType.MailUser == user.RecipientType);
		}

		protected override void PrepareRecipientObject(ref ADUser user)
		{
			TaskLogger.LogEnter();
			ProxyAddressCollection emailAddresses = user.EmailAddresses;
			base.PrepareRecipientObject(ref user);
			if (this.PreserveEmailAddresses)
			{
				user.EmailAddresses = emailAddresses;
			}
			if (this.BypassModerationCheck.IsPresent)
			{
				user.BypassModerationCheck = true;
			}
			if (base.ParameterSetName == "Archive")
			{
				if (user.RecipientType == RecipientType.MailUser)
				{
					this.CreateArchiveIfNecessary(user);
					TaskLogger.LogExit();
					return;
				}
				RecipientIdParameter recipientIdParameter = new RecipientIdParameter((ADObjectId)user.Identity);
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidArchiveRecipientType(recipientIdParameter.ToString(), user.RecipientType.ToString())), ErrorCategory.InvalidArgument, recipientIdParameter);
			}
			if (base.ParameterSetName != "Archive")
			{
				user.ExternalEmailAddress = this.ExternalEmailAddress;
				user.UsePreferMessageFormat = this.UsePreferMessageFormat;
				user.MessageFormat = this.MessageFormat;
				user.MessageBodyFormat = this.MessageBodyFormat;
				user.MacAttachmentFormat = this.MacAttachmentFormat;
				user.UseMapiRichTextFormat = UseMapiRichTextFormat.UseDefaultSettings;
				user.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.RemoteMailUser);
				user.RecipientTypeDetails = RecipientTypeDetails.MailUser;
				MailUserTaskHelper.ValidateExternalEmailAddress(user, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			}
			if (this.JournalArchiveAddress != SmtpAddress.Empty)
			{
				user.JournalArchiveAddress = this.JournalArchiveAddress;
			}
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			MailUser sendToPipeline = new MailUser(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		private void CreateArchiveIfNecessary(ADUser user)
		{
			if (user.ArchiveGuid == Guid.Empty)
			{
				if (user.DisabledArchiveGuid != Guid.Empty && this.ArchiveGuid == user.DisabledArchiveGuid)
				{
					this.recoverArchive = MailboxTaskHelper.IsArchiveRecoverable(user, this.ConfigurationSession, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, user.Id));
					if (this.recoverArchive)
					{
						user.ArchiveDatabase = user.DisabledArchiveDatabase;
					}
				}
				user.ArchiveGuid = this.ArchiveGuid;
				user.ArchiveName = ((this.ArchiveName == null) ? new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + user.DisplayName) : this.ArchiveName);
				user.ArchiveQuota = RecipientConstants.ArchiveAddOnQuota;
				user.ArchiveWarningQuota = RecipientConstants.ArchiveAddOnWarningQuota;
				user.ArchiveStatus |= ArchiveStatusFlags.Active;
				user.AllowArchiveAddressSync = true;
				MailboxTaskHelper.ApplyDefaultArchivePolicy(user, this.ConfigurationSession);
				return;
			}
			base.WriteError(new RecipientTaskException(Strings.ErrorArchiveAlreadyPresent(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
		}

		private bool recoverArchive;
	}
}
