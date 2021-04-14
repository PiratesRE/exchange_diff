using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchLoggingEvent : EventArgs
	{
		internal SearchLoggingEvent(LocalizedString loggingMessage)
		{
			this.loggingMessage = loggingMessage;
		}

		internal LocalizedString LoggingMessage
		{
			get
			{
				return this.loggingMessage;
			}
			set
			{
				this.loggingMessage = value;
			}
		}

		private LocalizedString loggingMessage;
	}
}
