using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetCalendarAppearanceConfiguration : SetCalendarConfigurationBase
	{
		[DataMember]
		public int WorkDays
		{
			get
			{
				return (int)(base["WorkDays"] ?? 0);
			}
			set
			{
				base["WorkDays"] = value;
			}
		}

		[DataMember]
		public int WorkingHoursStartTime
		{
			get
			{
				return (int)((TimeSpan)(base["WorkingHoursStartTime"] ?? TimeSpan.Zero)).TotalMinutes;
			}
			set
			{
				base["WorkingHoursStartTime"] = TimeSpan.FromMinutes((double)value);
			}
		}

		[DataMember]
		public int WorkingHoursEndTime
		{
			get
			{
				return (int)((TimeSpan)(base["WorkingHoursEndTime"] ?? TimeSpan.Zero)).TotalMinutes;
			}
			set
			{
				base["WorkingHoursEndTime"] = TimeSpan.FromMinutes((double)value);
			}
		}

		[DataMember]
		public string WorkingHoursTimeZone
		{
			get
			{
				return (string)base["WorkingHoursTimeZone"];
			}
			set
			{
				base["WorkingHoursTimeZone"] = value;
			}
		}

		[DataMember]
		public int WeekStartDay
		{
			get
			{
				return (int)(base["WeekStartDay"] ?? 0);
			}
			set
			{
				base["WeekStartDay"] = value;
				LocalSession.Current.WeekStartDay = -1;
			}
		}

		[DataMember]
		public int FirstWeekOfYear
		{
			get
			{
				return (int)(base["FirstWeekOfYear"] ?? FirstWeekRules.FirstDay);
			}
			set
			{
				base["FirstWeekOfYear"] = value;
			}
		}

		[DataMember]
		public bool ShowWeekNumbers
		{
			get
			{
				return (bool)(base["ShowWeekNumbers"] ?? false);
			}
			set
			{
				base["ShowWeekNumbers"] = value;
			}
		}

		[DataMember]
		public string TimeIncrement
		{
			get
			{
				return (string)base["TimeIncrement"];
			}
			set
			{
				base["TimeIncrement"] = value;
			}
		}
	}
}
