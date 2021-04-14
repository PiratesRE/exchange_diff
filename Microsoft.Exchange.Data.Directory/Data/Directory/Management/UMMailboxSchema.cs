using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class UMMailboxSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition AllowUMCallsFromNonUsers = ADRecipientSchema.AllowUMCallsFromNonUsers;

		public static readonly ADPropertyDefinition CallAnsweringAudioCodec = ADUserSchema.CallAnsweringAudioCodec;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition UMAddresses = ADRecipientSchema.UMAddresses;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition LinkedMasterAccount = ADRecipientSchema.LinkedMasterAccount;

		public static readonly ADPropertyDefinition OperatorNumber = ADUserSchema.OperatorNumber;

		public static readonly ADPropertyDefinition PhoneProviderId = ADUserSchema.PhoneProviderId;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition ServerLegacyDN = ADMailboxRecipientSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition ServerName = ADMailboxRecipientSchema.ServerName;

		public static readonly ADPropertyDefinition UMDtmfMap = ADRecipientSchema.UMDtmfMap;

		public static readonly ADPropertyDefinition UMMailboxPolicy = ADUserSchema.UMMailboxPolicy;

		public static readonly ADPropertyDefinition UMRecipientDialPlanId = ADRecipientSchema.UMRecipientDialPlanId;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition AnonymousCallersCanLeaveMessages = ADUserSchema.AnonymousCallersCanLeaveMessages;

		public static readonly ADPropertyDefinition ASREnabled = ADUserSchema.ASREnabled;

		public static readonly ADPropertyDefinition VoiceMailAnalysisEnabled = ADUserSchema.VoiceMailAnalysisEnabled;

		public static readonly ADPropertyDefinition Extensions = ADRecipientSchema.Extensions;

		public static readonly ADPropertyDefinition FaxEnabled = ADUserSchema.FaxEnabled;

		public static readonly ADPropertyDefinition MissedCallNotificationEnabled = ADUserSchema.MissedCallNotificationEnabled;

		public static readonly ADPropertyDefinition UMSMSNotificationOption = ADUserSchema.UMSMSNotificationOption;

		public static readonly ADPropertyDefinition PinlessAccessToVoiceMailEnabled = ADUserSchema.PinlessAccessToVoiceMailEnabled;

		public static readonly ADPropertyDefinition SIPResourceIdentifier = ADUserSchema.SIPResourceIdentifier;

		public static readonly ADPropertyDefinition PhoneNumber = ADUserSchema.PhoneNumber;

		public static readonly ADPropertyDefinition SubscriberAccessEnabled = ADUserSchema.SubscriberAccessEnabled;

		public static readonly ADPropertyDefinition TUIAccessToCalendarEnabled = ADUserSchema.TUIAccessToCalendarEnabled;

		public static readonly ADPropertyDefinition TUIAccessToEmailEnabled = ADUserSchema.TUIAccessToEmailEnabled;

		public static readonly ADPropertyDefinition PlayOnPhoneEnabled = ADUserSchema.PlayOnPhoneEnabled;

		public static readonly ADPropertyDefinition CallAnsweringRulesEnabled = ADUserSchema.CallAnsweringRulesEnabled;

		public static readonly ADPropertyDefinition UMEnabled = ADUserSchema.UMEnabled;

		public static readonly ADPropertyDefinition UMProvisioningRequested = ADRecipientSchema.UMProvisioningRequested;

		public static readonly ADPropertyDefinition AccessTelephoneNumbers = ADUserSchema.AccessTelephoneNumbers;

		public static readonly ADPropertyDefinition CallAnsweringRulesExtensions = ADUserSchema.CallAnsweringRulesExtensions;

		public static readonly ADPropertyDefinition UCSImListMigrationCompleted = ADRecipientSchema.UCSImListMigrationCompleted;
	}
}
