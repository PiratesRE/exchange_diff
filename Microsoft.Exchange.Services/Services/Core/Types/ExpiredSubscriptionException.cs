using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ExpiredSubscriptionException : ServicePermanentException
	{
		public ExpiredSubscriptionException() : base((CoreResources.IDs)3329761676U)
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
