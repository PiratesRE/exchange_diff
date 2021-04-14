using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotGetAggregatedAccountException : ServicePermanentException
	{
		public CannotGetAggregatedAccountException(Enum messageId) : base(ResponseCodeType.ErrorCannotGetAggregatedAccount, messageId)
		{
		}

		public CannotGetAggregatedAccountException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotGetAggregatedAccount, messageId, innerException)
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
