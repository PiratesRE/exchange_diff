using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidSmtpAddressException : ServicePermanentException
	{
		public InvalidSmtpAddressException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidSmtpAddress, innerException)
		{
		}

		public InvalidSmtpAddressException() : base(CoreResources.IDs.ErrorInvalidSmtpAddress)
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
