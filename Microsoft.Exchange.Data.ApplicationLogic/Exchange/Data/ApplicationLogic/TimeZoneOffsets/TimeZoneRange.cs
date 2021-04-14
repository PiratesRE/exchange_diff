using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.TimeZoneOffsets
{
	public class TimeZoneRange
	{
		public ExDateTime UtcTime { get; set; }

		public int Offset { get; set; }
	}
}
