using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidSerializedAccessTokenException : ServicePermanentException
	{
		public InvalidSerializedAccessTokenException() : base((CoreResources.IDs)2485795088U)
		{
		}

		public InvalidSerializedAccessTokenException(Exception innerException) : base((CoreResources.IDs)2485795088U, innerException)
		{
		}

		public InvalidSerializedAccessTokenException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidSerializedAccessToken, messageId, innerException)
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
