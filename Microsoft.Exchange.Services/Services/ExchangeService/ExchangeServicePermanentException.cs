using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[Serializable]
	internal class ExchangeServicePermanentException : LocalizedException
	{
		public ExchangeServicePermanentException(LocalizedString message) : base(message)
		{
		}

		public ExchangeServicePermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public ExchangeServicePermanentException(LocalizedException innerException) : this(innerException.LocalizedString, innerException)
		{
		}
	}
}
