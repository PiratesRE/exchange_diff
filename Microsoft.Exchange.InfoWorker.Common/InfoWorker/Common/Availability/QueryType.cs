using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[Flags]
	public enum QueryType
	{
		FreeBusy = 1,
		MeetingSuggestions = 2
	}
}
