using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedCultureException : ServicePermanentException
	{
		public UnsupportedCultureException() : base(CoreResources.IDs.ErrorUnsupportedCulture)
		{
		}

		public UnsupportedCultureException(Exception innerException) : base(CoreResources.IDs.ErrorUnsupportedCulture, innerException)
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
