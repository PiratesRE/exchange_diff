using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class IndividualAttendeeConflictData : AttendeeConflictData
	{
		internal static IndividualAttendeeConflictData Create(AttendeeData inputAttendee, BusyType busyType)
		{
			return new IndividualAttendeeConflictData
			{
				attendee = inputAttendee,
				busyType = busyType
			};
		}

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

		[XmlIgnore]
		public AttendeeData Attendee
		{
			get
			{
				return this.attendee;
			}
		}

		[XmlIgnore]
		public bool AttendeeHasConflict
		{
			get
			{
				switch (this.busyType)
				{
				case BusyType.Tentative:
				case BusyType.Busy:
				case BusyType.OOF:
				case BusyType.NoData:
					return true;
				}
				return false;
			}
		}

		[XmlIgnore]
		public bool IsMissingFreeBusyData
		{
			get
			{
				BusyType busyType = this.busyType;
				return busyType == BusyType.NoData;
			}
		}

		private AttendeeData attendee;

		private BusyType busyType;
	}
}
