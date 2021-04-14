using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NonPrimarySmtpAddressException : ServicePermanentException
	{
		public NonPrimarySmtpAddressException(string smtpAddress) : base(CoreResources.IDs.ErrorNonPrimarySmtpAddress)
		{
			base.ConstantValues.Add("Primary", smtpAddress);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private const string PrimaryKey = "Primary";
	}
}
