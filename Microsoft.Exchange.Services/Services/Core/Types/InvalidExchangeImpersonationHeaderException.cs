using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidExchangeImpersonationHeaderException : ServicePermanentException
	{
		public InvalidExchangeImpersonationHeaderException() : base(CoreResources.IDs.ErrorInvalidExchangeImpersonationHeaderData)
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
