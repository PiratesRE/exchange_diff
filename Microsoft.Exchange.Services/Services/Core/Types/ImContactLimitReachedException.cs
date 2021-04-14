using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ImContactLimitReachedException : ServicePermanentException
	{
		public ImContactLimitReachedException() : base(ResponseCodeType.ErrorImContactLimitReached, CoreResources.ErrorImContactLimitReached(Global.UnifiedContactStoreConfiguration.MaxImContacts))
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
