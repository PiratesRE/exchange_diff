using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(MeetingCancellationMessageType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[KnownType(typeof(MeetingResponseMessageType))]
	[KnownType(typeof(MeetingRequestMessageType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "MeetingMessage")]
	[Serializable]
	public class MeetingMessageType : MessageType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public ItemId AssociatedCalendarItemId
		{
			get
			{
				return base.GetValueOrDefault<ItemId>(MeetingMessageSchema.AssociatedCalendarItemId);
			}
			set
			{
				this[MeetingMessageSchema.AssociatedCalendarItemId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public bool IsDelegated
		{
			get
			{
				return base.GetValueOrDefault<bool>(MeetingMessageSchema.IsDelegated);
			}
			set
			{
				this[MeetingMessageSchema.IsDelegated] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsDelegatedSpecified
		{
			get
			{
				return base.IsSet(MeetingMessageSchema.IsDelegated);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public bool IsOutOfDate
		{
			get
			{
				return base.GetValueOrDefault<bool>(MeetingMessageSchema.IsOutOfDate);
			}
			set
			{
				this[MeetingMessageSchema.IsOutOfDate] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsOutOfDateSpecified
		{
			get
			{
				return base.IsSet(MeetingMessageSchema.IsOutOfDate);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public bool HasBeenProcessed
		{
			get
			{
				return base.GetValueOrDefault<bool>(MeetingMessageSchema.HasBeenProcessed);
			}
			set
			{
				this[MeetingMessageSchema.HasBeenProcessed] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool HasBeenProcessedSpecified
		{
			get
			{
				return base.IsSet(MeetingMessageSchema.HasBeenProcessed);
			}
			set
			{
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public ResponseTypeType ResponseType
		{
			get
			{
				if (!this.ResponseTypeSpecified)
				{
					return ResponseTypeType.Unknown;
				}
				return EnumUtilities.Parse<ResponseTypeType>(this.ResponseTypeString);
			}
			set
			{
				this.ResponseTypeString = EnumUtilities.ToString<ResponseTypeType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ResponseType", EmitDefaultValue = false, Order = 5)]
		public string ResponseTypeString
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingMessageSchema.ResponseType);
			}
			set
			{
				this[MeetingMessageSchema.ResponseType] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ResponseTypeSpecified
		{
			get
			{
				return base.IsSet(MeetingMessageSchema.ResponseType);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string UID
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingMessageSchema.ICalendarUid);
			}
			set
			{
				this[MeetingMessageSchema.ICalendarUid] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string RecurrenceId
		{
			get
			{
				return base.GetValueOrDefault<string>(CalendarItemSchema.ICalendarRecurrenceId);
			}
			set
			{
				this[CalendarItemSchema.ICalendarRecurrenceId] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool RecurrenceIdSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.ICalendarRecurrenceId);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		[DateTimeString]
		public string DateTimeStamp
		{
			get
			{
				return base.GetValueOrDefault<string>(CalendarItemSchema.ICalendarDateTimeStamp);
			}
			set
			{
				this[CalendarItemSchema.ICalendarDateTimeStamp] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DateTimeStampSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.ICalendarDateTimeStamp);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public bool? IsOrganizer
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsOrganizer);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsOrganizerSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsOrganizer);
			}
			set
			{
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.MeetingMessage;
			}
		}
	}
}
