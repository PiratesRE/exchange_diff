using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ChangeKeyRequiredException : ServicePermanentException
	{
		public ChangeKeyRequiredException() : base((CoreResources.IDs)3941855338U)
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
