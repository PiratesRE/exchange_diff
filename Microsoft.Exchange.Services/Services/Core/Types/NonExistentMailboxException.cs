using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NonExistentMailboxException : ServicePermanentException
	{
		public NonExistentMailboxException(Enum messageId, string mailbox) : base(ResponseCodeType.ErrorNonExistentMailbox, messageId)
		{
			CoreResources.IDs ds = (CoreResources.IDs)messageId;
			if (ds == (CoreResources.IDs)3279543955U || ds == (CoreResources.IDs)4074099229U)
			{
				base.ConstantValues.Add("MailboxGuid", mailbox);
				return;
			}
			if (ds != (CoreResources.IDs)4088802584U)
			{
				return;
			}
			base.ConstantValues.Add("SmtpAddress", mailbox);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private const string SmtpAddressKey = "SmtpAddress";

		private const string MailboxGuid = "MailboxGuid";
	}
}
