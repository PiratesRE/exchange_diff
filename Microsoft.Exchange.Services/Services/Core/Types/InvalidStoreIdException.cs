using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal class InvalidStoreIdException : ServicePermanentException
	{
		public InvalidStoreIdException(Enum messageId) : base(messageId)
		{
		}

		public InvalidStoreIdException(Enum messageId, Exception innerException) : base(messageId, innerException)
		{
		}

		public InvalidStoreIdException(ResponseCodeType responseCode, Enum messageId) : base(responseCode, messageId)
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
