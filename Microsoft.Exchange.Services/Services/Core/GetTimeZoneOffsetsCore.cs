using System;
using Microsoft.Exchange.Data.ApplicationLogic.TimeZoneOffsets;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class GetTimeZoneOffsetsCore
	{
		public static TimeZoneOffsetsType[] GetTheTimeZoneOffsets(ExDateTime startTime, ExDateTime endTime, string timeZoneId)
		{
			if (endTime < startTime)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2498507918U);
			}
			if (startTime.AddYears(100) < endTime)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2538042329U);
			}
			TimeZoneOffset[] theTimeZoneOffsets = TimeZoneOffsets.GetTheTimeZoneOffsets(startTime, endTime, timeZoneId, WsPerformanceCounters.TotalTimeZoneOffsetsTablesBuilt);
			if (timeZoneId != null && theTimeZoneOffsets == null)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)4066404319U);
			}
			TimeZoneOffsetsType[] array = new TimeZoneOffsetsType[theTimeZoneOffsets.Length];
			for (int i = 0; i < theTimeZoneOffsets.Length; i++)
			{
				array[i] = new TimeZoneOffsetsType();
				array[i].TimeZoneId = theTimeZoneOffsets[i].TimeZoneId;
				array[i].OffsetRanges = new TimeZoneRangeType[theTimeZoneOffsets[i].OffsetRanges.Length];
				for (int j = 0; j < theTimeZoneOffsets[i].OffsetRanges.Length; j++)
				{
					array[i].OffsetRanges[j] = new TimeZoneRangeType();
					array[i].OffsetRanges[j].UtcTime = theTimeZoneOffsets[i].OffsetRanges[j].UtcTime.ToISOString();
					array[i].OffsetRanges[j].Offset = theTimeZoneOffsets[i].OffsetRanges[j].Offset;
				}
			}
			return array;
		}
	}
}
