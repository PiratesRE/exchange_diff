using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidClientAccessTokenRequestException : ServicePermanentException
	{
		public InvalidClientAccessTokenRequestException() : base((CoreResources.IDs)2958727324U)
		{
		}

		public InvalidClientAccessTokenRequestException(Enum messageId) : base(ResponseCodeType.ErrorInvalidClientAccessTokenRequest, messageId)
		{
		}

		public InvalidClientAccessTokenRequestException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidClientAccessTokenRequest, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
