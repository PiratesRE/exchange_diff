using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[Serializable]
	internal class ExchangeServiceTransientException : TransientException
	{
		public ExchangeServiceTransientException(LocalizedString message) : base(message)
		{
		}

		public ExchangeServiceTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public ExchangeServiceTransientException(LocalizedException innerException) : this(innerException.LocalizedString, innerException)
		{
		}
	}
}
