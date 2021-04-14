using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailTipsDisabledException : ServicePermanentException
	{
		public MailTipsDisabledException() : base(ResponseCodeType.ErrorMailTipsDisabled, CoreResources.IDs.ErrorMailTipsDisabled)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
