using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class IWPermanentException : LocalizedException
	{
		public IWPermanentException(LocalizedString message) : base(message)
		{
		}

		public IWPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
