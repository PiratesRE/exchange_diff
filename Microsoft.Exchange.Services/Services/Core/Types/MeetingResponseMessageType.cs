using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MeetingResponseMessageType : MeetingMessageType
	{
		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.MeetingResponse;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		[DateTimeString]
		public string Start
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingResponseSchema.Start);
			}
			set
			{
				this[MeetingResponseSchema.Start] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string End
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingResponseSchema.End);
			}
			set
			{
				this[MeetingResponseSchema.End] = value;
			}
		}

		[IgnoreDataMember]
		public string Location
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MeetingResponseSchema.Location);
			}
			set
			{
				base.PropertyBag[MeetingResponseSchema.Location] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public RecurrenceType Recurrence
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RecurrenceType>(CalendarItemSchema.OrganizerSpecific.Recurrence);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence] = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement("CalendarItemType")]
		public CalendarItemTypeType CalendarItemType
		{
			get
			{
				if (!this.CalendarItemTypeSpecified)
				{
					return CalendarItemTypeType.Single;
				}
				return EnumUtilities.Parse<CalendarItemTypeType>(this.CalendarItemTypeString);
			}
			set
			{
				this.CalendarItemTypeString = EnumUtilities.ToString<CalendarItemTypeType>(value);
			}
		}

		[DataMember(Name = "CalendarItemType", EmitDefaultValue = false, Order = 4)]
		[XmlIgnore]
		public string CalendarItemTypeString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MeetingResponseSchema.CalendarItemType);
			}
			set
			{
				base.PropertyBag[MeetingResponseSchema.CalendarItemType] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool CalendarItemTypeSpecified
		{
			get
			{
				return base.IsSet(MeetingResponseSchema.CalendarItemType);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		[DateTimeString]
		public string ProposedStart
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingResponseSchema.ProposedStart);
			}
			set
			{
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string ProposedEnd
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingResponseSchema.ProposedEnd);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsNewTimeProposal
		{
			get
			{
				return !string.IsNullOrEmpty(this.ProposedStart) || !string.IsNullOrEmpty(this.ProposedEnd);
			}
			set
			{
			}
		}

		[DataMember(Name = "Location", EmitDefaultValue = false, Order = 7)]
		public EnhancedLocationType EnhancedLocation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EnhancedLocationType>(MeetingResponseSchema.EnhancedLocation);
			}
			set
			{
				base.PropertyBag[MeetingResponseSchema.EnhancedLocation] = value;
			}
		}
	}
}
