using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AccessModeSpecifiedException : ServicePermanentException
	{
		public AccessModeSpecifiedException() : base((CoreResources.IDs)3314483401U)
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
