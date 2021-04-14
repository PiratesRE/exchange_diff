using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedMimeConversionException : ServicePermanentException
	{
		public UnsupportedMimeConversionException() : base(CoreResources.IDs.ErrorUnsupportedMimeConversion)
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
