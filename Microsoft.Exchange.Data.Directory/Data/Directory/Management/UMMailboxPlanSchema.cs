using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class UMMailboxPlanSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition AllowUMCallsFromNonUsers = ADRecipientSchema.AllowUMCallsFromNonUsers;

		public static readonly ADPropertyDefinition CallAnsweringAudioCodec = ADUserSchema.CallAnsweringAudioCodec;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition OperatorNumber = ADUserSchema.OperatorNumber;

		public static readonly ADPropertyDefinition UMMailboxPolicy = ADUserSchema.UMMailboxPolicy;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition PhoneProviderId = ADUserSchema.PhoneProviderId;

		public static readonly ADPropertyDefinition AnonymousCallersCanLeaveMessages = ADUserSchema.AnonymousCallersCanLeaveMessages;

		public static readonly ADPropertyDefinition ASREnabled = ADUserSchema.ASREnabled;

		public static readonly ADPropertyDefinition VoiceMailAnalysisEnabled = ADUserSchema.VoiceMailAnalysisEnabled;

		public static readonly ADPropertyDefinition FaxEnabled = ADUserSchema.FaxEnabled;

		public static readonly ADPropertyDefinition MissedCallNotificationEnabled = ADUserSchema.MissedCallNotificationEnabled;

		public static readonly ADPropertyDefinition UMSMSNotificationOption = ADUserSchema.UMSMSNotificationOption;

		public static readonly ADPropertyDefinition SubscriberAccessEnabled = ADUserSchema.SubscriberAccessEnabled;

		public static readonly ADPropertyDefinition PinlessAccessToVoiceMailEnabled = ADUserSchema.PinlessAccessToVoiceMailEnabled;

		public static readonly ADPropertyDefinition TUIAccessToCalendarEnabled = ADUserSchema.TUIAccessToCalendarEnabled;

		public static readonly ADPropertyDefinition TUIAccessToEmailEnabled = ADUserSchema.TUIAccessToEmailEnabled;

		public static readonly ADPropertyDefinition PlayOnPhoneEnabled = ADUserSchema.PlayOnPhoneEnabled;

		public static readonly ADPropertyDefinition CallAnsweringRulesEnabled = ADUserSchema.CallAnsweringRulesEnabled;

		public static readonly ADPropertyDefinition UMEnabled = ADUserSchema.UMEnabled;

		public static readonly ADPropertyDefinition UMRecipientDialPlanId = ADRecipientSchema.UMRecipientDialPlanId;

		public static readonly ADPropertyDefinition UMProvisioningRequested = ADRecipientSchema.UMProvisioningRequested;
	}
}
