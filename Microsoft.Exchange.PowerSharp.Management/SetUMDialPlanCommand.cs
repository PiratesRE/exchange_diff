using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMDialPlanCommand : SyntheticCommandWithPipelineInputNoOutput<UMDialPlan>
	{
		private SetUMDialPlanCommand() : base("Set-UMDialPlan")
		{
		}

		public SetUMDialPlanCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMDialPlanCommand SetParameters(SetUMDialPlanCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMDialPlanCommand SetParameters(SetUMDialPlanCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
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

			public virtual string UMAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["UMAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual string CountryOrRegionCode
			{
				set
				{
					base.PowerSharpParameters["CountryOrRegionCode"] = value;
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

			public virtual int LogonFailuresBeforeDisconnect
			{
				set
				{
					base.PowerSharpParameters["LogonFailuresBeforeDisconnect"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AccessTelephoneNumbers
			{
				set
				{
					base.PowerSharpParameters["AccessTelephoneNumbers"] = value;
				}
			}

			public virtual bool FaxEnabled
			{
				set
				{
					base.PowerSharpParameters["FaxEnabled"] = value;
				}
			}

			public virtual int InputFailuresBeforeDisconnect
			{
				set
				{
					base.PowerSharpParameters["InputFailuresBeforeDisconnect"] = value;
				}
			}

			public virtual string OutsideLineAccessCode
			{
				set
				{
					base.PowerSharpParameters["OutsideLineAccessCode"] = value;
				}
			}

			public virtual DialByNamePrimaryEnum DialByNamePrimary
			{
				set
				{
					base.PowerSharpParameters["DialByNamePrimary"] = value;
				}
			}

			public virtual DialByNameSecondaryEnum DialByNameSecondary
			{
				set
				{
					base.PowerSharpParameters["DialByNameSecondary"] = value;
				}
			}

			public virtual AudioCodecEnum AudioCodec
			{
				set
				{
					base.PowerSharpParameters["AudioCodec"] = value;
				}
			}

			public virtual UMLanguage DefaultLanguage
			{
				set
				{
					base.PowerSharpParameters["DefaultLanguage"] = value;
				}
			}

			public virtual UMVoIPSecurityType VoIPSecurity
			{
				set
				{
					base.PowerSharpParameters["VoIPSecurity"] = value;
				}
			}

			public virtual int MaxCallDuration
			{
				set
				{
					base.PowerSharpParameters["MaxCallDuration"] = value;
				}
			}

			public virtual int MaxRecordingDuration
			{
				set
				{
					base.PowerSharpParameters["MaxRecordingDuration"] = value;
				}
			}

			public virtual int RecordingIdleTimeout
			{
				set
				{
					base.PowerSharpParameters["RecordingIdleTimeout"] = value;
				}
			}

			public virtual MultiValuedProperty<string> PilotIdentifierList
			{
				set
				{
					base.PowerSharpParameters["PilotIdentifierList"] = value;
				}
			}

			public virtual bool WelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["WelcomeGreetingEnabled"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual string WelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["WelcomeGreetingFilename"] = value;
				}
			}

			public virtual string InfoAnnouncementFilename
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementFilename"] = value;
				}
			}

			public virtual string OperatorExtension
			{
				set
				{
					base.PowerSharpParameters["OperatorExtension"] = value;
				}
			}

			public virtual string DefaultOutboundCallingLineId
			{
				set
				{
					base.PowerSharpParameters["DefaultOutboundCallingLineId"] = value;
				}
			}

			public virtual string Extension
			{
				set
				{
					base.PowerSharpParameters["Extension"] = value;
				}
			}

			public virtual DisambiguationFieldEnum MatchedNameSelectionMethod
			{
				set
				{
					base.PowerSharpParameters["MatchedNameSelectionMethod"] = value;
				}
			}

			public virtual InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementEnabled"] = value;
				}
			}

			public virtual string InternationalAccessCode
			{
				set
				{
					base.PowerSharpParameters["InternationalAccessCode"] = value;
				}
			}

			public virtual string NationalNumberPrefix
			{
				set
				{
					base.PowerSharpParameters["NationalNumberPrefix"] = value;
				}
			}

			public virtual NumberFormat InCountryOrRegionNumberFormat
			{
				set
				{
					base.PowerSharpParameters["InCountryOrRegionNumberFormat"] = value;
				}
			}

			public virtual NumberFormat InternationalNumberFormat
			{
				set
				{
					base.PowerSharpParameters["InternationalNumberFormat"] = value;
				}
			}

			public virtual bool CallSomeoneEnabled
			{
				set
				{
					base.PowerSharpParameters["CallSomeoneEnabled"] = value;
				}
			}

			public virtual CallSomeoneScopeEnum ContactScope
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

			public virtual MultiValuedProperty<DialGroupEntry> ConfiguredInCountryOrRegionGroups
			{
				set
				{
					base.PowerSharpParameters["ConfiguredInCountryOrRegionGroups"] = value;
				}
			}

			public virtual string LegacyPromptPublishingPoint
			{
				set
				{
					base.PowerSharpParameters["LegacyPromptPublishingPoint"] = value;
				}
			}

			public virtual MultiValuedProperty<DialGroupEntry> ConfiguredInternationalGroups
			{
				set
				{
					base.PowerSharpParameters["ConfiguredInternationalGroups"] = value;
				}
			}

			public virtual bool TUIPromptEditingEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIPromptEditingEnabled"] = value;
				}
			}

			public virtual bool CallAnsweringRulesEnabled
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringRulesEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EquivalentDialPlanPhoneContexts
			{
				set
				{
					base.PowerSharpParameters["EquivalentDialPlanPhoneContexts"] = value;
				}
			}

			public virtual MultiValuedProperty<UMNumberingPlanFormat> NumberingPlanFormats
			{
				set
				{
					base.PowerSharpParameters["NumberingPlanFormats"] = value;
				}
			}

			public virtual bool AllowHeuristicADCallingLineIdResolution
			{
				set
				{
					base.PowerSharpParameters["AllowHeuristicADCallingLineIdResolution"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new UMDialPlanIdParameter(value) : null);
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

			public virtual string UMAutoAttendant
			{
				set
				{
					base.PowerSharpParameters["UMAutoAttendant"] = ((value != null) ? new UMAutoAttendantIdParameter(value) : null);
				}
			}

			public virtual string CountryOrRegionCode
			{
				set
				{
					base.PowerSharpParameters["CountryOrRegionCode"] = value;
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

			public virtual int LogonFailuresBeforeDisconnect
			{
				set
				{
					base.PowerSharpParameters["LogonFailuresBeforeDisconnect"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AccessTelephoneNumbers
			{
				set
				{
					base.PowerSharpParameters["AccessTelephoneNumbers"] = value;
				}
			}

			public virtual bool FaxEnabled
			{
				set
				{
					base.PowerSharpParameters["FaxEnabled"] = value;
				}
			}

			public virtual int InputFailuresBeforeDisconnect
			{
				set
				{
					base.PowerSharpParameters["InputFailuresBeforeDisconnect"] = value;
				}
			}

			public virtual string OutsideLineAccessCode
			{
				set
				{
					base.PowerSharpParameters["OutsideLineAccessCode"] = value;
				}
			}

			public virtual DialByNamePrimaryEnum DialByNamePrimary
			{
				set
				{
					base.PowerSharpParameters["DialByNamePrimary"] = value;
				}
			}

			public virtual DialByNameSecondaryEnum DialByNameSecondary
			{
				set
				{
					base.PowerSharpParameters["DialByNameSecondary"] = value;
				}
			}

			public virtual AudioCodecEnum AudioCodec
			{
				set
				{
					base.PowerSharpParameters["AudioCodec"] = value;
				}
			}

			public virtual UMLanguage DefaultLanguage
			{
				set
				{
					base.PowerSharpParameters["DefaultLanguage"] = value;
				}
			}

			public virtual UMVoIPSecurityType VoIPSecurity
			{
				set
				{
					base.PowerSharpParameters["VoIPSecurity"] = value;
				}
			}

			public virtual int MaxCallDuration
			{
				set
				{
					base.PowerSharpParameters["MaxCallDuration"] = value;
				}
			}

			public virtual int MaxRecordingDuration
			{
				set
				{
					base.PowerSharpParameters["MaxRecordingDuration"] = value;
				}
			}

			public virtual int RecordingIdleTimeout
			{
				set
				{
					base.PowerSharpParameters["RecordingIdleTimeout"] = value;
				}
			}

			public virtual MultiValuedProperty<string> PilotIdentifierList
			{
				set
				{
					base.PowerSharpParameters["PilotIdentifierList"] = value;
				}
			}

			public virtual bool WelcomeGreetingEnabled
			{
				set
				{
					base.PowerSharpParameters["WelcomeGreetingEnabled"] = value;
				}
			}

			public virtual bool AutomaticSpeechRecognitionEnabled
			{
				set
				{
					base.PowerSharpParameters["AutomaticSpeechRecognitionEnabled"] = value;
				}
			}

			public virtual string WelcomeGreetingFilename
			{
				set
				{
					base.PowerSharpParameters["WelcomeGreetingFilename"] = value;
				}
			}

			public virtual string InfoAnnouncementFilename
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementFilename"] = value;
				}
			}

			public virtual string OperatorExtension
			{
				set
				{
					base.PowerSharpParameters["OperatorExtension"] = value;
				}
			}

			public virtual string DefaultOutboundCallingLineId
			{
				set
				{
					base.PowerSharpParameters["DefaultOutboundCallingLineId"] = value;
				}
			}

			public virtual string Extension
			{
				set
				{
					base.PowerSharpParameters["Extension"] = value;
				}
			}

			public virtual DisambiguationFieldEnum MatchedNameSelectionMethod
			{
				set
				{
					base.PowerSharpParameters["MatchedNameSelectionMethod"] = value;
				}
			}

			public virtual InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
			{
				set
				{
					base.PowerSharpParameters["InfoAnnouncementEnabled"] = value;
				}
			}

			public virtual string InternationalAccessCode
			{
				set
				{
					base.PowerSharpParameters["InternationalAccessCode"] = value;
				}
			}

			public virtual string NationalNumberPrefix
			{
				set
				{
					base.PowerSharpParameters["NationalNumberPrefix"] = value;
				}
			}

			public virtual NumberFormat InCountryOrRegionNumberFormat
			{
				set
				{
					base.PowerSharpParameters["InCountryOrRegionNumberFormat"] = value;
				}
			}

			public virtual NumberFormat InternationalNumberFormat
			{
				set
				{
					base.PowerSharpParameters["InternationalNumberFormat"] = value;
				}
			}

			public virtual bool CallSomeoneEnabled
			{
				set
				{
					base.PowerSharpParameters["CallSomeoneEnabled"] = value;
				}
			}

			public virtual CallSomeoneScopeEnum ContactScope
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

			public virtual MultiValuedProperty<DialGroupEntry> ConfiguredInCountryOrRegionGroups
			{
				set
				{
					base.PowerSharpParameters["ConfiguredInCountryOrRegionGroups"] = value;
				}
			}

			public virtual string LegacyPromptPublishingPoint
			{
				set
				{
					base.PowerSharpParameters["LegacyPromptPublishingPoint"] = value;
				}
			}

			public virtual MultiValuedProperty<DialGroupEntry> ConfiguredInternationalGroups
			{
				set
				{
					base.PowerSharpParameters["ConfiguredInternationalGroups"] = value;
				}
			}

			public virtual bool TUIPromptEditingEnabled
			{
				set
				{
					base.PowerSharpParameters["TUIPromptEditingEnabled"] = value;
				}
			}

			public virtual bool CallAnsweringRulesEnabled
			{
				set
				{
					base.PowerSharpParameters["CallAnsweringRulesEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EquivalentDialPlanPhoneContexts
			{
				set
				{
					base.PowerSharpParameters["EquivalentDialPlanPhoneContexts"] = value;
				}
			}

			public virtual MultiValuedProperty<UMNumberingPlanFormat> NumberingPlanFormats
			{
				set
				{
					base.PowerSharpParameters["NumberingPlanFormats"] = value;
				}
			}

			public virtual bool AllowHeuristicADCallingLineIdResolution
			{
				set
				{
					base.PowerSharpParameters["AllowHeuristicADCallingLineIdResolution"] = value;
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
