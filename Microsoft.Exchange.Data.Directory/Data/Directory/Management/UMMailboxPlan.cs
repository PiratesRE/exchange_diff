using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class UMMailboxPlan : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return UMMailboxPlan.schema;
			}
		}

		public UMMailboxPlan()
		{
		}

		public UMMailboxPlan(ADRecipient dataObject) : base(dataObject)
		{
		}

		internal static UMMailboxPlan FromDataObject(ADRecipient dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new UMMailboxPlan(dataObject);
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[UMMailboxPlanSchema.DisplayName];
			}
			set
			{
				this[UMMailboxPlanSchema.DisplayName] = value;
			}
		}

		public bool UMEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.UMEnabled];
			}
			internal set
			{
				this[UMMailboxPlanSchema.UMEnabled] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[UMMailboxPlanSchema.UMRecipientDialPlanId];
			}
			set
			{
				this[UMMailboxPlanSchema.UMRecipientDialPlanId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TUIAccessToCalendarEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.TUIAccessToCalendarEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.TUIAccessToCalendarEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FaxEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.FaxEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.FaxEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TUIAccessToEmailEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.TUIAccessToEmailEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.TUIAccessToEmailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SubscriberAccessEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.SubscriberAccessEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.SubscriberAccessEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PinlessAccessToVoiceMailEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.PinlessAccessToVoiceMailEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.PinlessAccessToVoiceMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneProviderId
		{
			get
			{
				return (string)this[UMMailboxPlanSchema.PhoneProviderId];
			}
			set
			{
				this[UMMailboxPlanSchema.PhoneProviderId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MissedCallNotificationEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.MissedCallNotificationEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.MissedCallNotificationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSMSNotificationOptions UMSMSNotificationOption
		{
			get
			{
				return (UMSMSNotificationOptions)this[UMMailboxPlanSchema.UMSMSNotificationOption];
			}
			set
			{
				this[UMMailboxPlanSchema.UMSMSNotificationOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AnonymousCallersCanLeaveMessages
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.AnonymousCallersCanLeaveMessages];
			}
			set
			{
				this[UMMailboxPlanSchema.AnonymousCallersCanLeaveMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutomaticSpeechRecognitionEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.ASREnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.ASREnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool VoiceMailAnalysisEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.VoiceMailAnalysisEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.VoiceMailAnalysisEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PlayOnPhoneEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.PlayOnPhoneEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.PlayOnPhoneEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CallAnsweringRulesEnabled
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.CallAnsweringRulesEnabled];
			}
			set
			{
				this[UMMailboxPlanSchema.CallAnsweringRulesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
		{
			get
			{
				return (AllowUMCallsFromNonUsersFlags)this[UMMailboxPlanSchema.AllowUMCallsFromNonUsers];
			}
			set
			{
				this[UMMailboxPlanSchema.AllowUMCallsFromNonUsers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OperatorNumber
		{
			get
			{
				return (string)this[UMMailboxPlanSchema.OperatorNumber];
			}
			set
			{
				this[UMMailboxPlanSchema.OperatorNumber] = value;
			}
		}

		public ADObjectId UMMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[UMMailboxPlanSchema.UMMailboxPolicy];
			}
			set
			{
				this[UMMailboxPlanSchema.UMMailboxPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AudioCodecEnum? CallAnsweringAudioCodec
		{
			get
			{
				return (AudioCodecEnum?)this[UMMailboxPlanSchema.CallAnsweringAudioCodec];
			}
			set
			{
				this[UMMailboxPlanSchema.CallAnsweringAudioCodec] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UMProvisioningRequested
		{
			get
			{
				return (bool)this[UMMailboxPlanSchema.UMProvisioningRequested];
			}
			set
			{
				this[UMMailboxPlanSchema.UMProvisioningRequested] = value;
			}
		}

		private static UMMailboxPlanSchema schema = ObjectSchema.GetInstance<UMMailboxPlanSchema>();
	}
}
