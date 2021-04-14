using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NonProvisionedException : ServicePermanentException
	{
		public NonProvisionedException(bool mowa) : base(ResponseCodeType.ErrorMailboxStoreUnavailable, mowa ? CoreResources.IDs.MowaNotProvisioned : CoreResources.IDs.DowaNotProvisioned)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
