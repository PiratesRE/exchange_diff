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
	public sealed class UMMailboxPolicy : MailboxPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UMMailboxPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UMMailboxPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return UMMailboxPolicy.parentPath;
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

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(UMMailboxPolicySchema.PINLifetime))
			{
				this.PINLifetime = EnhancedTimeSpan.FromDays(60.0);
			}
			if (!base.IsModified(UMMailboxPolicySchema.LogonFailuresBeforePINReset))
			{
				this.LogonFailuresBeforePINReset = 5;
			}
			if (!base.IsModified(UMMailboxPolicySchema.MaxLogonAttempts))
			{
				this.MaxLogonAttempts = 15;
			}
			if (!base.IsModified(UMMailboxPolicySchema.AllowMissedCallNotifications))
			{
				this.AllowMissedCallNotifications = true;
			}
			base.StampPersistableDefaultValues();
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(UMMailboxPolicySchema.AssociatedUsers)
			});
			UMMailboxPolicy[] array = base.Session.Find<UMMailboxPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!this.LogonFailuresBeforePINReset.IsUnlimited && !this.MaxLogonAttempts.IsUnlimited && this.LogonFailuresBeforePINReset.Value >= this.MaxLogonAttempts.Value)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorLogonFailuresBeforePINReset(this.LogonFailuresBeforePINReset.Value, this.MaxLogonAttempts.ToString()), this.Identity, string.Empty));
			}
			if (this.AllowFax && string.IsNullOrEmpty(this.FaxServerURI))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.FaxServerURINoValue, UMMailboxPolicySchema.FaxServerURI, this));
			}
		}

		internal UMDialPlan GetDialPlan()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(this.UMDialPlan);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 663, "GetDialPlan", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\UMMailboxPolicy.cs");
			return tenantOrTopologyConfigurationSession.Read<UMDialPlan>(this.UMDialPlan);
		}

		[Parameter(Mandatory = false)]
		public int MaxGreetingDuration
		{
			get
			{
				return (int)this[UMMailboxPolicySchema.MaxGreetingDuration];
			}
			set
			{
				this[UMMailboxPolicySchema.MaxGreetingDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxLogonAttempts
		{
			get
			{
				return (Unlimited<int>)this[UMMailboxPolicySchema.MaxLogonAttempts];
			}
			set
			{
				this[UMMailboxPolicySchema.MaxLogonAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowCommonPatterns
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowCommonPatterns];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowCommonPatterns] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> PINLifetime
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[UMMailboxPolicySchema.PINLifetime];
			}
			set
			{
				this[UMMailboxPolicySchema.PINLifetime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PINHistoryCount
		{
			get
			{
				return (int)this[UMMailboxPolicySchema.PINHistoryCount];
			}
			set
			{
				this[UMMailboxPolicySchema.PINHistoryCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSMSNotification
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowSMSNotification];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowSMSNotification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DRMProtectionOptions ProtectUnauthenticatedVoiceMail
		{
			get
			{
				return (DRMProtectionOptions)this[UMMailboxPolicySchema.ProtectUnauthenticatedVoiceMail];
			}
			set
			{
				this[UMMailboxPolicySchema.ProtectUnauthenticatedVoiceMail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DRMProtectionOptions ProtectAuthenticatedVoiceMail
		{
			get
			{
				return (DRMProtectionOptions)this[UMMailboxPolicySchema.ProtectAuthenticatedVoiceMail];
			}
			set
			{
				this[UMMailboxPolicySchema.ProtectAuthenticatedVoiceMail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ProtectedVoiceMailText
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.ProtectedVoiceMailText];
			}
			set
			{
				this[UMMailboxPolicySchema.ProtectedVoiceMailText] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireProtectedPlayOnPhone
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.RequireProtectedPlayOnPhone];
			}
			set
			{
				this[UMMailboxPolicySchema.RequireProtectedPlayOnPhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinPINLength
		{
			get
			{
				return (int)this[UMMailboxPolicySchema.MinPINLength];
			}
			set
			{
				this[UMMailboxPolicySchema.MinPINLength] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FaxMessageText
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.FaxMessageText];
			}
			set
			{
				this[UMMailboxPolicySchema.FaxMessageText] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UMEnabledText
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.UMEnabledText];
			}
			set
			{
				this[UMMailboxPolicySchema.UMEnabledText] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ResetPINText
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.ResetPINText];
			}
			set
			{
				this[UMMailboxPolicySchema.ResetPINText] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> SourceForestPolicyNames
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxPolicySchema.SourceForestPolicyNames];
			}
			set
			{
				this[UMMailboxPolicySchema.SourceForestPolicyNames] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string VoiceMailText
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.VoiceMailText];
			}
			set
			{
				this[UMMailboxPolicySchema.VoiceMailText] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[UMMailboxPolicySchema.UMDialPlan];
			}
			set
			{
				this[UMMailboxPolicySchema.UMDialPlan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FaxServerURI
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.FaxServerURI];
			}
			set
			{
				this[UMMailboxPolicySchema.FaxServerURI] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInCountryOrRegionGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxPolicySchema.AllowedInCountryOrRegionGroups];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowedInCountryOrRegionGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedInternationalGroups
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxPolicySchema.AllowedInternationalGroups];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowedInternationalGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowDialPlanSubscribers
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowDialPlanSubscribers];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowDialPlanSubscribers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowExtensions
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowExtensions];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowExtensions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> LogonFailuresBeforePINReset
		{
			get
			{
				return (Unlimited<int>)this[UMMailboxPolicySchema.LogonFailuresBeforePINReset];
			}
			set
			{
				this[UMMailboxPolicySchema.LogonFailuresBeforePINReset] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowMissedCallNotifications
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowMissedCallNotifications];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowMissedCallNotifications] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowFax
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowFax];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowFax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTUIAccessToCalendar
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowTUIAccessToCalendar];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowTUIAccessToCalendar] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTUIAccessToEmail
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowTUIAccessToEmail];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowTUIAccessToEmail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSubscriberAccess
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowSubscriberAccess];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowSubscriberAccess] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTUIAccessToDirectory
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowTUIAccessToDirectory];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowTUIAccessToDirectory] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTUIAccessToPersonalContacts
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowTUIAccessToPersonalContacts];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowTUIAccessToPersonalContacts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowAutomaticSpeechRecognition
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowASR];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowASR] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowPlayOnPhone
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowPlayOnPhone];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowPlayOnPhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowVoiceMailPreview
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowVoiceMailPreview];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowVoiceMailPreview] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowCallAnsweringRules
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowPersonalAutoAttendant];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowPersonalAutoAttendant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowMessageWaitingIndicator
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowMessageWaitingIndicator];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowMessageWaitingIndicator] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowPinlessVoiceMailAccess
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowPinlessVoiceMailAccess];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowPinlessVoiceMailAccess] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowVoiceResponseToOtherMessageTypes
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowVoiceResponseToOtherMessageTypes];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowVoiceResponseToOtherMessageTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowVoiceMailAnalysis
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowVoiceMailAnalysis];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowVoiceMailAnalysis] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowVoiceNotification
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowVoiceNotification];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowVoiceNotification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InformCallerOfVoiceMailAnalysis
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.InformCallerOfVoiceMailAnalysis];
			}
			set
			{
				this[UMMailboxPolicySchema.InformCallerOfVoiceMailAnalysis] = value;
			}
		}

		internal bool AllowVirtualNumber
		{
			get
			{
				return (bool)this[UMMailboxPolicySchema.AllowVirtualNumber];
			}
			set
			{
				this[UMMailboxPolicySchema.AllowVirtualNumber] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress? VoiceMailPreviewPartnerAddress
		{
			get
			{
				return (SmtpAddress?)this[UMMailboxPolicySchema.VoiceMailPreviewPartnerAddress];
			}
			set
			{
				this[UMMailboxPolicySchema.VoiceMailPreviewPartnerAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string VoiceMailPreviewPartnerAssignedID
		{
			get
			{
				return (string)this[UMMailboxPolicySchema.VoiceMailPreviewPartnerAssignedID];
			}
			set
			{
				this[UMMailboxPolicySchema.VoiceMailPreviewPartnerAssignedID] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int VoiceMailPreviewPartnerMaxMessageDuration
		{
			get
			{
				return (int)this[UMMailboxPolicySchema.VoiceMailPreviewPartnerMaxMessageDuration];
			}
			set
			{
				this[UMMailboxPolicySchema.VoiceMailPreviewPartnerMaxMessageDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int VoiceMailPreviewPartnerMaxDeliveryDelay
		{
			get
			{
				return (int)this[UMMailboxPolicySchema.VoiceMailPreviewPartnerMaxDeliveryDelay];
			}
			set
			{
				this[UMMailboxPolicySchema.VoiceMailPreviewPartnerMaxDeliveryDelay] = value;
			}
		}

		private static UMMailboxPolicySchema schema = ObjectSchema.GetInstance<UMMailboxPolicySchema>();

		private static string mostDerivedClass = "msExchUMRecipientTemplate";

		private static ADObjectId parentPath = new ADObjectId("CN=UM Mailbox Policies");
	}
}
