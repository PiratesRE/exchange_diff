using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class UMDialPlan : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UMDialPlan.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UMDialPlan.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return UMDialPlan.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		public int NumberOfDigitsInExtension
		{
			get
			{
				return (int)this[UMDialPlanSchema.NumberOfDigitsInExtension];
			}
			set
			{
				this[UMDialPlanSchema.NumberOfDigitsInExtension] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int LogonFailuresBeforeDisconnect
		{
			get
			{
				return (int)this[UMDialPlanSchema.LogonFailuresBeforeDisconnect];
			}
			set
			{
				this[UMDialPlanSchema.LogonFailuresBeforeDisconnect] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AccessTelephoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMDialPlanSchema.AccessTelephoneNumbers];
			}
			set
			{
				this[UMDialPlanSchema.AccessTelephoneNumbers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FaxEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.FaxEnabled];
			}
			set
			{
				this[UMDialPlanSchema.FaxEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int InputFailuresBeforeDisconnect
		{
			get
			{
				return (int)this[UMDialPlanSchema.InputFailuresBeforeDisconnect];
			}
			set
			{
				this[UMDialPlanSchema.InputFailuresBeforeDisconnect] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OutsideLineAccessCode
		{
			get
			{
				return (string)this[UMDialPlanSchema.OutsideLineAccessCode];
			}
			set
			{
				this[UMDialPlanSchema.OutsideLineAccessCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DialByNamePrimaryEnum DialByNamePrimary
		{
			get
			{
				return (DialByNamePrimaryEnum)this[UMDialPlanSchema.DialByNamePrimary];
			}
			set
			{
				this[UMDialPlanSchema.DialByNamePrimary] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DialByNameSecondaryEnum DialByNameSecondary
		{
			get
			{
				return (DialByNameSecondaryEnum)this[UMDialPlanSchema.DialByNameSecondary];
			}
			set
			{
				this[UMDialPlanSchema.DialByNameSecondary] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AudioCodecEnum AudioCodec
		{
			get
			{
				return (AudioCodecEnum)this[UMDialPlanSchema.AudioCodec];
			}
			set
			{
				this[UMDialPlanSchema.AudioCodec] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMLanguage DefaultLanguage
		{
			get
			{
				return (UMLanguage)this[UMDialPlanSchema.DefaultLanguage];
			}
			set
			{
				this[UMDialPlanSchema.DefaultLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMVoIPSecurityType VoIPSecurity
		{
			get
			{
				return (UMVoIPSecurityType)this[UMDialPlanSchema.VoIPSecurity];
			}
			set
			{
				this[UMDialPlanSchema.VoIPSecurity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxCallDuration
		{
			get
			{
				return (int)this[UMDialPlanSchema.MaxCallDuration];
			}
			set
			{
				this[UMDialPlanSchema.MaxCallDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxRecordingDuration
		{
			get
			{
				return (int)this[UMDialPlanSchema.MaxRecordingDuration];
			}
			set
			{
				this[UMDialPlanSchema.MaxRecordingDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int RecordingIdleTimeout
		{
			get
			{
				return (int)this[UMDialPlanSchema.RecordingIdleTimeout];
			}
			set
			{
				this[UMDialPlanSchema.RecordingIdleTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> PilotIdentifierList
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMDialPlanSchema.PilotIdentifierList];
			}
			set
			{
				this[UMDialPlanSchema.PilotIdentifierList] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> UMServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[UMDialPlanSchema.UMServers];
			}
		}

		public MultiValuedProperty<ADObjectId> UMMailboxPolicies
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[UMDialPlanSchema.UMMailboxPolicies];
			}
		}

		public MultiValuedProperty<ADObjectId> UMAutoAttendants
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[UMDialPlanSchema.UMAutoAttendants];
			}
		}

		[Parameter(Mandatory = false)]
		public bool WelcomeGreetingEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.WelcomeGreetingEnabled];
			}
			set
			{
				this[UMDialPlanSchema.WelcomeGreetingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutomaticSpeechRecognitionEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.AutomaticSpeechRecognitionEnabled];
			}
			set
			{
				this[UMDialPlanSchema.AutomaticSpeechRecognitionEnabled] = value;
			}
		}

		public string PhoneContext
		{
			get
			{
				return (string)this[UMDialPlanSchema.PhoneContext];
			}
			internal set
			{
				this[UMDialPlanSchema.PhoneContext] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WelcomeGreetingFilename
		{
			get
			{
				return (string)this[UMDialPlanSchema.WelcomeGreetingFilename];
			}
			set
			{
				this[UMDialPlanSchema.WelcomeGreetingFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string InfoAnnouncementFilename
		{
			get
			{
				return (string)this[UMDialPlanSchema.InfoAnnouncementFilename];
			}
			set
			{
				this[UMDialPlanSchema.InfoAnnouncementFilename] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OperatorExtension
		{
			get
			{
				return (string)this[UMDialPlanSchema.OperatorExtension];
			}
			set
			{
				this[UMDialPlanSchema.OperatorExtension] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DefaultOutboundCallingLineId
		{
			get
			{
				return (string)this[UMDialPlanSchema.DefaultOutboundCallingLineId];
			}
			set
			{
				this[UMDialPlanSchema.DefaultOutboundCallingLineId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Extension
		{
			get
			{
				return (string)this[UMDialPlanSchema.Extension];
			}
			set
			{
				this[UMDialPlanSchema.Extension] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DisambiguationFieldEnum MatchedNameSelectionMethod
		{
			get
			{
				return (DisambiguationFieldEnum)this[UMDialPlanSchema.MatchedNameSelectionMethod];
			}
			set
			{
				this[UMDialPlanSchema.MatchedNameSelectionMethod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InfoAnnouncementEnabledEnum InfoAnnouncementEnabled
		{
			get
			{
				return (InfoAnnouncementEnabledEnum)this[UMDialPlanSchema.InfoAnnouncementEnabled];
			}
			set
			{
				this[UMDialPlanSchema.InfoAnnouncementEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string InternationalAccessCode
		{
			get
			{
				return (string)this[UMDialPlanSchema.InternationalAccessCode];
			}
			set
			{
				this[UMDialPlanSchema.InternationalAccessCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string NationalNumberPrefix
		{
			get
			{
				return (string)this[UMDialPlanSchema.NationalNumberPrefix];
			}
			set
			{
				this[UMDialPlanSchema.NationalNumberPrefix] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NumberFormat InCountryOrRegionNumberFormat
		{
			get
			{
				return (NumberFormat)this[UMDialPlanSchema.InCountryOrRegionNumberFormat];
			}
			set
			{
				this[UMDialPlanSchema.InCountryOrRegionNumberFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NumberFormat InternationalNumberFormat
		{
			get
			{
				return (NumberFormat)this[UMDialPlanSchema.InternationalNumberFormat];
			}
			set
			{
				this[UMDialPlanSchema.InternationalNumberFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CallSomeoneEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.CallSomeoneEnabled];
			}
			set
			{
				this[UMDialPlanSchema.CallSomeoneEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CallSomeoneScopeEnum ContactScope
		{
			get
			{
				return (CallSomeoneScopeEnum)this[UMDialPlanSchema.ContactScope];
			}
			set
			{
				this[UMDialPlanSchema.ContactScope] = value;
			}
		}

		public ADObjectId ContactAddressList
		{
			get
			{
				return (ADObjectId)this[UMDialPlanSchema.ContactAddressList];
			}
			set
			{
				this[UMDialPlanSchema.ContactAddressList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SendVoiceMsgEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.SendVoiceMsgEnabled];
			}
			set
			{
				this[UMDialPlanSchema.SendVoiceMsgEnabled] = value;
			}
		}

		public ADObjectId UMAutoAttendant
		{
			get
			{
				return (ADObjectId)this[UMDialPlanSchema.UMAutoAttendant];
			}
			set
			{
				this[UMDialPlanSchema.UMAutoAttendant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowDialPlanSubscribers
		{
			get
			{
				return (bool)this[UMDialPlanSchema.AllowDialPlanSubscribers];
			}
			set
			{
				this[UMDialPlanSchema.AllowDialPlanSubscribers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowExtensions
		{
			get
			{
				return (bool)this[UMDialPlanSchema.AllowExtensions];
			}
			set
			{
				this[UMDialPlanSchema.AllowExtensions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInCountryOrRegionGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMDialPlanSchema.AllowedInCountryOrRegionGroups];
			}
			set
			{
				this[UMDialPlanSchema.AllowedInCountryOrRegionGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInternationalGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMDialPlanSchema.AllowedInternationalGroups];
			}
			set
			{
				this[UMDialPlanSchema.AllowedInternationalGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DialGroupEntry> ConfiguredInCountryOrRegionGroups
		{
			get
			{
				return (MultiValuedProperty<DialGroupEntry>)this[UMDialPlanSchema.ConfiguredInCountryOrRegionGroups];
			}
			set
			{
				this[UMDialPlanSchema.ConfiguredInCountryOrRegionGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LegacyPromptPublishingPoint
		{
			get
			{
				return (string)this[UMDialPlanSchema.PromptPublishingPoint];
			}
			set
			{
				this[UMDialPlanSchema.PromptPublishingPoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DialGroupEntry> ConfiguredInternationalGroups
		{
			get
			{
				return (MultiValuedProperty<DialGroupEntry>)this[UMDialPlanSchema.ConfiguredInternationalGroups];
			}
			set
			{
				this[UMDialPlanSchema.ConfiguredInternationalGroups] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> UMIPGateway
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[UMDialPlanSchema.UMIPGateway];
			}
		}

		public UMUriType URIType
		{
			get
			{
				return (UMUriType)this[UMDialPlanSchema.URIType];
			}
			internal set
			{
				this[UMDialPlanSchema.URIType] = value;
			}
		}

		public UMSubscriberType SubscriberType
		{
			get
			{
				return (UMSubscriberType)this[UMDialPlanSchema.SubscriberType];
			}
			internal set
			{
				this[UMDialPlanSchema.SubscriberType] = value;
			}
		}

		public UMGlobalCallRoutingScheme GlobalCallRoutingScheme
		{
			get
			{
				return (UMGlobalCallRoutingScheme)this[UMDialPlanSchema.GlobalCallRoutingScheme];
			}
			set
			{
				this[UMDialPlanSchema.GlobalCallRoutingScheme] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TUIPromptEditingEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.TUIPromptEditingEnabled];
			}
			set
			{
				this[UMDialPlanSchema.TUIPromptEditingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CallAnsweringRulesEnabled
		{
			get
			{
				return (bool)this[UMDialPlanSchema.PersonalAutoAttendantEnabled];
			}
			set
			{
				this[UMDialPlanSchema.PersonalAutoAttendantEnabled] = value;
			}
		}

		public bool SipResourceIdentifierRequired
		{
			get
			{
				return (bool)this[UMDialPlanSchema.SipResourceIdentifierRequired];
			}
			internal set
			{
				this[UMDialPlanSchema.SipResourceIdentifierRequired] = value;
			}
		}

		public int FDSPollingInterval
		{
			get
			{
				return (int)this[UMDialPlanSchema.FDSPollingInterval];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EquivalentDialPlanPhoneContexts
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMDialPlanSchema.EquivalentDialPlanPhoneContexts];
			}
			set
			{
				this[UMDialPlanSchema.EquivalentDialPlanPhoneContexts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<UMNumberingPlanFormat> NumberingPlanFormats
		{
			get
			{
				return (MultiValuedProperty<UMNumberingPlanFormat>)this[UMDialPlanSchema.NumberingPlanFormats];
			}
			set
			{
				this[UMDialPlanSchema.NumberingPlanFormats] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowHeuristicADCallingLineIdResolution
		{
			get
			{
				return (bool)this[UMDialPlanSchema.AllowHeuristicADCallingLineIdResolution];
			}
			set
			{
				this[UMDialPlanSchema.AllowHeuristicADCallingLineIdResolution] = value;
			}
		}

		public string CountryOrRegionCode
		{
			get
			{
				return (string)this[UMDialPlanSchema.CountryOrRegionCode];
			}
			set
			{
				this[UMDialPlanSchema.CountryOrRegionCode] = value;
			}
		}

		internal string PromptChangeKey
		{
			get
			{
				return (string)this[UMDialPlanSchema.PromptChangeKey];
			}
			set
			{
				this[UMDialPlanSchema.PromptChangeKey] = value;
			}
		}

		internal bool TryMapNumberingPlan(string number, out string mappedNumber)
		{
			mappedNumber = null;
			string text = null;
			foreach (UMNumberingPlanFormat umnumberingPlanFormat in this.NumberingPlanFormats)
			{
				if (umnumberingPlanFormat.TryMapNumber(number, out text))
				{
					if (mappedNumber != null)
					{
						mappedNumber = null;
						break;
					}
					mappedNumber = text;
				}
			}
			return null != mappedNumber;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.IsModified(ADObjectSchema.Name) && base.ObjectState != ObjectState.New)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.DPCantChangeName, base.Id, string.Empty));
			}
			if (base.IsModified(UMDialPlanSchema.URIType) && base.ObjectState != ObjectState.New)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.CantSetDialPlanProperty("URIType"), base.Id, string.Empty));
			}
			if (base.IsModified(UMDialPlanSchema.NumberOfDigitsInExtension) && base.ObjectState != ObjectState.New)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.CantSetDialPlanProperty("NumberOfDigitsInExtension"), base.Id, string.Empty));
			}
			if (this.SubscriberType == UMSubscriberType.Consumer && base.ObjectState != ObjectState.New)
			{
				if (base.IsModified(UMDialPlanSchema.CallSomeoneEnabled) && this.CallSomeoneEnabled)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.InvalidConsumerDialPlanSetting("CallSomeoneEnabled"), base.Id, string.Empty));
				}
				if (base.IsModified(UMDialPlanSchema.SendVoiceMsgEnabled) && this.SendVoiceMsgEnabled)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.InvalidConsumerDialPlanSetting("SendVoiceMsgEnabled"), base.Id, string.Empty));
				}
			}
			if (this.CallSomeoneEnabled || this.SendVoiceMsgEnabled)
			{
				if (this.ContactScope == CallSomeoneScopeEnum.Extension)
				{
					if (string.IsNullOrEmpty(this.Extension))
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCallSomeoneScopeSettings("Extension", "Extension"), base.Id, string.Empty));
					}
					else if (this.Extension.Length != this.NumberOfDigitsInExtension)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.InvalidExtension("Extension", this.NumberOfDigitsInExtension), base.Id, string.Empty));
					}
				}
				if (this.ContactScope == CallSomeoneScopeEnum.AutoAttendantLink && this.UMAutoAttendant == null)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCallSomeoneScopeSettings("AutoAttendantLink", "UMAutoAttendant"), base.Id, string.Empty));
				}
				if (this.ContactScope == CallSomeoneScopeEnum.AddressList && this.ContactAddressList == null)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.InvalidCallSomeoneScopeSettings("AddressList", "ContactAddressList"), base.Id, string.Empty));
				}
			}
			if (this.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False && string.IsNullOrEmpty(this.InfoAnnouncementFilename))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.SpecifyAnnouncementFileName, base.Id, string.Empty));
			}
			if (this.WelcomeGreetingEnabled && string.IsNullOrEmpty(this.WelcomeGreetingFilename))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.SpecifyCustomGreetingFileName, base.Id, string.Empty));
			}
			if (!this.SipResourceIdentifierRequired && (this.URIType != UMUriType.E164 || this.SubscriberType != UMSubscriberType.Enterprise))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.SipResourceIdentifierRequiredNotAllowed, base.Id, string.Empty));
			}
			if (string.IsNullOrEmpty(this.DefaultOutboundCallingLineId) && !this.SipResourceIdentifierRequired)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.MissingDefaultOutboundCallingLineId, base.Id, string.Empty));
			}
		}

		internal bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, base.Id),
				new ExistsFilter(UMDialPlanSchema.AssociatedUsers)
			});
			UMDialPlan[] array = base.Session.Find<UMDialPlan>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		internal bool CheckForAssociatedPolicies()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, base.Id),
				new ExistsFilter(UMDialPlanSchema.AssociatedPolicies)
			});
			UMDialPlan[] array = base.Session.Find<UMDialPlan>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		internal string GetDefaultExtension(string phone)
		{
			if (phone != null && phone.Length >= this.NumberOfDigitsInExtension)
			{
				return phone.Substring(phone.Length - this.NumberOfDigitsInExtension, this.NumberOfDigitsInExtension);
			}
			return null;
		}

		internal bool SupportsAirSync()
		{
			return this.URIType == UMUriType.E164 && this.SubscriberType == UMSubscriberType.Consumer && !string.IsNullOrEmpty(this.CountryOrRegionCode);
		}

		private static UMDialPlanSchema schema = ObjectSchema.GetInstance<UMDialPlanSchema>();

		private static string mostDerivedClass = "msExchUMDialPlan";

		private static ADObjectId parentPath = new ADObjectId("CN=UM DialPlan Container");
	}
}
