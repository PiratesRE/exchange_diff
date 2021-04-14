using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateExceptionNotDelegate : ServicePermanentException
	{
		public DelegateExceptionNotDelegate() : base((CoreResources.IDs)2410622290U)
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
