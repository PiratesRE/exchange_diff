using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal struct MeetingMessageTypeConverter : IConverter<string, MeetingMessageType>
	{
		public IConverter<string, MeetingMessageType> StorageToEntitiesConverter
		{
			get
			{
				return this;
			}
		}

		MeetingMessageType IConverter<string, MeetingMessageType>.Convert(string itemClass)
		{
			if (ObjectClass.IsMeetingRequest(itemClass))
			{
				return MeetingMessageType.SingleInstanceRequest;
			}
			if (ObjectClass.IsMeetingRequestSeries(itemClass))
			{
				return MeetingMessageType.SeriesRequest;
			}
			if (ObjectClass.IsMeetingCancellation(itemClass))
			{
				return MeetingMessageType.SingleInstanceCancel;
			}
			if (ObjectClass.IsMeetingCancellationSeries(itemClass))
			{
				return MeetingMessageType.SeriesCancel;
			}
			if (ObjectClass.IsMeetingResponse(itemClass))
			{
				return MeetingMessageType.SingleInstanceResponse;
			}
			if (ObjectClass.IsMeetingResponseSeries(itemClass))
			{
				return MeetingMessageType.SeriesResponse;
			}
			if (ObjectClass.IsMeetingForwardNotification(itemClass))
			{
				return MeetingMessageType.SingleInstanceForwardNotification;
			}
			throw new ArgumentOutOfRangeException("itemClass");
		}
	}
}
