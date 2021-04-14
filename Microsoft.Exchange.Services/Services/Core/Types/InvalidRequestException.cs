using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidRequestException : ServicePermanentException
	{
		public InvalidRequestException() : base((CoreResources.IDs)3784063568U)
		{
		}

		public InvalidRequestException(Exception innerException) : base((CoreResources.IDs)3784063568U, innerException)
		{
		}

		public InvalidRequestException(Enum messageId) : base(ResponseCodeType.ErrorInvalidRequest, messageId)
		{
		}

		public InvalidRequestException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidRequest, messageId, innerException)
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
