using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal class InvalidIdFormatVersionException : ServicePermanentException
	{
		public InvalidIdFormatVersionException() : base(CoreResources.IDs.ErrorInvalidIdMalformedEwsLegacyIdFormat)
		{
			this.exchangeVersion = ExchangeVersion.Exchange2007SP1;
		}

		public InvalidIdFormatVersionException(Enum messageId) : base(ResponseCodeType.ErrorInvalidIdMalformed, messageId)
		{
			this.exchangeVersion = ExchangeVersion.Exchange2007;
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return this.exchangeVersion;
			}
		}

		private ExchangeVersion exchangeVersion;
	}
}
