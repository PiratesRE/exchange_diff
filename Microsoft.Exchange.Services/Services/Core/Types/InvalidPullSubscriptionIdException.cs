using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidPullSubscriptionIdException : ServicePermanentException
	{
		public InvalidPullSubscriptionIdException() : base(CoreResources.IDs.ErrorInvalidPullSubscriptionId)
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
