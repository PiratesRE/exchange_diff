using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetCalendarReminderConfiguration : SetCalendarConfigurationBase
	{
		[DataMember]
		public bool RemindersEnabled
		{
			get
			{
				return (bool)(base["RemindersEnabled"] ?? false);
			}
			set
			{
				base["RemindersEnabled"] = value;
			}
		}

		[DataMember]
		public bool ReminderSoundEnabled
		{
			get
			{
				return (bool)(base["ReminderSoundEnabled"] ?? false);
			}
			set
			{
				base["ReminderSoundEnabled"] = value;
			}
		}

		[DataMember]
		public int DefaultReminderTime
		{
			get
			{
				return (int)((TimeSpan)(base["DefaultReminderTime"] ?? TimeSpan.Zero)).TotalMinutes;
			}
			set
			{
				base["DefaultReminderTime"] = TimeSpan.FromMinutes((double)value);
			}
		}
	}
}
