using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class CalendarConfiguration : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return CalendarConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public CalendarProcessingFlags AutomateProcessing
		{
			get
			{
				return (CalendarProcessingFlags)this[CalendarConfigurationSchema.AutomateProcessing];
			}
			set
			{
				this[CalendarConfigurationSchema.AutomateProcessing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowConflicts
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AllowConflicts];
			}
			set
			{
				this[CalendarConfigurationSchema.AllowConflicts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int BookingWindowInDays
		{
			get
			{
				return (int)this[CalendarConfigurationSchema.BookingWindowInDays];
			}
			set
			{
				this[CalendarConfigurationSchema.BookingWindowInDays] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaximumDurationInMinutes
		{
			get
			{
				return (int)this[CalendarConfigurationSchema.MaximumDurationInMinutes];
			}
			set
			{
				this[CalendarConfigurationSchema.MaximumDurationInMinutes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowRecurringMeetings
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AllowRecurringMeetings];
			}
			set
			{
				this[CalendarConfigurationSchema.AllowRecurringMeetings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnforceSchedulingHorizon
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.EnforceSchedulingHorizon];
			}
			set
			{
				this[CalendarConfigurationSchema.EnforceSchedulingHorizon] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ScheduleOnlyDuringWorkHours
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.ScheduleOnlyDuringWorkHours];
			}
			set
			{
				this[CalendarConfigurationSchema.ScheduleOnlyDuringWorkHours] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ConflictPercentageAllowed
		{
			get
			{
				return (int)this[CalendarConfigurationSchema.ConflictPercentageAllowed];
			}
			set
			{
				this[CalendarConfigurationSchema.ConflictPercentageAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaximumConflictInstances
		{
			get
			{
				return (int)this[CalendarConfigurationSchema.MaximumConflictInstances];
			}
			set
			{
				this[CalendarConfigurationSchema.MaximumConflictInstances] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForwardRequestsToDelegates
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.ForwardRequestsToDelegates];
			}
			set
			{
				this[CalendarConfigurationSchema.ForwardRequestsToDelegates] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeleteAttachments
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.DeleteAttachments];
			}
			set
			{
				this[CalendarConfigurationSchema.DeleteAttachments] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeleteComments
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.DeleteComments];
			}
			set
			{
				this[CalendarConfigurationSchema.DeleteComments] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemovePrivateProperty
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.RemovePrivateProperty];
			}
			set
			{
				this[CalendarConfigurationSchema.RemovePrivateProperty] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeleteSubject
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.DeleteSubject];
			}
			set
			{
				this[CalendarConfigurationSchema.DeleteSubject] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AddOrganizerToSubject
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AddOrganizerToSubject];
			}
			set
			{
				this[CalendarConfigurationSchema.AddOrganizerToSubject] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeleteNonCalendarItems
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.DeleteNonCalendarItems];
			}
			set
			{
				this[CalendarConfigurationSchema.DeleteNonCalendarItems] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TentativePendingApproval
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.TentativePendingApproval];
			}
			set
			{
				this[CalendarConfigurationSchema.TentativePendingApproval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableResponseDetails
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.EnableResponseDetails];
			}
			set
			{
				this[CalendarConfigurationSchema.EnableResponseDetails] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OrganizerInfo
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.OrganizerInfo];
			}
			set
			{
				this[CalendarConfigurationSchema.OrganizerInfo] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ResourceDelegates
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[CalendarConfigurationSchema.ResourceDelegates];
			}
			set
			{
				this[CalendarConfigurationSchema.ResourceDelegates] = value;
			}
		}

		public MultiValuedProperty<string> RequestOutOfPolicy
		{
			get
			{
				return (MultiValuedProperty<string>)this[CalendarConfigurationSchema.RequestOutOfPolicyLegDN];
			}
			set
			{
				this[CalendarConfigurationSchema.RequestOutOfPolicyLegDN] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllRequestOutOfPolicy
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AllRequestOutOfPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.AllRequestOutOfPolicy] = value;
			}
		}

		public MultiValuedProperty<string> BookInPolicy
		{
			get
			{
				return (MultiValuedProperty<string>)this[CalendarConfigurationSchema.BookInPolicyLegDN];
			}
			set
			{
				this[CalendarConfigurationSchema.BookInPolicyLegDN] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllBookInPolicy
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AllBookInPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.AllBookInPolicy] = value;
			}
		}

		public MultiValuedProperty<string> RequestInPolicy
		{
			get
			{
				return (MultiValuedProperty<string>)this[CalendarConfigurationSchema.RequestInPolicyLegDN];
			}
			set
			{
				this[CalendarConfigurationSchema.RequestInPolicyLegDN] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllRequestInPolicy
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AllRequestInPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.AllRequestInPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AddAdditionalResponse
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AddAdditionalResponse];
			}
			set
			{
				this[CalendarConfigurationSchema.AddAdditionalResponse] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AdditionalResponse
		{
			get
			{
				return (string)this[CalendarConfigurationSchema.AdditionalResponse];
			}
			set
			{
				this[CalendarConfigurationSchema.AdditionalResponse] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemoveOldMeetingMessages
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.RemoveOldMeetingMessages];
			}
			set
			{
				this[CalendarConfigurationSchema.RemoveOldMeetingMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AddNewRequestsTentatively
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.AddNewRequestsTentatively];
			}
			set
			{
				this[CalendarConfigurationSchema.AddNewRequestsTentatively] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ProcessExternalMeetingMessages
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.ProcessExternalMeetingMessages];
			}
			set
			{
				this[CalendarConfigurationSchema.ProcessExternalMeetingMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemoveForwardedMeetingNotifications
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.RemoveForwardedMeetingNotifications];
			}
			set
			{
				this[CalendarConfigurationSchema.RemoveForwardedMeetingNotifications] = value;
			}
		}

		internal int DefaultReminderTime
		{
			get
			{
				return (int)this[CalendarConfigurationSchema.DefaultReminderTime];
			}
			set
			{
				this[CalendarConfigurationSchema.DefaultReminderTime] = value;
			}
		}

		internal bool DisableReminders
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.DisableReminders];
			}
			set
			{
				this[CalendarConfigurationSchema.DisableReminders] = value;
			}
		}

		internal bool SkipProcessing
		{
			get
			{
				return (bool)this[CalendarConfigurationSchema.SkipProcessing];
			}
			set
			{
				this[CalendarConfigurationSchema.SkipProcessing] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> RequestOutOfPolicyLegacy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[CalendarConfigurationSchema.RequestOutOfPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.RequestOutOfPolicy] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> BookInPolicyLegacy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[CalendarConfigurationSchema.BookInPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.BookInPolicy] = value;
			}
		}

		internal MultiValuedProperty<ADObjectId> RequestInPolicyLegacy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[CalendarConfigurationSchema.RequestInPolicy];
			}
			set
			{
				this[CalendarConfigurationSchema.RequestInPolicy] = value;
			}
		}

		private static CalendarConfigurationSchema schema = ObjectSchema.GetInstance<CalendarConfigurationSchema>();
	}
}
