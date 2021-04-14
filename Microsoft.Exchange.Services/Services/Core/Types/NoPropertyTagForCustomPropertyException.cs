using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NoPropertyTagForCustomPropertyException : ServicePermanentException
	{
		public NoPropertyTagForCustomPropertyException() : base((CoreResources.IDs)3969305989U)
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
