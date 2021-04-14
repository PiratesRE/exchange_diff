using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotSetAggregatedAccountException : ServicePermanentException
	{
		public CannotSetAggregatedAccountException(Enum messageId) : base(ResponseCodeType.ErrorCannotSetAggregatedAccount, messageId)
		{
		}

		public CannotSetAggregatedAccountException(ResponseCodeType responseCodeType, Enum messageId) : base(responseCodeType, messageId)
		{
		}

		public CannotSetAggregatedAccountException(ResponseCodeType responseCodeType, LocalizedString message) : base(responseCodeType, message)
		{
		}

		public CannotSetAggregatedAccountException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotSetAggregatedAccount, messageId, innerException)
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
