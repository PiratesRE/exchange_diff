using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotAddAggregatedAccountToListException : ServicePermanentException
	{
		public CannotAddAggregatedAccountToListException(Enum messageId) : base(ResponseCodeType.ErrorCannotAddAggregatedAccountToList, messageId)
		{
		}

		public CannotAddAggregatedAccountToListException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotAddAggregatedAccountToList, messageId, innerException)
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
