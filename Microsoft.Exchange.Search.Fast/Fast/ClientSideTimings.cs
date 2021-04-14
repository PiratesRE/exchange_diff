using System;

namespace Microsoft.Exchange.Search.Fast
{
	internal class ClientSideTimings
	{
		public TimeSpan TimeInGetConnection { get; set; }

		public TimeSpan TimeInPropertyBagLoad { get; set; }

		public TimeSpan TimeInMessageItemConversion { get; set; }

		public TimeSpan TimeDeterminingAgeOfItem { get; set; }

		public TimeSpan TimeInMimeConversion { get; set; }

		public TimeSpan TimeInShouldAnnotateMessage { get; set; }
	}
}
