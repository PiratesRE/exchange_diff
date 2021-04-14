using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MeetingCancellationMessageType : MeetingMessageType
	{
		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.MeetingCancellation;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Start
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingCancellationSchema.Start);
			}
			set
			{
				this[MeetingCancellationSchema.Start] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[DateTimeString]
		public string End
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingCancellationSchema.End);
			}
			set
			{
				this[MeetingCancellationSchema.End] = value;
			}
		}

		[IgnoreDataMember]
		public string Location
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MeetingCancellationSchema.Location);
			}
			set
			{
				base.PropertyBag[MeetingCancellationSchema.Location] = value;
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

		[XmlIgnore]
		[DataMember(Name = "CalendarItemType", EmitDefaultValue = false, Order = 4)]
		public string CalendarItemTypeString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MeetingCancellationSchema.CalendarItemType);
			}
			set
			{
				base.PropertyBag[MeetingCancellationSchema.CalendarItemType] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool CalendarItemTypeSpecified
		{
			get
			{
				return base.IsSet(MeetingCancellationSchema.CalendarItemType);
			}
			set
			{
			}
		}

		[DataMember(Name = "Location", EmitDefaultValue = false, Order = 5)]
		public EnhancedLocationType EnhancedLocation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EnhancedLocationType>(MeetingCancellationSchema.EnhancedLocation);
			}
			set
			{
				base.PropertyBag[MeetingCancellationSchema.EnhancedLocation] = value;
			}
		}
	}
}
