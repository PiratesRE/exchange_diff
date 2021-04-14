using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class QueryFilterTooLongException : ServicePermanentException
	{
		public QueryFilterTooLongException() : base((CoreResources.IDs)2285125742U)
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
