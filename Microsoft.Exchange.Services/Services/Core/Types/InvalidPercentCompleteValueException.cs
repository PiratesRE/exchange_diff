using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidPercentCompleteValueException : ServicePermanentException
	{
		public InvalidPercentCompleteValueException() : base((CoreResources.IDs)3035123300U)
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
