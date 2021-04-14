using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class TokenSerializationDeniedException : ServicePermanentException
	{
		public TokenSerializationDeniedException() : base((CoreResources.IDs)3279473776U)
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
