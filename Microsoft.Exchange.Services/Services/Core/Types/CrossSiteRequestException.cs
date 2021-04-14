using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CrossSiteRequestException : ServicePermanentException
	{
		public CrossSiteRequestException(string smtpAddress) : base(ResponseCodeType.ErrorCrossSiteRequest, CoreResources.IDs.ErrorCrossSiteRequest)
		{
			base.ConstantValues.Add("AutodiscoverSmtpAddress", smtpAddress);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}

		private const string AutodiscoverSmtpAddress = "AutodiscoverSmtpAddress";
	}
}
