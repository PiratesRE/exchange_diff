using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidIndexedPagingParametersException : ServicePermanentException
	{
		public InvalidIndexedPagingParametersException() : base(CoreResources.IDs.ErrorInvalidIndexedPagingParameters)
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
