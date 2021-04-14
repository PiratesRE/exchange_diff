using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.SendAsDefaults;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxMessageConfiguration", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxMessageConfiguration : SetXsoObjectWithIdentityTaskBase<MailboxMessageConfiguration>
	{
		protected override void InternalValidate()
		{
			base.InternalValidate();
			base.VerifyIsWithinScopes((IRecipientSession)base.DataSession, this.DataObject, true, new DataAccessTask<ADUser>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMailboxMessageConfiguration(this.Identity.ToString());
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			XsoDictionaryDataProvider xsoDictionaryDataProvider = new XsoDictionaryDataProvider(principal, "Set-MailboxMessageConfiguration");
			this.mailboxSession = xsoDictionaryDataProvider.MailboxSession;
			return xsoDictionaryDataProvider;
		}

		protected override void StampChangesOnXsoObject(IConfigurable dataObject)
		{
			base.StampChangesOnXsoObject(dataObject);
			MailboxMessageConfiguration mailboxMessageConfiguration = (MailboxMessageConfiguration)dataObject;
			if (mailboxMessageConfiguration.IsModified(MailboxMessageConfigurationSchema.SignatureHtml))
			{
				mailboxMessageConfiguration.SignatureHtml = TextConverterHelper.SanitizeHtml(mailboxMessageConfiguration.SignatureHtml);
				if (!mailboxMessageConfiguration.IsModified(MailboxMessageConfigurationSchema.SignatureText))
				{
					mailboxMessageConfiguration.SignatureText = TextConverterHelper.HtmlToText(mailboxMessageConfiguration.SignatureHtml, true);
				}
			}
			else if (mailboxMessageConfiguration.IsModified(MailboxMessageConfigurationSchema.SignatureText))
			{
				mailboxMessageConfiguration.SignatureHtml = TextConverterHelper.TextToHtml(mailboxMessageConfiguration.SignatureText);
			}
			if (SyncUtilities.IsDatacenterMode() && mailboxMessageConfiguration.IsModified(MailboxMessageConfigurationSchema.SendAddressDefault))
			{
				SendAsDefaultsManager sendAsDefaultsManager = new SendAsDefaultsManager();
				sendAsDefaultsManager.SaveSettingForOutlook(mailboxMessageConfiguration.SendAddressDefault, this.mailboxSession);
			}
			this.mailboxSession = null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is ExchangeDataException || exception is StorageTransientException || exception is TextConvertersException || base.IsKnownException(exception);
		}

		private MailboxSession mailboxSession;
	}
}
