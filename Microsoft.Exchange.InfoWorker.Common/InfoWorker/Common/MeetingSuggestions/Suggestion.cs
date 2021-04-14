using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class Suggestion
	{
		public Suggestion()
		{
		}

		internal Suggestion(ExDateTime meetingStartTime, int inputMeetingDuration, int inputRequiredAttendeeCount, int inputOptionalAttendeeCount, AttendeeData[] inputAttendees, ConfigOptions inputOptions)
		{
			this.options = inputOptions;
			Suggestion.Tracer.TraceDebug<object, ExDateTime>((long)this.GetHashCode(), "{0}: Suggestion.Suggestion() entered, inputStartDateTime: {1}", TraceContext.Get(), meetingStartTime);
			this.meetingStartTime = meetingStartTime;
			this.startDateTime = meetingStartTime.LocalTime;
			this.meetingDuration = inputMeetingDuration;
			this.requiredAttendeeCount = inputRequiredAttendeeCount;
			this.optionalAttendeeCount = inputOptionalAttendeeCount;
			ExDateTime endUtc = meetingStartTime.AddMinutes((double)inputMeetingDuration);
			bool flag = true;
			int num = 0;
			this.attendeeConflictDataArray = new AttendeeConflictData[inputAttendees.Length];
			foreach (AttendeeData attendeeData in inputAttendees)
			{
				BusyType busyType = attendeeData.GetBusyType(this.meetingStartTime, this.meetingDuration);
				IndividualAttendeeConflictData individualAttendeeConflictData = IndividualAttendeeConflictData.Create(attendeeData, busyType);
				if (attendeeData.AttendeeType == MeetingAttendeeType.Required || attendeeData.AttendeeType == MeetingAttendeeType.Organizer)
				{
					this.SetRequiredAttendeeAvailability(attendeeData, individualAttendeeConflictData);
					if (individualAttendeeConflictData.BusyType != BusyType.NoData)
					{
						this.excludeConflict |= attendeeData.ExcludeConflict;
					}
				}
				else if (attendeeData.AttendeeType == MeetingAttendeeType.Optional)
				{
					this.SetOptionalAttendeeAvailability(attendeeData, individualAttendeeConflictData);
				}
				else if (attendeeData.AttendeeType == MeetingAttendeeType.Room)
				{
					this.SetRoomAttendeeAvailability(attendeeData, individualAttendeeConflictData);
					if (individualAttendeeConflictData.BusyType != BusyType.NoData)
					{
						flag &= attendeeData.ExcludeConflict;
					}
				}
				else if (attendeeData.AttendeeType == MeetingAttendeeType.Resource)
				{
					this.SetResourceAttendeeAvailability(attendeeData, individualAttendeeConflictData);
				}
				else
				{
					Suggestion.Tracer.TraceError<object, MeetingAttendeeType>((long)this.GetHashCode(), "{0}: unknown attendee type: {1}", TraceContext.Get(), attendeeData.AttendeeType);
				}
				if (attendeeData.AttendeeType == MeetingAttendeeType.Organizer)
				{
					this.isWorkTime = attendeeData.WorkingHours.IsWorkTime(this.meetingStartTime, endUtc);
				}
				this.attendeeConflictDataArray[num] = individualAttendeeConflictData;
				num++;
			}
			if (this.roomsRequested)
			{
				this.excludeConflict = (this.excludeConflict || flag);
			}
			Suggestion.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: required: {1}, optional: {2}, worktime?: {3}", new object[]
			{
				TraceContext.Get(),
				this.requiredAttendeeCount,
				this.optionalAttendeeCount,
				this.isWorkTime
			});
			if (this.requiredAttendeeCount != 0)
			{
				this.sumPctReqFrontTime = this.sumReqFrontTime / (double)this.requiredAttendeeCount;
				this.weightedPctReqConflicts = this.weightedReqConflicts / (double)this.requiredAttendeeCount;
			}
			else
			{
				this.weightedPctReqConflicts = 1.0;
			}
			if (this.optionalAttendeeCount != 0)
			{
				this.pctOptConflicts = (double)this.optionalAttendeeConflictCount / (double)this.optionalAttendeeCount;
				this.pctOptOverlap = this.optOverlap / (double)this.optionalAttendeeCount;
			}
			this.timeSlotRating = this.GetRating();
			if (this.weightedPctReqConflicts == 0.0)
			{
				this.bucket = SuggestionQuality.Excellent;
			}
			else if (this.weightedPctReqConflicts * 100.0 >= 50.0)
			{
				this.bucket = SuggestionQuality.Poor;
			}
			else if (this.weightedPctReqConflicts * 100.0 <= (double)this.options.GoodThreshold)
			{
				this.bucket = SuggestionQuality.Good;
			}
			else
			{
				this.bucket = SuggestionQuality.Fair;
			}
			Suggestion.Tracer.TraceDebug<object, long, SuggestionQuality>((long)this.GetHashCode(), "{0}: final suggestion results, timeSlotRating: {1}, bucket: {2}", TraceContext.Get(), this.timeSlotRating, this.bucket);
		}

		private void SetConvenience(AttendeeData attendee)
		{
			int convenience = attendee.WorkingHours.GetConvenience(this.meetingStartTime, this.meetingDuration);
			if (convenience > this.convenienceMax)
			{
				this.convenienceMax = convenience;
			}
			this.convenienceSum += convenience;
			if (this.convenienceSum > 256)
			{
				this.convenienceSum = 256;
			}
		}

		private double GetConflictValue(BusyType busyType)
		{
			double result = 0.0;
			switch (busyType)
			{
			case BusyType.Tentative:
			case BusyType.NoData:
				result = 0.5;
				break;
			case BusyType.Busy:
			case BusyType.OOF:
				result = 1.0;
				break;
			}
			return result;
		}

		private void SetRequiredAttendeeAvailability(AttendeeData attendee, IndividualAttendeeConflictData attendeeConflict)
		{
			BusyType busyType = attendeeConflict.BusyType;
			if (busyType == BusyType.NoData)
			{
				this.requiredAttendeeCount--;
				return;
			}
			if (this.strongestReqConflict < busyType)
			{
				this.strongestReqConflict = busyType;
			}
			if (busyType != BusyType.Free)
			{
				this.requiredAttendeeConflictCount++;
			}
			this.SetConvenience(attendee);
			int num = this.meetingDuration / this.options.FreeBusyInterval;
			int num2 = 0;
			int num3 = 0;
			double num4 = 0.0;
			if (num > 0)
			{
				BusyType[] busyTypeRange = attendee.GetBusyTypeRange(this.meetingStartTime, this.meetingDuration);
				ExDateTime startUtc = this.meetingStartTime.ToUtc();
				foreach (BusyType busyType2 in busyTypeRange)
				{
					ExDateTime exDateTime = startUtc.AddMinutes((double)this.options.FreeBusyInterval);
					if (busyType2 != BusyType.Free)
					{
						num4 += this.GetConflictValue(busyType2);
						num3++;
					}
					else if (!attendee.WorkingHours.IsWorkTime(startUtc, exDateTime))
					{
						num4 += 0.25;
						num3++;
					}
					else if (num3 == 0)
					{
						num2++;
					}
					startUtc = exDateTime;
				}
				this.sumReqFrontTime += (double)num2 / (double)num;
				double num5 = (double)num2 / (double)num;
				if (num5 < this.minPctReqFrontTime)
				{
					this.minPctReqFrontTime = num5;
				}
				this.weightedReqConflicts += num4 / (double)num;
			}
		}

		private void SetOptionalAttendeeAvailability(AttendeeData attendee, IndividualAttendeeConflictData attendeeConflict)
		{
			BusyType busyType = attendeeConflict.BusyType;
			if (busyType == BusyType.NoData)
			{
				this.optionalAttendeeCount--;
				return;
			}
			if (busyType != BusyType.Free)
			{
				if (this.strongestOptConflict < busyType)
				{
					this.strongestOptConflict = busyType;
				}
				this.optionalAttendeeConflictCount++;
				this.weightedOptConflicts += (double)busyType * 0.25 + 0.25;
				BusyType[] busyTypeRange = attendee.GetBusyTypeRange(this.meetingStartTime, this.meetingDuration);
				foreach (BusyType busyType2 in busyTypeRange)
				{
					if (busyType2 != BusyType.Free)
					{
						this.optOverlap += (double)this.options.FreeBusyInterval / (double)this.meetingDuration;
						this.weightedOptOverlap += ((double)busyType * 0.25 + 0.25) * ((double)this.options.FreeBusyInterval / (double)this.meetingDuration);
					}
				}
			}
		}

		private void SetRoomAttendeeAvailability(AttendeeData attendee, IndividualAttendeeConflictData attendeeConflict)
		{
			BusyType busyType = attendeeConflict.BusyType;
			if (busyType == BusyType.NoData)
			{
				return;
			}
			this.roomsRequested = true;
			this.roomCount++;
			if (busyType == BusyType.Free)
			{
				this.roomsAvailableCount++;
				return;
			}
			this.roomConflictCount++;
		}

		private void SetResourceAttendeeAvailability(AttendeeData attendee, IndividualAttendeeConflictData attendeeConflict)
		{
			BusyType busyType = attendeeConflict.BusyType;
			if (busyType == BusyType.NoData)
			{
				return;
			}
			this.resourceAttendeeCount++;
			if (busyType == BusyType.Free)
			{
				this.resourceAttendeeAvailableCount++;
				return;
			}
			this.resourceAttendeeConflictCount++;
		}

		private long GetRating()
		{
			long num = 0L;
			if (this.requiredAttendeeConflictCount > 0 && this.excludeConflict)
			{
				return -1L;
			}
			num += (long)Math.Round(this.weightedPctReqConflicts * 100.0) << 56;
			num += (long)this.strongestReqConflict << 53;
			num += (long)Math.Round((1.0 - this.minPctReqFrontTime) * 100.0) << 46;
			num += (long)Math.Round((1.0 - this.sumPctReqFrontTime) * 100.0) << 39;
			if (this.roomsRequested && this.roomsAvailableCount == 0)
			{
				num += 274877906944L;
			}
			num += (long)this.convenienceMax << 32;
			num += (long)this.convenienceSum << 24;
			num += (long)Math.Round(this.pctOptConflicts * 100.0) << 17;
			num += (long)this.strongestOptConflict << 14;
			return num + ((long)Math.Round(this.pctOptOverlap * 100.0) << 7);
		}

		[XmlElement]
		public DateTime MeetingTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				this.startDateTime = value;
			}
		}

		[XmlElement]
		public bool IsWorkTime
		{
			get
			{
				return this.isWorkTime;
			}
			set
			{
				this.isWorkTime = value;
			}
		}

		[XmlIgnore]
		public TimeSpan Time
		{
			get
			{
				return this.startDateTime.TimeOfDay;
			}
		}

		[XmlIgnore]
		public long TimeSlotRating
		{
			get
			{
				return this.timeSlotRating;
			}
		}

		[XmlElement]
		public SuggestionQuality SuggestionQuality
		{
			get
			{
				return this.bucket;
			}
			set
			{
				this.bucket = value;
			}
		}

		[XmlIgnore]
		public int RequiredAttendeeConflictCount
		{
			get
			{
				return this.requiredAttendeeConflictCount;
			}
		}

		[XmlIgnore]
		public int OptionalAttendeeConflictCount
		{
			get
			{
				return this.optionalAttendeeConflictCount;
			}
		}

		[XmlIgnore]
		public int RequiredAttendeeCount
		{
			get
			{
				return this.requiredAttendeeCount;
			}
		}

		[XmlIgnore]
		public int OptionalAttendeeCount
		{
			get
			{
				return this.optionalAttendeeCount;
			}
		}

		[XmlIgnore]
		public int ResourceAttendeeConflictCount
		{
			get
			{
				return this.resourceAttendeeConflictCount;
			}
		}

		[XmlIgnore]
		public int AvailableRoomsCount
		{
			get
			{
				return this.roomsAvailableCount;
			}
		}

		[XmlIgnore]
		public int RoomCount
		{
			get
			{
				return this.roomCount;
			}
		}

		[XmlIgnore]
		public int ResourceAttendeeCount
		{
			get
			{
				return this.resourceAttendeeCount;
			}
		}

		[XmlIgnore]
		public int ResourceAttendeeAvailableCount
		{
			get
			{
				return this.resourceAttendeeAvailableCount;
			}
		}

		[XmlIgnore]
		public int RoomConflictCount
		{
			get
			{
				return this.roomConflictCount;
			}
		}

		[XmlArray(IsNullable = false)]
		[XmlArrayItem(Type = typeof(TooBigGroupAttendeeConflictData))]
		[XmlArrayItem(Type = typeof(UnknownAttendeeConflictData))]
		[XmlArrayItem(Type = typeof(IndividualAttendeeConflictData))]
		[XmlArrayItem(Type = typeof(GroupAttendeeConflictData))]
		public AttendeeConflictData[] AttendeeConflictDataArray
		{
			get
			{
				return this.attendeeConflictDataArray;
			}
			set
			{
				this.attendeeConflictDataArray = value;
			}
		}

		private const int MaximumInconvenienceSum = 256;

		private static readonly Trace Tracer = ExTraceGlobals.MeetingSuggestionsTracer;

		private DateTime startDateTime;

		private ExDateTime meetingStartTime;

		private long timeSlotRating;

		private bool isWorkTime = true;

		private bool excludeConflict;

		private SuggestionQuality bucket;

		private int requiredAttendeeConflictCount;

		private int optionalAttendeeConflictCount;

		private int resourceAttendeeCount;

		private int resourceAttendeeAvailableCount;

		private int resourceAttendeeConflictCount;

		private int roomCount;

		private int roomConflictCount;

		private int roomsAvailableCount;

		private bool roomsRequested;

		private int requiredAttendeeCount;

		private int optionalAttendeeCount;

		private int convenienceSum;

		private int convenienceMax;

		private int meetingDuration;

		private BusyType strongestReqConflict;

		private BusyType strongestOptConflict;

		private double weightedReqConflicts;

		private double weightedPctReqConflicts;

		private double pctOptConflicts;

		private double weightedOptConflicts;

		private double optOverlap;

		private double pctOptOverlap;

		private double weightedOptOverlap;

		private double sumReqFrontTime;

		private double sumPctReqFrontTime;

		private double minPctReqFrontTime = 1.0;

		private ConfigOptions options;

		private AttendeeConflictData[] attendeeConflictDataArray;
	}
}
