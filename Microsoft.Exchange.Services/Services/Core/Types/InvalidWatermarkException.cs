using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidWatermarkException : ServicePermanentException
	{
		public InvalidWatermarkException() : base((CoreResources.IDs)3312780993U)
		{
		}

		public InvalidWatermarkException(Exception innerException) : base((CoreResources.IDs)3312780993U, innerException)
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
