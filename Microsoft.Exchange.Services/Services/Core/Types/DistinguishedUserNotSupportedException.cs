using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DistinguishedUserNotSupportedException : ServicePermanentException
	{
		public DistinguishedUserNotSupportedException() : base((CoreResources.IDs)4170132598U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
