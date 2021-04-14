using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidLogonTypeException : ServicePermanentException
	{
		public InvalidLogonTypeException() : base((CoreResources.IDs)3522975510U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP1;
			}
		}
	}
}
