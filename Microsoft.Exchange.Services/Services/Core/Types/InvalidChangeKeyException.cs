using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidChangeKeyException : ServicePermanentException
	{
		public InvalidChangeKeyException() : base(CoreResources.IDs.ErrorInvalidChangeKey)
		{
		}

		public InvalidChangeKeyException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidChangeKey, innerException)
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
