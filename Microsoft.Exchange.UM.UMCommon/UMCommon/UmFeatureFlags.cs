using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal struct UmFeatureFlags
	{
		public void Initialize(UMSubscriber subscriber, UMMailboxPolicy policy)
		{
			if (subscriber == null)
			{
				throw new ArgumentNullException("subscriber");
			}
			if (subscriber.ADRecipient == null)
			{
				throw new ArgumentException("subscriber.ADRecipient is null");
			}
			if (policy == null)
			{
				throw new ArgumentNullException("policy");
			}
			UMMailbox adummailboxSettings = subscriber.ADUMMailboxSettings;
			this.EnabledForAnonymousCallerMessages = adummailboxSettings.AnonymousCallersCanLeaveMessages;
			this.EnabledForCalendarAccess = (policy.AllowTUIAccessToCalendar && adummailboxSettings.TUIAccessToCalendarEnabled);
			this.EnabledForEmailAccess = (policy.AllowTUIAccessToEmail && adummailboxSettings.TUIAccessToEmailEnabled);
			this.EnabledForSubscriberAccess = (policy.AllowSubscriberAccess && adummailboxSettings.SubscriberAccessEnabled);
			this.EnabledForDirectoryAccess = policy.AllowTUIAccessToDirectory;
			this.EnabledForContactsAccess = policy.AllowTUIAccessToPersonalContacts;
			this.EnabledForPlayOnPhone = (policy.AllowPlayOnPhone && adummailboxSettings.PlayOnPhoneEnabled);
			this.EnabledForMissedCallNotification = (policy.AllowMissedCallNotifications && adummailboxSettings.MissedCallNotificationEnabled);
			this.EnabledForMessageWaitingIndicator = policy.AllowMessageWaitingIndicator;
			this.EnabledForVirtualNumber = policy.AllowVirtualNumber;
			this.EnabledForPinlessVoiceMailAccess = (policy.AllowPinlessVoiceMailAccess && adummailboxSettings.PinlessAccessToVoiceMailEnabled);
			this.EnabledForSmsNotifications = (policy.AllowSMSNotification && !subscriber.ADRecipient.IsPersonToPersonTextMessagingEnabled && subscriber.ADRecipient.IsMachineToPersonTextMessagingEnabled);
			this.RequireProtectedPlayOnPhone = policy.RequireProtectedPlayOnPhone;
			this.EnabledForVoiceResponseToOtherMessageTypes = policy.AllowVoiceResponseToOtherMessageTypes;
			UMDialPlan dialPlan = subscriber.DialPlan;
			this.EnabledForFax = (dialPlan.FaxEnabled && policy.AllowFax && adummailboxSettings.FaxEnabled);
			this.EnabledForASR = (dialPlan.AutomaticSpeechRecognitionEnabled && policy.AllowAutomaticSpeechRecognition && adummailboxSettings.AutomaticSpeechRecognitionEnabled);
			this.EnabledForPAA = (dialPlan.CallAnsweringRulesEnabled && policy.AllowCallAnsweringRules && adummailboxSettings.CallAnsweringRulesEnabled);
			bool flag = DialGroups.HaveIntersection(dialPlan.ConfiguredInCountryOrRegionGroups, policy.AllowedInCountryOrRegionGroups) || DialGroups.HaveIntersection(dialPlan.ConfiguredInternationalGroups, policy.AllowedInternationalGroups);
			this.EnabledForOutcalling = (policy.AllowDialPlanSubscribers || policy.AllowExtensions || flag);
		}

		public bool EnabledForFax;

		public bool EnabledForCalendarAccess;

		public bool EnabledForEmailAccess;

		public bool EnabledForASR;

		public bool EnabledForSubscriberAccess;

		public bool EnabledForDirectoryAccess;

		public bool EnabledForContactsAccess;

		public bool EnabledForPlayOnPhone;

		public bool EnabledForSmsNotifications;

		public bool RequireProtectedPlayOnPhone;

		public bool EnabledForPAA;

		public bool EnabledForMessageWaitingIndicator;

		public bool EnabledForMissedCallNotification;

		public bool EnabledForAnonymousCallerMessages;

		public bool EnabledForOutcalling;

		public bool EnabledForVirtualNumber;

		public bool EnabledForPinlessVoiceMailAccess;

		public bool EnabledForVoiceResponseToOtherMessageTypes;
	}
}
