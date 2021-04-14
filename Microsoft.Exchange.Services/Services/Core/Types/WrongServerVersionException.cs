using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class WrongServerVersionException : ServicePermanentException
	{
		public WrongServerVersionException(string smtpAddress) : base(ResponseCodeType.ErrorWrongServerVersion, (CoreResources.IDs)3533302998U)
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
