using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidCompleteDateException : ServicePermanentException
	{
		public InvalidCompleteDateException(Enum messageId) : base(ResponseCodeType.ErrorInvalidCompleteDate, messageId)
		{
		}

		public InvalidCompleteDateException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidCompleteDate, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
