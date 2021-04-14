using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class TimeSlot
	{
		[XmlElement]
		public string Start
		{
			get
			{
				this.ValidateBeforeSerialize();
				return TimeSlot.GetTimeString(this.StartTimeInMinutes);
			}
			set
			{
				this.StartTimeInMinutes = TimeSlot.GetMinutesFromMidnight(value);
			}
		}

		[XmlElement]
		public string End
		{
			get
			{
				this.ValidateBeforeSerialize();
				return TimeSlot.GetTimeString(this.EndTimeInMinutes);
			}
			set
			{
				this.EndTimeInMinutes = TimeSlot.GetMinutesFromMidnight(value);
			}
		}

		public TimeSlot()
		{
		}

		internal TimeSlot(int startTimeInMinutes, int endTimeInMinutes)
		{
			this.StartTimeInMinutes = startTimeInMinutes;
			this.EndTimeInMinutes = endTimeInMinutes;
		}

		private static string GetTimeString(int minutesFromMidnight)
		{
			return TimeSpan.FromMinutes((double)minutesFromMidnight).ToString();
		}

		private static int GetMinutesFromMidnight(string timeString)
		{
			TimeSpan timeSpan = TimeSpan.MinValue;
			Exception ex = null;
			try
			{
				timeSpan = TimeSpan.Parse(timeString);
			}
			catch (FormatException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentNullException ex3)
			{
				ex = ex3;
			}
			catch (OverflowException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				Microsoft.Exchange.Diagnostics.Components.Data.Storage.ExTraceGlobals.WorkHoursTracer.TraceError<string, Exception>(-1L, "Unable to parse time '{0}', exception: {1}", timeString, ex);
				throw new WorkingHoursXmlMalformedException(ServerStrings.InvalidTimeSlot, ex);
			}
			return (int)timeSpan.TotalMinutes;
		}

		[XmlIgnore]
		internal int StartTimeInMinutes
		{
			get
			{
				return this.startTimeInMinutes;
			}
			set
			{
				if (value < 0 || value > 1440)
				{
					throw new InvalidWorkingHourParameterException(ServerStrings.InvalidWorkingPeriod("StartTimeInMinutes", 0, 1440));
				}
				this.startTimeInMinutes = value;
			}
		}

		[XmlIgnore]
		internal int EndTimeInMinutes
		{
			get
			{
				return this.endTimeInMinutes;
			}
			set
			{
				if (value < 0 || value > 1440)
				{
					throw new InvalidWorkingHourParameterException(ServerStrings.InvalidWorkingPeriod("EndTimeInMinutes", 0, 1440));
				}
				this.endTimeInMinutes = value;
			}
		}

		private void ValidateBeforeSerialize()
		{
			if (this.StartTimeInMinutes > this.EndTimeInMinutes)
			{
				throw new InvalidWorkingHourParameterException(ServerStrings.InvalidTimesInTimeSlot);
			}
		}

		private int startTimeInMinutes;

		private int endTimeInMinutes;

		private static readonly Trace Tracer = Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common.ExTraceGlobals.WorkingHoursTracer;
	}
}
