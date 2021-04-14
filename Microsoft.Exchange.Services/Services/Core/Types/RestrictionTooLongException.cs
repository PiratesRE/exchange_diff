using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class RestrictionTooLongException : ServicePermanentException
	{
		public RestrictionTooLongException() : base((CoreResources.IDs)3143473274U)
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
