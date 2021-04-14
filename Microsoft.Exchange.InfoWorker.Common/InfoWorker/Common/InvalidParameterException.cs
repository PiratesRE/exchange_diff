using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[Serializable]
	public class InvalidParameterException : LocalizedException
	{
		public InvalidParameterException(LocalizedString message) : base(message)
		{
		}

		public InvalidParameterException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
