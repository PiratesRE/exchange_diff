using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedTypeForConversionException : ServicePermanentException
	{
		public UnsupportedTypeForConversionException() : base(CoreResources.IDs.ErrorUnsupportedTypeForConversion)
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
