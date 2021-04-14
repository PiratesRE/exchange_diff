using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[Serializable]
	internal class ExchangeServiceResponseException : ExchangeServicePermanentException
	{
		public ExchangeServiceResponseException(LocalizedString message) : base(message)
		{
		}
	}
}
