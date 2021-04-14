using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class OutlookRuleBlobExistsException : ServicePermanentException
	{
		public OutlookRuleBlobExistsException() : base(ResponseCodeType.ErrorOutlookRuleBlobExists, CoreResources.IDs.RuleErrorOutlookRuleBlobExists)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP1;
			}
		}
	}
}
