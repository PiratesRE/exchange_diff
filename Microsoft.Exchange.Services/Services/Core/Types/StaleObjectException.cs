using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class StaleObjectException : ServicePermanentException
	{
		public StaleObjectException() : base((CoreResources.IDs)3943872330U)
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
