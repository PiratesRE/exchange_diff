using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotRemoveAggregatedAccountException : ServicePermanentException
	{
		public CannotRemoveAggregatedAccountException(Enum messageId) : base(ResponseCodeType.ErrorCannotRemoveAggregatedAccount, messageId)
		{
		}

		public CannotRemoveAggregatedAccountException(ResponseCodeType responseCode, Enum messageId) : base(responseCode, messageId)
		{
		}

		public CannotRemoveAggregatedAccountException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotRemoveAggregatedAccount, messageId, innerException)
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
