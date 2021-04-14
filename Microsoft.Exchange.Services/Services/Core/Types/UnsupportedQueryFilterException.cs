using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedQueryFilterException : ServicePermanentException
	{
		public UnsupportedQueryFilterException(Enum messageId) : base(messageId)
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
