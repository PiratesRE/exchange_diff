using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidFractionalPagingParametersException : ServicePermanentException
	{
		public InvalidFractionalPagingParametersException() : base((CoreResources.IDs)2620420056U)
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
