using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxAutoReplyConfiguration", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxAutoReplyConfiguration : SetXsoObjectWithIdentityTaskBase<MailboxAutoReplyConfiguration>
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
				return Strings.ConfirmationMessageMailboxAutoReplyConfiguration(this.Identity.ToString());
			}
		}

		protected override void StampChangesOnXsoObject(IConfigurable dataObject)
		{
			base.StampChangesOnXsoObject(dataObject);
			MailboxAutoReplyConfiguration mailboxAutoReplyConfiguration = (MailboxAutoReplyConfiguration)dataObject;
			if (!string.IsNullOrEmpty(mailboxAutoReplyConfiguration.InternalMessage) && mailboxAutoReplyConfiguration.IsChanged(MailboxAutoReplyConfigurationSchema.InternalMessage))
			{
				mailboxAutoReplyConfiguration.InternalMessage = TextConverterHelper.SanitizeHtml(mailboxAutoReplyConfiguration.InternalMessage);
			}
			if (!string.IsNullOrEmpty(mailboxAutoReplyConfiguration.ExternalMessage) && mailboxAutoReplyConfiguration.IsChanged(MailboxAutoReplyConfigurationSchema.ExternalMessage))
			{
				mailboxAutoReplyConfiguration.ExternalMessage = TextConverterHelper.SanitizeHtml(mailboxAutoReplyConfiguration.ExternalMessage);
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new MailboxAutoReplyConfigurationDataProvider(principal, "Set-MailboxAutoReplyConfiguration");
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is ExchangeDataException || exception is StorageTransientException || exception is TextConvertersException || exception is InvalidScheduledOofDuration || base.IsKnownException(exception);
		}
	}
}
