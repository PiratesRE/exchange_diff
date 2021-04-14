using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TooManyKeywordsException : ServicePermanentException
	{
		public TooManyKeywordsException(Exception innerException) : base((CoreResources.IDs)3032287327U, innerException)
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
