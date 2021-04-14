using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxContainerGuidMismatchException : ServicePermanentException
	{
		public MailboxContainerGuidMismatchException(Enum messageId) : base(ResponseCodeType.ErrorMailboxContainerGuidMismatch, messageId)
		{
		}

		public MailboxContainerGuidMismatchException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorMailboxContainerGuidMismatch, messageId, innerException)
		{
		}

		public MailboxContainerGuidMismatchException(ResponseCodeType responseCodeType, LocalizedString message) : base(responseCodeType, message)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
