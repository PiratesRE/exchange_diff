using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotAddAggregatedAccountException : ServicePermanentException
	{
		public CannotAddAggregatedAccountException(Enum messageId) : base(ResponseCodeType.ErrorCannotAddAggregatedAccount, messageId)
		{
		}

		public CannotAddAggregatedAccountException(ResponseCodeType responseCodeType, Enum messageId) : base(responseCodeType, messageId)
		{
		}

		public CannotAddAggregatedAccountException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotAddAggregatedAccount, messageId, innerException)
		{
		}

		public CannotAddAggregatedAccountException(ResponseCodeType responseCodeType, LocalizedString message) : base(responseCodeType, message)
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
