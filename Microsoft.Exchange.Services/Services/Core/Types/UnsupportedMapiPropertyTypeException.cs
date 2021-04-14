using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnsupportedMapiPropertyTypeException : ServicePermanentException
	{
		public UnsupportedMapiPropertyTypeException() : base(CoreResources.IDs.ErrorUnsupportedMapiPropertyType)
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
