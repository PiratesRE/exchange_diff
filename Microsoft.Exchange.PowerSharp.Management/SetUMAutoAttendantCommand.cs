using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMAutoAttendantCommand : SyntheticCommandWithPipelineInputNoOutput<UMAutoAttendant>
	{
		private SetUMAutoAttendantCommand() : base("Set-UMAutoAttendant")
		{
		}

		public SetUMAutoAttendantCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMAutoAttendantCommand SetParameters(SetUMAutoAttendantCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMAutoAttendantCommand SetParameters(SetUMAutoAttendantCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string DTMFFallbackAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["DTMFFallbackAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual AddressListIdParameter ContactAddressList
			{
				set
				{
					base.PowerSharpParameters["ContactAddressList"] = value;
				}
			}

			public virtual string ContactRecipientContainer
			{
				set
				{
					base.PowerSharpParameters["ContactRecipientContainer"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual string TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual UMTimeZone TimeZoneName
			{
				set
				{
					base.PowerSharpParameters["TimeZoneName"] = value;
				}
			}

			public virtual string DefaultMailbox
			{
				set
				{
					base.PowerSharpParameters["DefaultMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool SpeechEnabled
			{
				set
				{
					base.PowerSharpParameters["SpeechEnabled"] = value;
				}
			}

			public virtual bool AllowDialPlanSubscribers
			{
				set
				{
					base.PowerSharpParameters["AllowDialPlanSubscribers"] = value;
				}
			}

			public virtual bool AllowExtensions
			{
				set
				{
					base.PowerSharpParameters["AllowExtensions"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedInCountryOrRegionGroups
			{
				set
				{
					base.PowerSharpParameters["AllowedInCountryOrRegionGroups"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedInternationalGroups
			{
				set
				{
					base.PowerSharpParameters["AllowedInternationalGroups"] = value;
				}
			}

			public virtual bool CallSomeoneEnabled
			{
				set
				{
					base.PowerSharpParameters["CallSomeoneEnabled"] = value;
				}
			}

			public virtual DialScopeEnum ContactScope
			{
				set
				{
					base.PowerSharpParameters["ContactScope"] = value;
				}
			}

			public virtual bool SendVoiceMsgEnabled
			{
				set
				{
					base.PowerSharpParameters["SendVoiceMsgEnabled"] = value;
				}
			}

			public virtual ScheduleInterval BusinessHoursSchedule
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursSchedule"] = value;
				}
			}

			public virtual MultiValuedProperty<string> PilotIdentifierList
			{
				set
				{
					base.PowerSharpParameters["PilotIdentifierList"] = value;
				}
			}

			public virtual MultiValuedProperty<HolidaySchedule> HolidaySchedule
			{
				set
				{
					base.PowerSharpParameters["HolidaySchedule"] = value;
				}
			}

			public virtual AutoAttendantDisambiguationFieldEnum MatchedNameSelectionMethod
			{
				set
				{
					base.PowerSharpParameters["MatchedNameSelectionMethod"] = value;
				}
			}

			public virtual string BusinessLocation
			{
				set
				{
					base.PowerSharpParameters["BusinessLocation"] = value;
				}
			}

			public virtual DayOfWeek WeekStartDay
			{
				set
				{
					base.PowerSharpParameters["WeekStartDay"] = value;
				}
			}

			public virtual UMLanguage Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual string OperatorExtension
			{
				set
				{
					base.PowerSharpParameters["OperatorExtension"] = value;
				}
			}

			public virtual string InfoAnnouncementFilename
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementFilename"] = value;
				}
			}

			public virtual InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementEnabled"] = value;
				}
			}

			public virtual bool NameLookupEnabled
			{
				set
				{
					base.PowerSharpParameters["NameLookupEnabled"] = value;
				}
			}

			public virtual bool StarOutToDialPlanEnabled
			{
				set
				{
					base.PowerSharpParameters["StarOutToDialPlanEnabled"] = value;
				}
			}

			public virtual bool ForwardCallsToDefaultMailbox
			{
				set
				{
					base.PowerSharpParameters["ForwardCallsToDefaultMailbox"] = value;
				}
			}

			public virtual string BusinessName
			{
				set
				{
					base.PowerSharpParameters["BusinessName"] = value;
				}
			}

			public virtual string BusinessHoursWelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursWelcomeGreetingFilename"] = value;
				}
			}

			public virtual bool BusinessHoursWelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursWelcomeGreetingEnabled"] = value;
				}
			}

			public virtual string BusinessHoursMainMenuCustomPromptFilename
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursMainMenuCustomPromptFilename"] = value;
				}
			}

			public virtual bool BusinessHoursMainMenuCustomPromptEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursMainMenuCustomPromptEnabled"] = value;
				}
			}

			public virtual bool BusinessHoursTransferToOperatorEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursTransferToOperatorEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<CustomMenuKeyMapping> BusinessHoursKeyMapping
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursKeyMapping"] = value;
				}
			}

			public virtual bool BusinessHoursKeyMappingEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursKeyMappingEnabled"] = value;
				}
			}

			public virtual string AfterHoursWelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["AfterHoursWelcomeGreetingFilename"] = value;
				}
			}

			public virtual bool AfterHoursWelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursWelcomeGreetingEnabled"] = value;
				}
			}

			public virtual string AfterHoursMainMenuCustomPromptFilename
			{
				set
				{
					base.PowerSharpParameters["AfterHoursMainMenuCustomPromptFilename"] = value;
				}
			}

			public virtual bool AfterHoursMainMenuCustomPromptEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursMainMenuCustomPromptEnabled"] = value;
				}
			}

			public virtual bool AfterHoursTransferToOperatorEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursTransferToOperatorEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<CustomMenuKeyMapping> AfterHoursKeyMapping
			{
				set
				{
					base.PowerSharpParameters["AfterHoursKeyMapping"] = value;
				}
			}

			public virtual bool AfterHoursKeyMappingEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursKeyMappingEnabled"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual string DTMFFallbackAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["DTMFFallbackAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual AddressListIdParameter ContactAddressList
			{
				set
				{
					base.PowerSharpParameters["ContactAddressList"] = value;
				}
			}

			public virtual string ContactRecipientContainer
			{
				set
				{
					base.PowerSharpParameters["ContactRecipientContainer"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual string TimeZone
			{
				set
				{
					base.PowerSharpParameters["TimeZone"] = value;
				}
			}

			public virtual UMTimeZone TimeZoneName
			{
				set
				{
					base.PowerSharpParameters["TimeZoneName"] = value;
				}
			}

			public virtual string DefaultMailbox
			{
				set
				{
					base.PowerSharpParameters["DefaultMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool SpeechEnabled
			{
				set
				{
					base.PowerSharpParameters["SpeechEnabled"] = value;
				}
			}

			public virtual bool AllowDialPlanSubscribers
			{
				set
				{
					base.PowerSharpParameters["AllowDialPlanSubscribers"] = value;
				}
			}

			public virtual bool AllowExtensions
			{
				set
				{
					base.PowerSharpParameters["AllowExtensions"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedInCountryOrRegionGroups
			{
				set
				{
					base.PowerSharpParameters["AllowedInCountryOrRegionGroups"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedInternationalGroups
			{
				set
				{
					base.PowerSharpParameters["AllowedInternationalGroups"] = value;
				}
			}

			public virtual bool CallSomeoneEnabled
			{
				set
				{
					base.PowerSharpParameters["CallSomeoneEnabled"] = value;
				}
			}

			public virtual DialScopeEnum ContactScope
			{
				set
				{
					base.PowerSharpParameters["ContactScope"] = value;
				}
			}

			public virtual bool SendVoiceMsgEnabled
			{
				set
				{
					base.PowerSharpParameters["SendVoiceMsgEnabled"] = value;
				}
			}

			public virtual ScheduleInterval BusinessHoursSchedule
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursSchedule"] = value;
				}
			}

			public virtual MultiValuedProperty<string> PilotIdentifierList
			{
				set
				{
					base.PowerSharpParameters["PilotIdentifierList"] = value;
				}
			}

			public virtual MultiValuedProperty<HolidaySchedule> HolidaySchedule
			{
				set
				{
					base.PowerSharpParameters["HolidaySchedule"] = value;
				}
			}

			public virtual AutoAttendantDisambiguationFieldEnum MatchedNameSelectionMethod
			{
				set
				{
					base.PowerSharpParameters["MatchedNameSelectionMethod"] = value;
				}
			}

			public virtual string BusinessLocation
			{
				set
				{
					base.PowerSharpParameters["BusinessLocation"] = value;
				}
			}

			public virtual DayOfWeek WeekStartDay
			{
				set
				{
					base.PowerSharpParameters["WeekStartDay"] = value;
				}
			}

			public virtual UMLanguage Language
			{
				set
				{
					base.PowerSharpParameters["Language"] = value;
				}
			}

			public virtual string OperatorExtension
			{
				set
				{
					base.PowerSharpParameters["OperatorExtension"] = value;
				}
			}

			public virtual string InfoAnnouncementFilename
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementFilename"] = value;
				}
			}

			public virtual InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementEnabled"] = value;
				}
			}

			public virtual bool NameLookupEnabled
			{
				set
				{
					base.PowerSharpParameters["NameLookupEnabled"] = value;
				}
			}

			public virtual bool StarOutToDialPlanEnabled
			{
				set
				{
					base.PowerSharpParameters["StarOutToDialPlanEnabled"] = value;
				}
			}

			public virtual bool ForwardCallsToDefaultMailbox
			{
				set
				{
					base.PowerSharpParameters["ForwardCallsToDefaultMailbox"] = value;
				}
			}

			public virtual string BusinessName
			{
				set
				{
					base.PowerSharpParameters["BusinessName"] = value;
				}
			}

			public virtual string BusinessHoursWelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursWelcomeGreetingFilename"] = value;
				}
			}

			public virtual bool BusinessHoursWelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursWelcomeGreetingEnabled"] = value;
				}
			}

			public virtual string BusinessHoursMainMenuCustomPromptFilename
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursMainMenuCustomPromptFilename"] = value;
				}
			}

			public virtual bool BusinessHoursMainMenuCustomPromptEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursMainMenuCustomPromptEnabled"] = value;
				}
			}

			public virtual bool BusinessHoursTransferToOperatorEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursTransferToOperatorEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<CustomMenuKeyMapping> BusinessHoursKeyMapping
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursKeyMapping"] = value;
				}
			}

			public virtual bool BusinessHoursKeyMappingEnabled
			{
				set
				{
					base.PowerSharpParameters["BusinessHoursKeyMappingEnabled"] = value;
				}
			}

			public virtual string AfterHoursWelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["AfterHoursWelcomeGreetingFilename"] = value;
				}
			}

			public virtual bool AfterHoursWelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursWelcomeGreetingEnabled"] = value;
				}
			}

			public virtual string AfterHoursMainMenuCustomPromptFilename
			{
				set
				{
					base.PowerSharpParameters["AfterHoursMainMenuCustomPromptFilename"] = value;
				}
			}

			public virtual bool AfterHoursMainMenuCustomPromptEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursMainMenuCustomPromptEnabled"] = value;
				}
			}

			public virtual bool AfterHoursTransferToOperatorEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursTransferToOperatorEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<CustomMenuKeyMapping> AfterHoursKeyMapping
			{
				set
				{
					base.PowerSharpParameters["AfterHoursKeyMapping"] = value;
				}
			}

			public virtual bool AfterHoursKeyMappingEnabled
			{
				set
				{
					base.PowerSharpParameters["AfterHoursKeyMappingEnabled"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
