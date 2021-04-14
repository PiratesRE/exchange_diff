using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	internal class RequestParsingException : LocalizedException
	{
		public RequestParsingException(string errorMessage) : this(errorMessage, errorMessage)
		{
		}

		public RequestParsingException(string errorMessage, string logMessage) : base(new LocalizedString(errorMessage))
		{
			this.LogMessage = logMessage;
		}

		public string LogMessage { get; private set; }
	}
}
