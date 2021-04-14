using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetCalendarProcessingCommand : SyntheticCommandWithPipelineInputNoOutput<CalendarConfiguration>
	{
		private SetCalendarProcessingCommand() : base("Set-CalendarProcessing")
		{
		}

		public SetCalendarProcessingCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetCalendarProcessingCommand SetParameters(SetCalendarProcessingCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetCalendarProcessingCommand SetParameters(SetCalendarProcessingCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual RecipientIdParameter ResourceDelegates
			{
				set
				{
					base.PowerSharpParameters["ResourceDelegates"] = value;
				}
			}

			public virtual RecipientIdParameter RequestOutOfPolicy
			{
				set
				{
					base.PowerSharpParameters["RequestOutOfPolicy"] = value;
				}
			}

			public virtual RecipientIdParameter BookInPolicy
			{
				set
				{
					base.PowerSharpParameters["BookInPolicy"] = value;
				}
			}

			public virtual RecipientIdParameter RequestInPolicy
			{
				set
				{
					base.PowerSharpParameters["RequestInPolicy"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual CalendarProcessingFlags AutomateProcessing
			{
				set
				{
					base.PowerSharpParameters["AutomateProcessing"] = value;
				}
			}

			public virtual bool AllowConflicts
			{
				set
				{
					base.PowerSharpParameters["AllowConflicts"] = value;
				}
			}

			public virtual int BookingWindowInDays
			{
				set
				{
					base.PowerSharpParameters["BookingWindowInDays"] = value;
				}
			}

			public virtual int MaximumDurationInMinutes
			{
				set
				{
					base.PowerSharpParameters["MaximumDurationInMinutes"] = value;
				}
			}

			public virtual bool AllowRecurringMeetings
			{
				set
				{
					base.PowerSharpParameters["AllowRecurringMeetings"] = value;
				}
			}

			public virtual bool EnforceSchedulingHorizon
			{
				set
				{
					base.PowerSharpParameters["EnforceSchedulingHorizon"] = value;
				}
			}

			public virtual bool ScheduleOnlyDuringWorkHours
			{
				set
				{
					base.PowerSharpParameters["ScheduleOnlyDuringWorkHours"] = value;
				}
			}

			public virtual int ConflictPercentageAllowed
			{
				set
				{
					base.PowerSharpParameters["ConflictPercentageAllowed"] = value;
				}
			}

			public virtual int MaximumConflictInstances
			{
				set
				{
					base.PowerSharpParameters["MaximumConflictInstances"] = value;
				}
			}

			public virtual bool ForwardRequestsToDelegates
			{
				set
				{
					base.PowerSharpParameters["ForwardRequestsToDelegates"] = value;
				}
			}

			public virtual bool DeleteAttachments
			{
				set
				{
					base.PowerSharpParameters["DeleteAttachments"] = value;
				}
			}

			public virtual bool DeleteComments
			{
				set
				{
					base.PowerSharpParameters["DeleteComments"] = value;
				}
			}

			public virtual bool RemovePrivateProperty
			{
				set
				{
					base.PowerSharpParameters["RemovePrivateProperty"] = value;
				}
			}

			public virtual bool DeleteSubject
			{
				set
				{
					base.PowerSharpParameters["DeleteSubject"] = value;
				}
			}

			public virtual bool AddOrganizerToSubject
			{
				set
				{
					base.PowerSharpParameters["AddOrganizerToSubject"] = value;
				}
			}

			public virtual bool DeleteNonCalendarItems
			{
				set
				{
					base.PowerSharpParameters["DeleteNonCalendarItems"] = value;
				}
			}

			public virtual bool TentativePendingApproval
			{
				set
				{
					base.PowerSharpParameters["TentativePendingApproval"] = value;
				}
			}

			public virtual bool EnableResponseDetails
			{
				set
				{
					base.PowerSharpParameters["EnableResponseDetails"] = value;
				}
			}

			public virtual bool OrganizerInfo
			{
				set
				{
					base.PowerSharpParameters["OrganizerInfo"] = value;
				}
			}

			public virtual bool AllRequestOutOfPolicy
			{
				set
				{
					base.PowerSharpParameters["AllRequestOutOfPolicy"] = value;
				}
			}

			public virtual bool AllBookInPolicy
			{
				set
				{
					base.PowerSharpParameters["AllBookInPolicy"] = value;
				}
			}

			public virtual bool AllRequestInPolicy
			{
				set
				{
					base.PowerSharpParameters["AllRequestInPolicy"] = value;
				}
			}

			public virtual bool AddAdditionalResponse
			{
				set
				{
					base.PowerSharpParameters["AddAdditionalResponse"] = value;
				}
			}

			public virtual string AdditionalResponse
			{
				set
				{
					base.PowerSharpParameters["AdditionalResponse"] = value;
				}
			}

			public virtual bool RemoveOldMeetingMessages
			{
				set
				{
					base.PowerSharpParameters["RemoveOldMeetingMessages"] = value;
				}
			}

			public virtual bool AddNewRequestsTentatively
			{
				set
				{
					base.PowerSharpParameters["AddNewRequestsTentatively"] = value;
				}
			}

			public virtual bool ProcessExternalMeetingMessages
			{
				set
				{
					base.PowerSharpParameters["ProcessExternalMeetingMessages"] = value;
				}
			}

			public virtual bool RemoveForwardedMeetingNotifications
			{
				set
				{
					base.PowerSharpParameters["RemoveForwardedMeetingNotifications"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual RecipientIdParameter ResourceDelegates
			{
				set
				{
					base.PowerSharpParameters["ResourceDelegates"] = value;
				}
			}

			public virtual RecipientIdParameter RequestOutOfPolicy
			{
				set
				{
					base.PowerSharpParameters["RequestOutOfPolicy"] = value;
				}
			}

			public virtual RecipientIdParameter BookInPolicy
			{
				set
				{
					base.PowerSharpParameters["BookInPolicy"] = value;
				}
			}

			public virtual RecipientIdParameter RequestInPolicy
			{
				set
				{
					base.PowerSharpParameters["RequestInPolicy"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual CalendarProcessingFlags AutomateProcessing
			{
				set
				{
					base.PowerSharpParameters["AutomateProcessing"] = value;
				}
			}

			public virtual bool AllowConflicts
			{
				set
				{
					base.PowerSharpParameters["AllowConflicts"] = value;
				}
			}

			public virtual int BookingWindowInDays
			{
				set
				{
					base.PowerSharpParameters["BookingWindowInDays"] = value;
				}
			}

			public virtual int MaximumDurationInMinutes
			{
				set
				{
					base.PowerSharpParameters["MaximumDurationInMinutes"] = value;
				}
			}

			public virtual bool AllowRecurringMeetings
			{
				set
				{
					base.PowerSharpParameters["AllowRecurringMeetings"] = value;
				}
			}

			public virtual bool EnforceSchedulingHorizon
			{
				set
				{
					base.PowerSharpParameters["EnforceSchedulingHorizon"] = value;
				}
			}

			public virtual bool ScheduleOnlyDuringWorkHours
			{
				set
				{
					base.PowerSharpParameters["ScheduleOnlyDuringWorkHours"] = value;
				}
			}

			public virtual int ConflictPercentageAllowed
			{
				set
				{
					base.PowerSharpParameters["ConflictPercentageAllowed"] = value;
				}
			}

			public virtual int MaximumConflictInstances
			{
				set
				{
					base.PowerSharpParameters["MaximumConflictInstances"] = value;
				}
			}

			public virtual bool ForwardRequestsToDelegates
			{
				set
				{
					base.PowerSharpParameters["ForwardRequestsToDelegates"] = value;
				}
			}

			public virtual bool DeleteAttachments
			{
				set
				{
					base.PowerSharpParameters["DeleteAttachments"] = value;
				}
			}

			public virtual bool DeleteComments
			{
				set
				{
					base.PowerSharpParameters["DeleteComments"] = value;
				}
			}

			public virtual bool RemovePrivateProperty
			{
				set
				{
					base.PowerSharpParameters["RemovePrivateProperty"] = value;
				}
			}

			public virtual bool DeleteSubject
			{
				set
				{
					base.PowerSharpParameters["DeleteSubject"] = value;
				}
			}

			public virtual bool AddOrganizerToSubject
			{
				set
				{
					base.PowerSharpParameters["AddOrganizerToSubject"] = value;
				}
			}

			public virtual bool DeleteNonCalendarItems
			{
				set
				{
					base.PowerSharpParameters["DeleteNonCalendarItems"] = value;
				}
			}

			public virtual bool TentativePendingApproval
			{
				set
				{
					base.PowerSharpParameters["TentativePendingApproval"] = value;
				}
			}

			public virtual bool EnableResponseDetails
			{
				set
				{
					base.PowerSharpParameters["EnableResponseDetails"] = value;
				}
			}

			public virtual bool OrganizerInfo
			{
				set
				{
					base.PowerSharpParameters["OrganizerInfo"] = value;
				}
			}

			public virtual bool AllRequestOutOfPolicy
			{
				set
				{
					base.PowerSharpParameters["AllRequestOutOfPolicy"] = value;
				}
			}

			public virtual bool AllBookInPolicy
			{
				set
				{
					base.PowerSharpParameters["AllBookInPolicy"] = value;
				}
			}

			public virtual bool AllRequestInPolicy
			{
				set
				{
					base.PowerSharpParameters["AllRequestInPolicy"] = value;
				}
			}

			public virtual bool AddAdditionalResponse
			{
				set
				{
					base.PowerSharpParameters["AddAdditionalResponse"] = value;
				}
			}

			public virtual string AdditionalResponse
			{
				set
				{
					base.PowerSharpParameters["AdditionalResponse"] = value;
				}
			}

			public virtual bool RemoveOldMeetingMessages
			{
				set
				{
					base.PowerSharpParameters["RemoveOldMeetingMessages"] = value;
				}
			}

			public virtual bool AddNewRequestsTentatively
			{
				set
				{
					base.PowerSharpParameters["AddNewRequestsTentatively"] = value;
				}
			}

			public virtual bool ProcessExternalMeetingMessages
			{
				set
				{
					base.PowerSharpParameters["ProcessExternalMeetingMessages"] = value;
				}
			}

			public virtual bool RemoveForwardedMeetingNotifications
			{
				set
				{
					base.PowerSharpParameters["RemoveForwardedMeetingNotifications"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
