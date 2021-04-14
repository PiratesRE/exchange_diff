using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MoreThanOneAccessModeSpecifiedException : ServicePermanentException
	{
		public MoreThanOneAccessModeSpecifiedException() : base(CoreResources.IDs.ErrorMoreThanOneAccessModeSpecified)
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
