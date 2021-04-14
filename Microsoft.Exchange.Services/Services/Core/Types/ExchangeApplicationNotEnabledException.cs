using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ExchangeApplicationNotEnabledException : ServicePermanentException
	{
		public ExchangeApplicationNotEnabledException(Enum messageId) : base(ResponseCodeType.ErrorExchangeApplicationNotEnabled, messageId)
		{
		}

		public ExchangeApplicationNotEnabledException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorExchangeApplicationNotEnabled, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
