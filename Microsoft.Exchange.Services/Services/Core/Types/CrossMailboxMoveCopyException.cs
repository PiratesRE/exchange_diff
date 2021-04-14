using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CrossMailboxMoveCopyException : ServicePermanentException
	{
		public CrossMailboxMoveCopyException() : base((CoreResources.IDs)2832845860U)
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
