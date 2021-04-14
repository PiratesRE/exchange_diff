using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.TimeZoneOffsets
{
	public class TimeZoneOffset
	{
		public string TimeZoneId { get; set; }

		public TimeZoneRange[] OffsetRanges { get; set; }
	}
}
