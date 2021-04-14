using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ImGroupLimitReachedException : ServicePermanentException
	{
		public ImGroupLimitReachedException() : base(ResponseCodeType.ErrorImGroupLimitReached, CoreResources.ErrorImGroupLimitReached(Global.UnifiedContactStoreConfiguration.MaxImGroups))
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
