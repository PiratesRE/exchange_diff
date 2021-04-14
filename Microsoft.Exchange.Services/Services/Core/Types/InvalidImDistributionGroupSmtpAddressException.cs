using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidImDistributionGroupSmtpAddressException : ServicePermanentException
	{
		public InvalidImDistributionGroupSmtpAddressException() : base(CoreResources.IDs.ErrorInvalidImDistributionGroupSmtpAddress)
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
