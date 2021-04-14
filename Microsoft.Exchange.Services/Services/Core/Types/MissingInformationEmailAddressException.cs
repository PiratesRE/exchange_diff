using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MissingInformationEmailAddressException : ServicePermanentException
	{
		public MissingInformationEmailAddressException() : base(CoreResources.IDs.ErrorMissingInformationEmailAddress)
		{
		}

		public MissingInformationEmailAddressException(Enum messageId) : base(ResponseCodeType.ErrorMissingInformationEmailAddress, messageId)
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
