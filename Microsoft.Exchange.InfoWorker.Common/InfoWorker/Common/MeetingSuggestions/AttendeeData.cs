using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	public class AttendeeData
	{
		public AttendeeData(MeetingAttendeeType attendeeType, string identity, bool excludeConflict, ExDateTime freeBusyStart, ExDateTime freeBusyEnd, string mergedFreeBusy, ExchangeVersionType requestSchemaVersion, AttendeeWorkHours workingHours)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (workingHours == null)
			{
				throw new ArgumentNullException("workingHours");
			}
			if (freeBusyStart.TimeZone != freeBusyEnd.TimeZone)
			{
				throw new ArgumentException("freeBusyStart.TimeZone != freeBusyEnd.TimeZone");
			}
			this.attendeeType = attendeeType;
			this.identity = identity;
			this.excludeConflict = excludeConflict;
			this.freeBusyStart = freeBusyStart;
			this.freeBusyEnd = freeBusyEnd;
			this.mergedFreeBusy = mergedFreeBusy;
			this.workingHours = workingHours;
			this.requestSchemaVersion = requestSchemaVersion;
		}

		private BusyType[] GetMergedFreeBusy(ExDateTime start, int duration)
		{
			int num = (int)((start - this.freeBusyStart).TotalMinutes / 30.0);
			int num2 = duration / 30;
			int num3 = (this.mergedFreeBusy != null) ? this.mergedFreeBusy.Length : 0;
			BusyType[] array = new BusyType[num2];
			for (int i = 0; i < num2; i++)
			{
				int num4 = num + i;
				if (num4 < 0 || num4 >= num3)
				{
					array[i] = BusyType.NoData;
				}
				else
				{
					array[i] = (BusyType)(this.mergedFreeBusy[num4] - '0');
					if (this.requestSchemaVersion < ExchangeVersionType.Exchange2012 && array[i] == BusyType.WorkingElsewhere)
					{
						array[i] = BusyType.NoData;
					}
				}
			}
			return array;
		}

		public BusyType[] GetBusyTypeRange(ExDateTime start, int duration)
		{
			return this.GetMergedFreeBusy(start, duration);
		}

		public BusyType GetBusyType(ExDateTime start, int duration)
		{
			BusyType[] array = this.GetMergedFreeBusy(start, duration);
			BusyType busyType = BusyType.Free;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > busyType)
				{
					busyType = array[i];
				}
			}
			return busyType;
		}

		public MeetingAttendeeType AttendeeType
		{
			get
			{
				return this.attendeeType;
			}
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public bool ExcludeConflict
		{
			get
			{
				return this.excludeConflict;
			}
		}

		public AttendeeWorkHours WorkingHours
		{
			get
			{
				return this.workingHours;
			}
		}

		public ExDateTime FreeBusyStartTime
		{
			get
			{
				return this.freeBusyStart;
			}
		}

		public ExDateTime FreeBusyEndTime
		{
			get
			{
				return this.freeBusyEnd;
			}
		}

		private const byte FreeBusyInterval = 30;

		private MeetingAttendeeType attendeeType;

		private string identity;

		private bool excludeConflict;

		private AttendeeWorkHours workingHours;

		private ExDateTime freeBusyStart;

		private ExDateTime freeBusyEnd;

		private string mergedFreeBusy;

		private ExchangeVersionType requestSchemaVersion;
	}
}
