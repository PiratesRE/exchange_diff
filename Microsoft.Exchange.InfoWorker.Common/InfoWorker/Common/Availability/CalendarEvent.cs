using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarEvent
	{
		[XmlElement]
		[IgnoreDataMember]
		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public DateTime EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		[DataMember(Name = "StartTime", IsRequired = true)]
		[XmlIgnore]
		public string StartTimeString
		{
			get
			{
				return this.StartTime.ToIso8061();
			}
			set
			{
				this.StartTime = DateTime.Parse(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "EndTime", IsRequired = true)]
		public string EndTimeString
		{
			get
			{
				return this.EndTime.ToIso8061();
			}
			set
			{
				this.EndTime = DateTime.Parse(value);
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public BusyType BusyType
		{
			get
			{
				return this.busyType;
			}
			set
			{
				this.busyType = value;
			}
		}

		[DataMember(Name = "BusyType")]
		[XmlIgnore]
		public string BusyTypeString
		{
			get
			{
				return EnumUtil.ToString<BusyType>(this.BusyType);
			}
			set
			{
				this.BusyType = EnumUtil.Parse<BusyType>(value);
			}
		}

		[DataMember]
		[XmlElement(IsNullable = false)]
		public CalendarEventDetails CalendarEventDetails
		{
			get
			{
				return this.calendarEventDetails;
			}
			set
			{
				this.calendarEventDetails = value;
			}
		}

		public override string ToString()
		{
			return string.Format("<StartTime = {0}, EndTime = {1}, BusyType = {2}, Subject = {3}>", new object[]
			{
				this.startTime,
				this.EndTime,
				this.busyType,
				(this.calendarEventDetails != null) ? this.calendarEventDetails.Subject : "<null>"
			});
		}

		[XmlIgnore]
		internal byte[] GlobalObjectId
		{
			get
			{
				return this.globalObjectId;
			}
		}

		[XmlIgnore]
		internal FreeBusyViewType FreeBusyViewType
		{
			get
			{
				return this.viewType;
			}
		}

		internal static CalendarEvent CreateFromQueryData(EmailAddress mailbox, object[] properties, FreeBusyViewType allowedView, bool isCallerMailboxOwner, ExchangeVersionType exchangeVersion)
		{
			CalendarEvent calendarEvent = new CalendarEvent();
			calendarEvent.viewType = allowedView;
			calendarEvent.globalObjectId = CalendarEvent.GetPropertyValue<byte[]>(properties, QueryPropertyIndexes.GlobalObjectId);
			calendarEvent.StartTime = DateTime.SpecifyKind((DateTime)CalendarEvent.GetPropertyValue<ExDateTime>(properties, QueryPropertyIndexes.StartTime, ExDateTime.MinValue), DateTimeKind.Unspecified);
			calendarEvent.EndTime = DateTime.SpecifyKind((DateTime)CalendarEvent.GetPropertyValue<ExDateTime>(properties, QueryPropertyIndexes.EndTime, ExDateTime.MinValue), DateTimeKind.Unspecified);
			BusyType busyType = CalendarEvent.GetPropertyValue<BusyType>(properties, QueryPropertyIndexes.BusyStatus, BusyType.Busy);
			if (busyType < BusyType.Free || busyType > BusyType.NoData)
			{
				CalendarEvent.CalendarViewTracer.TraceError((long)calendarEvent.GetHashCode(), "{0}: Calendar event with start time {1} and end time {2} in mailbox {3} has invalid busy type: {4}. This is being returned as BusyType.Tentative", new object[]
				{
					TraceContext.Get(),
					calendarEvent.StartTime,
					calendarEvent.EndTime,
					mailbox,
					busyType
				});
				calendarEvent.BusyType = BusyType.Tentative;
			}
			else
			{
				if (exchangeVersion < ExchangeVersionType.Exchange2012 && busyType == BusyType.WorkingElsewhere)
				{
					busyType = BusyType.Free;
				}
				calendarEvent.BusyType = busyType;
			}
			Sensitivity propertyValue = CalendarEvent.GetPropertyValue<Sensitivity>(properties, QueryPropertyIndexes.Sensitivity, Sensitivity.Normal);
			if (propertyValue < Sensitivity.Normal || propertyValue > Sensitivity.CompanyConfidential)
			{
				CalendarEvent.CalendarViewTracer.TraceError((long)calendarEvent.GetHashCode(), "{0}: Calendar event with start time {1} and end time {2} in mailbox {3} has invalid sensitivity value: {4}.", new object[]
				{
					TraceContext.Get(),
					calendarEvent.StartTime,
					calendarEvent.EndTime,
					mailbox,
					propertyValue
				});
			}
			VersionedId propertyValue2 = CalendarEvent.GetPropertyValue<VersionedId>(properties, QueryPropertyIndexes.EntryId);
			ByteArray byteArray = new ByteArray(propertyValue2.ObjectId.ProviderLevelItemId);
			if (allowedView != FreeBusyViewType.Detailed && allowedView != FreeBusyViewType.DetailedMerged)
			{
				return calendarEvent;
			}
			calendarEvent.CalendarEventDetails = new CalendarEventDetails();
			CalendarEventDetails calendarEventDetails = calendarEvent.CalendarEventDetails;
			calendarEventDetails.IsPrivate = (propertyValue != Sensitivity.Normal);
			if (calendarEventDetails.IsPrivate && !isCallerMailboxOwner)
			{
				CalendarEvent.CalendarViewTracer.TraceError((long)calendarEvent.GetHashCode(), "{0}: Calendar event with start time {1} and end time {2} in mailbox {3} is a private item. Detail data will not be included.", new object[]
				{
					TraceContext.Get(),
					calendarEvent.StartTime,
					calendarEvent.EndTime,
					mailbox
				});
				return calendarEvent;
			}
			calendarEventDetails.ID = byteArray.ToString();
			calendarEventDetails.Subject = CalendarEvent.GetPropertyValue<string>(properties, QueryPropertyIndexes.Subject);
			calendarEventDetails.Location = CalendarEvent.GetPropertyValue<string>(properties, QueryPropertyIndexes.Location);
			calendarEventDetails.IsReminderSet = CalendarEvent.GetPropertyValue<bool>(properties, QueryPropertyIndexes.IsReminderSet, false);
			AppointmentStateFlags propertyValue3 = CalendarEvent.GetPropertyValue<AppointmentStateFlags>(properties, QueryPropertyIndexes.AppointmentState, AppointmentStateFlags.None);
			calendarEventDetails.IsMeeting = ((propertyValue3 & AppointmentStateFlags.Meeting) > AppointmentStateFlags.None);
			CalendarItemType propertyValue4 = CalendarEvent.GetPropertyValue<CalendarItemType>(properties, QueryPropertyIndexes.CalendarItemType, CalendarItemType.Single);
			if (propertyValue4 == CalendarItemType.Occurrence)
			{
				calendarEventDetails.IsRecurring = true;
			}
			if (propertyValue4 == CalendarItemType.Exception)
			{
				calendarEventDetails.IsException = true;
				calendarEventDetails.IsRecurring = true;
			}
			return calendarEvent;
		}

		private static T GetPropertyValue<T>(object[] properties, QueryPropertyIndexes index, T defaultValue)
		{
			object obj = properties[(int)index];
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private static T GetPropertyValue<T>(object[] properties, QueryPropertyIndexes index) where T : class
		{
			object obj = properties[(int)index];
			if (obj == null || obj is PropertyError)
			{
				return default(T);
			}
			return (T)((object)obj);
		}

		private FreeBusyViewType viewType = FreeBusyViewType.FreeBusy;

		private DateTime startTime = DateTime.MinValue;

		private DateTime endTime = DateTime.MinValue;

		private BusyType busyType = BusyType.NoData;

		private CalendarEventDetails calendarEventDetails;

		private byte[] globalObjectId;

		private static readonly Trace CalendarViewTracer = ExTraceGlobals.CalendarViewTracer;
	}
}
