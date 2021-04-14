using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidPagingMaxRowsException : ServicePermanentException
	{
		public InvalidPagingMaxRowsException() : base((CoreResources.IDs)2467205866U)
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
