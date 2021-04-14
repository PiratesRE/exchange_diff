using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public enum MeetingMessageType
	{
		Unknown,
		SingleInstanceRequest,
		SingleInstanceResponse,
		SingleInstanceCancel,
		SingleInstanceForwardNotification,
		SeriesRequest,
		SeriesResponse,
		SeriesCancel,
		SeriesForwardNotification
	}
}
