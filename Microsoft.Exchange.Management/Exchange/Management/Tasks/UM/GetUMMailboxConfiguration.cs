using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMMailboxConfiguration", DefaultParameterSetName = "Identity")]
	public sealed class GetUMMailboxConfiguration : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
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
						UMMailboxConfiguration presentationObject = this.GetPresentationObject(umsubscriber);
						base.WriteObject(presentationObject);
						goto IL_57;
					}
					catch (UserConfigurationException exception)
					{
						base.WriteError(exception, (ErrorCategory)1001, null);
						goto IL_57;
					}
				}
				base.WriteError(new UserNotUmEnabledException(this.Identity.ToString()), (ErrorCategory)1000, null);
				IL_57:;
			}
			TaskLogger.LogExit();
		}

		private UMMailboxConfiguration GetPresentationObject(UMSubscriber subscriber)
		{
			UMMailboxConfiguration ummailboxConfiguration = new UMMailboxConfiguration(this.DataObject.Identity);
			ummailboxConfiguration.Greeting = subscriber.ConfigFolder.CurrentMailboxGreetingType;
			ummailboxConfiguration.DefaultPlayOnPhoneNumber = subscriber.ConfigFolder.PlayOnPhoneDialString;
			ummailboxConfiguration.ReadOldestUnreadVoiceMessagesFirst = subscriber.ConfigFolder.ReadUnreadVoicemailInFIFOOrder;
			ummailboxConfiguration.ReceivedVoiceMailPreviewEnabled = subscriber.ConfigFolder.ReceivedVoiceMailPreviewEnabled;
			ummailboxConfiguration.SentVoiceMailPreviewEnabled = subscriber.ConfigFolder.SentVoiceMailPreviewEnabled;
			byte[] entryId = Convert.FromBase64String(subscriber.ConfigFolder.TelephoneAccessFolderEmail);
			StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
			ADUser dataObject = this.DataObject;
			MailboxFolderId identity = new MailboxFolderId(dataObject.Id, storeObjectId, null);
			using (MailboxFolderDataProvider mailboxFolderDataProvider = new MailboxFolderDataProvider(base.SessionSettings, this.DataObject, "get-ummailboxconfiguration"))
			{
				ummailboxConfiguration.FolderToReadEmailsFrom = (MailboxFolder)mailboxFolderDataProvider.Read<MailboxFolder>(identity);
			}
			return ummailboxConfiguration;
		}
	}
}
