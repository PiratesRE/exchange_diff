using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMMailboxPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<UMMailboxPolicy>
	{
		private SetUMMailboxPolicyCommand() : base("Set-UMMailboxPolicy")
		{
		}

		public SetUMMailboxPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMMailboxPolicyCommand SetParameters(SetUMMailboxPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMMailboxPolicyCommand SetParameters(SetUMMailboxPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string UMDialPlan
			{
				set
				{
					base.PowerSharpParameters["UMDialPlan"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int MaxGreetingDuration
			{
				set
				{
					base.PowerSharpParameters["MaxGreetingDuration"] = value;
				}
			}

			public virtual Unlimited<int> MaxLogonAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxLogonAttempts"] = value;
				}
			}

			public virtual bool AllowCommonPatterns
			{
				set
				{
					base.PowerSharpParameters["AllowCommonPatterns"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> PINLifetime
			{
				set
				{
					base.PowerSharpParameters["PINLifetime"] = value;
				}
			}

			public virtual int PINHistoryCount
			{
				set
				{
					base.PowerSharpParameters["PINHistoryCount"] = value;
				}
			}

			public virtual bool AllowSMSNotification
			{
				set
				{
					base.PowerSharpParameters["AllowSMSNotification"] = value;
				}
			}

			public virtual DRMProtectionOptions ProtectUnauthenticatedVoiceMail
			{
				set
				{
					base.PowerSharpParameters["ProtectUnauthenticatedVoiceMail"] = value;
				}
			}

			public virtual DRMProtectionOptions ProtectAuthenticatedVoiceMail
			{
				set
				{
					base.PowerSharpParameters["ProtectAuthenticatedVoiceMail"] = value;
				}
			}

			public virtual string ProtectedVoiceMailText
			{
				set
				{
					base.PowerSharpParameters["ProtectedVoiceMailText"] = value;
				}
			}

			public virtual bool RequireProtectedPlayOnPhone
			{
				set
				{
					base.PowerSharpParameters["RequireProtectedPlayOnPhone"] = value;
				}
			}

			public virtual int MinPINLength
			{
				set
				{
					base.PowerSharpParameters["MinPINLength"] = value;
				}
			}

			public virtual string FaxMessageText
			{
				set
				{
					base.PowerSharpParameters["FaxMessageText"] = value;
				}
			}

			public virtual string UMEnabledText
			{
				set
				{
					base.PowerSharpParameters["UMEnabledText"] = value;
				}
			}

			public virtual string ResetPINText
			{
				set
				{
					base.PowerSharpParameters["ResetPINText"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SourceForestPolicyNames
			{
				set
				{
					base.PowerSharpParameters["SourceForestPolicyNames"] = value;
				}
			}

			public virtual string VoiceMailText
			{
				set
				{
					base.PowerSharpParameters["VoiceMailText"] = value;
				}
			}

			public virtual string FaxServerURI
			{
				set
				{
					base.PowerSharpParameters["FaxServerURI"] = value;
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

			public virtual Unlimited<int> LogonFailuresBeforePINReset
			{
				set
				{
					base.PowerSharpParameters["LogonFailuresBeforePINReset"] = value;
				}
			}

			public virtual bool AllowMissedCallNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowMissedCallNotifications"] = value;
				}
			}

			public virtual bool AllowFax
			{
				set
				{
					base.PowerSharpParameters["AllowFax"] = value;
				}
			}

			public virtual bool AllowTUIAccessToCalendar
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToCalendar"] = value;
				}
			}

			public virtual bool AllowTUIAccessToEmail
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToEmail"] = value;
				}
			}

			public virtual bool AllowSubscriberAccess
			{
				set
				{
					base.PowerSharpParameters["AllowSubscriberAccess"] = value;
				}
			}

			public virtual bool AllowTUIAccessToDirectory
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToDirectory"] = value;
				}
			}

			public virtual bool AllowTUIAccessToPersonalContacts
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToPersonalContacts"] = value;
				}
			}

			public virtual bool AllowAutomaticSpeechRecognition
			{
				set
				{
					base.PowerSharpParameters["AllowAutomaticSpeechRecognition"] = value;
				}
			}

			public virtual bool AllowPlayOnPhone
			{
				set
				{
					base.PowerSharpParameters["AllowPlayOnPhone"] = value;
				}
			}

			public virtual bool AllowVoiceMailPreview
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceMailPreview"] = value;
				}
			}

			public virtual bool AllowCallAnsweringRules
			{
				set
				{
					base.PowerSharpParameters["AllowCallAnsweringRules"] = value;
				}
			}

			public virtual bool AllowMessageWaitingIndicator
			{
				set
				{
					base.PowerSharpParameters["AllowMessageWaitingIndicator"] = value;
				}
			}

			public virtual bool AllowPinlessVoiceMailAccess
			{
				set
				{
					base.PowerSharpParameters["AllowPinlessVoiceMailAccess"] = value;
				}
			}

			public virtual bool AllowVoiceResponseToOtherMessageTypes
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceResponseToOtherMessageTypes"] = value;
				}
			}

			public virtual bool AllowVoiceMailAnalysis
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceMailAnalysis"] = value;
				}
			}

			public virtual bool AllowVoiceNotification
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceNotification"] = value;
				}
			}

			public virtual bool InformCallerOfVoiceMailAnalysis
			{
				set
				{
					base.PowerSharpParameters["InformCallerOfVoiceMailAnalysis"] = value;
				}
			}

			public virtual SmtpAddress? VoiceMailPreviewPartnerAddress
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerAddress"] = value;
				}
			}

			public virtual string VoiceMailPreviewPartnerAssignedID
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerAssignedID"] = value;
				}
			}

			public virtual int VoiceMailPreviewPartnerMaxMessageDuration
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerMaxMessageDuration"] = value;
				}
			}

			public virtual int VoiceMailPreviewPartnerMaxDeliveryDelay
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerMaxDeliveryDelay"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual string UMDialPlan
			{
				set
				{
					base.PowerSharpParameters["UMDialPlan"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForceUpgrade
			{
				set
				{
					base.PowerSharpParameters["ForceUpgrade"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int MaxGreetingDuration
			{
				set
				{
					base.PowerSharpParameters["MaxGreetingDuration"] = value;
				}
			}

			public virtual Unlimited<int> MaxLogonAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxLogonAttempts"] = value;
				}
			}

			public virtual bool AllowCommonPatterns
			{
				set
				{
					base.PowerSharpParameters["AllowCommonPatterns"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> PINLifetime
			{
				set
				{
					base.PowerSharpParameters["PINLifetime"] = value;
				}
			}

			public virtual int PINHistoryCount
			{
				set
				{
					base.PowerSharpParameters["PINHistoryCount"] = value;
				}
			}

			public virtual bool AllowSMSNotification
			{
				set
				{
					base.PowerSharpParameters["AllowSMSNotification"] = value;
				}
			}

			public virtual DRMProtectionOptions ProtectUnauthenticatedVoiceMail
			{
				set
				{
					base.PowerSharpParameters["ProtectUnauthenticatedVoiceMail"] = value;
				}
			}

			public virtual DRMProtectionOptions ProtectAuthenticatedVoiceMail
			{
				set
				{
					base.PowerSharpParameters["ProtectAuthenticatedVoiceMail"] = value;
				}
			}

			public virtual string ProtectedVoiceMailText
			{
				set
				{
					base.PowerSharpParameters["ProtectedVoiceMailText"] = value;
				}
			}

			public virtual bool RequireProtectedPlayOnPhone
			{
				set
				{
					base.PowerSharpParameters["RequireProtectedPlayOnPhone"] = value;
				}
			}

			public virtual int MinPINLength
			{
				set
				{
					base.PowerSharpParameters["MinPINLength"] = value;
				}
			}

			public virtual string FaxMessageText
			{
				set
				{
					base.PowerSharpParameters["FaxMessageText"] = value;
				}
			}

			public virtual string UMEnabledText
			{
				set
				{
					base.PowerSharpParameters["UMEnabledText"] = value;
				}
			}

			public virtual string ResetPINText
			{
				set
				{
					base.PowerSharpParameters["ResetPINText"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SourceForestPolicyNames
			{
				set
				{
					base.PowerSharpParameters["SourceForestPolicyNames"] = value;
				}
			}

			public virtual string VoiceMailText
			{
				set
				{
					base.PowerSharpParameters["VoiceMailText"] = value;
				}
			}

			public virtual string FaxServerURI
			{
				set
				{
					base.PowerSharpParameters["FaxServerURI"] = value;
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

			public virtual Unlimited<int> LogonFailuresBeforePINReset
			{
				set
				{
					base.PowerSharpParameters["LogonFailuresBeforePINReset"] = value;
				}
			}

			public virtual bool AllowMissedCallNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowMissedCallNotifications"] = value;
				}
			}

			public virtual bool AllowFax
			{
				set
				{
					base.PowerSharpParameters["AllowFax"] = value;
				}
			}

			public virtual bool AllowTUIAccessToCalendar
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToCalendar"] = value;
				}
			}

			public virtual bool AllowTUIAccessToEmail
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToEmail"] = value;
				}
			}

			public virtual bool AllowSubscriberAccess
			{
				set
				{
					base.PowerSharpParameters["AllowSubscriberAccess"] = value;
				}
			}

			public virtual bool AllowTUIAccessToDirectory
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToDirectory"] = value;
				}
			}

			public virtual bool AllowTUIAccessToPersonalContacts
			{
				set
				{
					base.PowerSharpParameters["AllowTUIAccessToPersonalContacts"] = value;
				}
			}

			public virtual bool AllowAutomaticSpeechRecognition
			{
				set
				{
					base.PowerSharpParameters["AllowAutomaticSpeechRecognition"] = value;
				}
			}

			public virtual bool AllowPlayOnPhone
			{
				set
				{
					base.PowerSharpParameters["AllowPlayOnPhone"] = value;
				}
			}

			public virtual bool AllowVoiceMailPreview
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceMailPreview"] = value;
				}
			}

			public virtual bool AllowCallAnsweringRules
			{
				set
				{
					base.PowerSharpParameters["AllowCallAnsweringRules"] = value;
				}
			}

			public virtual bool AllowMessageWaitingIndicator
			{
				set
				{
					base.PowerSharpParameters["AllowMessageWaitingIndicator"] = value;
				}
			}

			public virtual bool AllowPinlessVoiceMailAccess
			{
				set
				{
					base.PowerSharpParameters["AllowPinlessVoiceMailAccess"] = value;
				}
			}

			public virtual bool AllowVoiceResponseToOtherMessageTypes
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceResponseToOtherMessageTypes"] = value;
				}
			}

			public virtual bool AllowVoiceMailAnalysis
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceMailAnalysis"] = value;
				}
			}

			public virtual bool AllowVoiceNotification
			{
				set
				{
					base.PowerSharpParameters["AllowVoiceNotification"] = value;
				}
			}

			public virtual bool InformCallerOfVoiceMailAnalysis
			{
				set
				{
					base.PowerSharpParameters["InformCallerOfVoiceMailAnalysis"] = value;
				}
			}

			public virtual SmtpAddress? VoiceMailPreviewPartnerAddress
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerAddress"] = value;
				}
			}

			public virtual string VoiceMailPreviewPartnerAssignedID
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerAssignedID"] = value;
				}
			}

			public virtual int VoiceMailPreviewPartnerMaxMessageDuration
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerMaxMessageDuration"] = value;
				}
			}

			public virtual int VoiceMailPreviewPartnerMaxDeliveryDelay
			{
				set
				{
					base.PowerSharpParameters["VoiceMailPreviewPartnerMaxDeliveryDelay"] = value;
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
