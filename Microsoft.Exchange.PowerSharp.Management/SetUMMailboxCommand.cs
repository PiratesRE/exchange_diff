using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMMailboxCommand : SyntheticCommandWithPipelineInputNoOutput<UMMailbox>
	{
		private SetUMMailboxCommand() : base("Set-UMMailbox")
		{
		}

		public SetUMMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMMailboxCommand SetParameters(SetUMMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMMailboxCommand SetParameters(SetUMMailboxCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string PhoneNumber
			{
				set
				{
					base.PowerSharpParameters["PhoneNumber"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AirSyncNumbers
			{
				set
				{
					base.PowerSharpParameters["AirSyncNumbers"] = value;
				}
			}

			public virtual SwitchParameter VerifyGlobalRoutingEntry
			{
				set
				{
					base.PowerSharpParameters["VerifyGlobalRoutingEntry"] = value;
				}
			}

			public virtual string UMMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["UMMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
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

			public virtual bool TUIAccessToCalendarEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIAccessToCalendarEnabled"] = value;
				}
			}

			public virtual bool FaxEnabled
			{
				set
				{
					base.PowerSharpParameters["FaxEnabled"] = value;
				}
			}

			public virtual bool TUIAccessToEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIAccessToEmailEnabled"] = value;
				}
			}

			public virtual bool SubscriberAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["SubscriberAccessEnabled"] = value;
				}
			}

			public virtual bool MissedCallNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["MissedCallNotificationEnabled"] = value;
				}
			}

			public virtual UMSMSNotificationOptions UMSMSNotificationOption
			{
				set
				{
					base.PowerSharpParameters["UMSMSNotificationOption"] = value;
				}
			}

			public virtual bool PinlessAccessToVoiceMailEnabled
			{
				set
				{
					base.PowerSharpParameters["PinlessAccessToVoiceMailEnabled"] = value;
				}
			}

			public virtual bool AnonymousCallersCanLeaveMessages
			{
				set
				{
					base.PowerSharpParameters["AnonymousCallersCanLeaveMessages"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual bool VoiceMailAnalysisEnabled
			{
				set
				{
					base.PowerSharpParameters["VoiceMailAnalysisEnabled"] = value;
				}
			}

			public virtual bool PlayOnPhoneEnabled
			{
				set
				{
					base.PowerSharpParameters["PlayOnPhoneEnabled"] = value;
				}
			}

			public virtual bool CallAnsweringRulesEnabled
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringRulesEnabled"] = value;
				}
			}

			public virtual AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
			{
				set
				{
					base.PowerSharpParameters["AllowUMCallsFromNonUsers"] = value;
				}
			}

			public virtual string OperatorNumber
			{
				set
				{
					base.PowerSharpParameters["OperatorNumber"] = value;
				}
			}

			public virtual string PhoneProviderId
			{
				set
				{
					base.PowerSharpParameters["PhoneProviderId"] = value;
				}
			}

			public virtual AudioCodecEnum? CallAnsweringAudioCodec
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringAudioCodec"] = value;
				}
			}

			public virtual bool ImListMigrationCompleted
			{
				set
				{
					base.PowerSharpParameters["ImListMigrationCompleted"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string PhoneNumber
			{
				set
				{
					base.PowerSharpParameters["PhoneNumber"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AirSyncNumbers
			{
				set
				{
					base.PowerSharpParameters["AirSyncNumbers"] = value;
				}
			}

			public virtual SwitchParameter VerifyGlobalRoutingEntry
			{
				set
				{
					base.PowerSharpParameters["VerifyGlobalRoutingEntry"] = value;
				}
			}

			public virtual string UMMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["UMMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
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

			public virtual bool TUIAccessToCalendarEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIAccessToCalendarEnabled"] = value;
				}
			}

			public virtual bool FaxEnabled
			{
				set
				{
					base.PowerSharpParameters["FaxEnabled"] = value;
				}
			}

			public virtual bool TUIAccessToEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIAccessToEmailEnabled"] = value;
				}
			}

			public virtual bool SubscriberAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["SubscriberAccessEnabled"] = value;
				}
			}

			public virtual bool MissedCallNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["MissedCallNotificationEnabled"] = value;
				}
			}

			public virtual UMSMSNotificationOptions UMSMSNotificationOption
			{
				set
				{
					base.PowerSharpParameters["UMSMSNotificationOption"] = value;
				}
			}

			public virtual bool PinlessAccessToVoiceMailEnabled
			{
				set
				{
					base.PowerSharpParameters["PinlessAccessToVoiceMailEnabled"] = value;
				}
			}

			public virtual bool AnonymousCallersCanLeaveMessages
			{
				set
				{
					base.PowerSharpParameters["AnonymousCallersCanLeaveMessages"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual bool VoiceMailAnalysisEnabled
			{
				set
				{
					base.PowerSharpParameters["VoiceMailAnalysisEnabled"] = value;
				}
			}

			public virtual bool PlayOnPhoneEnabled
			{
				set
				{
					base.PowerSharpParameters["PlayOnPhoneEnabled"] = value;
				}
			}

			public virtual bool CallAnsweringRulesEnabled
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringRulesEnabled"] = value;
				}
			}

			public virtual AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
			{
				set
				{
					base.PowerSharpParameters["AllowUMCallsFromNonUsers"] = value;
				}
			}

			public virtual string OperatorNumber
			{
				set
				{
					base.PowerSharpParameters["OperatorNumber"] = value;
				}
			}

			public virtual string PhoneProviderId
			{
				set
				{
					base.PowerSharpParameters["PhoneProviderId"] = value;
				}
			}

			public virtual AudioCodecEnum? CallAnsweringAudioCodec
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringAudioCodec"] = value;
				}
			}

			public virtual bool ImListMigrationCompleted
			{
				set
				{
					base.PowerSharpParameters["ImListMigrationCompleted"] = value;
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
