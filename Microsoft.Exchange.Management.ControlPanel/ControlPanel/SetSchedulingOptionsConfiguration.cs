using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetSchedulingOptionsConfiguration : SetResourceConfigurationBase
	{
		public SetSchedulingOptionsConfiguration()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetMailboxCalendarConfiguration = new SetMailboxCalendarConfiguration();
		}

		public SetMailboxCalendarConfiguration SetMailboxCalendarConfiguration { get; private set; }

		[DataMember]
		public bool AutoAcceptAutomateProcessing
		{
			get
			{
				return (bool)(base["AutomateProcessing"] ?? false);
			}
			set
			{
				base["AutomateProcessing"] = (value ? CalendarProcessingFlags.AutoAccept : CalendarProcessingFlags.None);
			}
		}

		[DataMember]
		public bool DisableReminders
		{
			get
			{
				return this.SetMailboxCalendarConfiguration.DisableReminders;
			}
			set
			{
				this.SetMailboxCalendarConfiguration.DisableReminders = value;
			}
		}

		[DataMember]
		public string BookingWindowInDays { get; set; }

		[DataMember]
		public bool EnforceSchedulingHorizon
		{
			get
			{
				return (bool)(base["EnforceSchedulingHorizon"] ?? false);
			}
			set
			{
				base["EnforceSchedulingHorizon"] = value;
			}
		}

		[DataMember]
		public bool? LimitDuration { get; set; }

		[DataMember]
		public string MaximumDurationInMinutes { get; set; }

		[DataMember]
		public bool ScheduleOnlyDuringWorkHours
		{
			get
			{
				return (bool)(base["ScheduleOnlyDuringWorkHours"] ?? false);
			}
			set
			{
				base["ScheduleOnlyDuringWorkHours"] = value;
			}
		}

		[DataMember]
		public bool AllowRecurringMeetings
		{
			get
			{
				return (bool)(base["AllowRecurringMeetings"] ?? false);
			}
			set
			{
				base["AllowRecurringMeetings"] = value;
			}
		}

		[DataMember]
		public bool AllowConflicts
		{
			get
			{
				return (bool)(base["AllowConflicts"] ?? false);
			}
			set
			{
				base["AllowConflicts"] = value;
			}
		}

		[DataMember]
		public string MaximumConflictInstances { get; set; }

		[DataMember]
		public string ConflictPercentageAllowed { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.BookingWindowInDays != null)
			{
				int num;
				if (!int.TryParse(this.BookingWindowInDays, out num) || num < 0 || num > 1080)
				{
					throw new FaultException(OwaOptionStrings.BookingWindowInDaysErrorMessage);
				}
				base["BookingWindowInDays"] = num;
			}
			if (this.LimitDuration != null && !this.LimitDuration.Value)
			{
				base["MaximumDurationInMinutes"] = 0;
			}
			else if (this.MaximumDurationInMinutes != null)
			{
				int num2;
				if (!int.TryParse(this.MaximumDurationInMinutes, out num2) || num2 < 0 || num2 > 2147483647)
				{
					throw new FaultException(OwaOptionStrings.MaximumDurationInMinutesErrorMessage);
				}
				base["MaximumDurationInMinutes"] = num2;
			}
			if (this.MaximumConflictInstances != null)
			{
				int num3;
				if (!int.TryParse(this.MaximumConflictInstances, out num3) || num3 < 0 || num3 > 2147483647)
				{
					throw new FaultException(OwaOptionStrings.MaximumConflictInstancesErrorMessage);
				}
				base["MaximumConflictInstances"] = num3;
			}
			if (this.ConflictPercentageAllowed == null)
			{
				return;
			}
			int num4;
			if (int.TryParse(this.ConflictPercentageAllowed, out num4) && num4 >= 0 && num4 <= 100)
			{
				base["ConflictPercentageAllowed"] = num4;
				return;
			}
			throw new FaultException(OwaOptionStrings.ConflictPercentageAllowedErrorMessage);
		}
	}
}
