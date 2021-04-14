using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMMailboxConfiguration", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetUMMailboxConfiguration : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter]
		public MailboxGreetingEnum Greeting
		{
			get
			{
				return (MailboxGreetingEnum)base.Fields["Greeting"];
			}
			set
			{
				base.Fields["Greeting"] = value;
			}
		}

		[Parameter]
		public MailboxFolderIdParameter FolderToReadEmailsFrom
		{
			get
			{
				return (MailboxFolderIdParameter)base.Fields["FolderToReadEmailsFrom"];
			}
			set
			{
				base.Fields["FolderToReadEmailsFrom"] = value;
			}
		}

		[Parameter]
		public bool ReadOldestUnreadVoiceMessagesFirst
		{
			get
			{
				return (bool)base.Fields["ReadOldestUnreadVoiceMessageFirst"];
			}
			set
			{
				base.Fields["ReadOldestUnreadVoiceMessageFirst"] = value;
			}
		}

		[Parameter]
		public string DefaultPlayOnPhoneNumber
		{
			get
			{
				return (string)base.Fields["DefaultPlayOnPhoneNumber"];
			}
			set
			{
				base.Fields["DefaultPlayOnPhoneNumber"] = value;
			}
		}

		[Parameter]
		public bool ReceivedVoiceMailPreviewEnabled
		{
			get
			{
				return (bool)base.Fields["ReceivedVoiceMailPreviewEnabled"];
			}
			set
			{
				base.Fields["ReceivedVoiceMailPreviewEnabled"] = value;
			}
		}

		[Parameter]
		public bool SentVoiceMailPreviewEnabled
		{
			get
			{
				return (bool)base.Fields["SentVoiceMailPreviewEnabled"];
			}
			set
			{
				base.Fields["SentVoiceMailPreviewEnabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMMailboxConfiguration(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.ValidateParameters();
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1000, this.Identity);
			}
			return adrecipient;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(this.DataObject))
			{
				if (umsubscriber != null)
				{
					try
					{
						this.SetPropertiesOnUMSubscriber(umsubscriber);
						goto IL_4F;
					}
					catch (UserConfigurationException exception)
					{
						base.WriteError(exception, (ErrorCategory)1001, null);
						goto IL_4F;
					}
				}
				base.WriteError(new UserNotUmEnabledException(this.Identity.ToString()), (ErrorCategory)1000, null);
				IL_4F:;
			}
			TaskLogger.LogExit();
		}

		private void ValidateParameters()
		{
			if (base.Fields.IsModified("FolderToReadEmailsFrom"))
			{
				this.mailboxFolder = ManageInboxRule.ResolveMailboxFolder(this.FolderToReadEmailsFrom, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADUser>), new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<MailboxFolder>), base.TenantGlobalCatalogSession, base.SessionSettings, this.DataObject, new ManageInboxRule.ThrowTerminatingErrorDelegate(base.WriteError));
			}
		}

		private void SetPropertiesOnUMSubscriber(UMSubscriber subscriber)
		{
			bool flag = false;
			if (base.Fields.IsModified("Greeting"))
			{
				subscriber.ConfigFolder.CurrentMailboxGreetingType = this.Greeting;
				flag = true;
			}
			if (base.Fields.IsModified("ReadOldestUnreadVoiceMessageFirst"))
			{
				subscriber.ConfigFolder.ReadUnreadVoicemailInFIFOOrder = this.ReadOldestUnreadVoiceMessagesFirst;
				flag = true;
			}
			if (base.Fields.IsModified("DefaultPlayOnPhoneNumber"))
			{
				subscriber.ConfigFolder.PlayOnPhoneDialString = this.DefaultPlayOnPhoneNumber;
				flag = true;
			}
			if (base.Fields.IsModified("ReceivedVoiceMailPreviewEnabled"))
			{
				subscriber.ConfigFolder.ReceivedVoiceMailPreviewEnabled = this.ReceivedVoiceMailPreviewEnabled;
				flag = true;
			}
			if (base.Fields.IsModified("SentVoiceMailPreviewEnabled"))
			{
				subscriber.ConfigFolder.SentVoiceMailPreviewEnabled = this.SentVoiceMailPreviewEnabled;
				flag = true;
			}
			if (this.mailboxFolder != null)
			{
				StoreObjectId objectId = this.mailboxFolder.InternalFolderIdentity.ObjectId;
				subscriber.ConfigFolder.TelephoneAccessFolderEmail = Convert.ToBase64String(objectId.ProviderLevelItemId);
				flag = true;
			}
			if (flag)
			{
				subscriber.ConfigFolder.Save();
			}
		}

		private MailboxFolder mailboxFolder;
	}
}
