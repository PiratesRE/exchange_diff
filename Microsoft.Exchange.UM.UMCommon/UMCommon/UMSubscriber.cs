using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMSubscriber : UMMailboxRecipient
	{
		public UMSubscriber(ADRecipient adrecipient)
		{
			this.Initialize(adrecipient, true);
		}

		public UMSubscriber(ADRecipient adrecipient, MailboxSession mbxSession)
		{
			this.Initialize(adrecipient, mbxSession, true);
		}

		protected UMSubscriber()
		{
		}

		public UMDialPlan DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		public bool IsFaxEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForFax;
			}
		}

		public bool IsVirtualNumberEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForVirtualNumber;
			}
		}

		public bool IsCalenderAccessEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForCalendarAccess;
			}
		}

		public bool IsEmailAccessEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForEmailAccess;
			}
		}

		public bool IsMissedCallNotificationEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForMissedCallNotification;
			}
		}

		public bool IsPinlessVoicemailAccessEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForPinlessVoiceMailAccess;
			}
		}

		public bool IsVoiceResponseToOtherMessageTypesEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForVoiceResponseToOtherMessageTypes;
			}
		}

		public bool CanAnonymousCallersLeaveMessage
		{
			get
			{
				return this.enabledFlags.EnabledForAnonymousCallerMessages;
			}
		}

		public bool IsASREnabled
		{
			get
			{
				return this.enabledFlags.EnabledForASR;
			}
		}

		public bool UseASR
		{
			get
			{
				return this.IsASREnabled && base.ConfigFolder.UseAsr;
			}
		}

		public bool IsSubscriberAccessEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForSubscriberAccess;
			}
		}

		public bool IsTUIAccessToDirectoryEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForDirectoryAccess;
			}
		}

		public bool IsTUIAccessToContactsEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForContactsAccess;
			}
		}

		public bool IsTUIAccessToAddressBookEnabled
		{
			get
			{
				return this.IsTUIAccessToDirectoryEnabled || this.IsTUIAccessToContactsEnabled;
			}
		}

		public bool IsEnabledForOutcalling
		{
			get
			{
				return this.enabledFlags.EnabledForOutcalling;
			}
		}

		public bool IsPlayOnPhoneEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForPlayOnPhone;
			}
		}

		public bool IsSmsNotificationsEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForSmsNotifications;
			}
		}

		public bool IsRequireProtectedPlayOnPhone
		{
			get
			{
				return this.enabledFlags.RequireProtectedPlayOnPhone;
			}
		}

		public bool IsPAAEnabled
		{
			get
			{
				return this.enabledFlags.EnabledForPAA;
			}
		}

		public UMMailboxPolicy UMMailboxPolicy
		{
			get
			{
				return base.InternalUMMailboxPolicy;
			}
		}

		public PasswordPolicy PasswordPolicy
		{
			get
			{
				return this.pwdPolicy;
			}
		}

		public string Extension
		{
			get
			{
				return base.ADRecipient.UMExtension;
			}
		}

		public string VirtualNumber
		{
			get
			{
				return this.Extension;
			}
		}

		public string OutboundCallingLineId
		{
			get
			{
				if (!string.IsNullOrEmpty(this.DialPlan.DefaultOutboundCallingLineId))
				{
					return this.DialPlan.DefaultOutboundCallingLineId;
				}
				if (this.IsVirtualNumberEnabled)
				{
					return this.VirtualNumber;
				}
				return this.Extension;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.authenticated;
			}
			set
			{
				this.authenticated = value;
			}
		}

		public UMMailbox ADUMMailboxSettings
		{
			get
			{
				return base.InternalADUMMailboxSettings;
			}
		}

		public override CultureInfo TelephonyCulture
		{
			get
			{
				ExAssert.RetailAssert(this.telephonyCulture != null, "TelephonyCulture: UMSubscriber not initialized");
				CultureInfo result;
				if ((result = this.telephonyCulture.Value) == null)
				{
					result = (this.DialPlan.DefaultLanguage.Culture ?? CommonConstants.DefaultCulture);
				}
				return result;
			}
		}

		public new static bool TryCreate(ADRecipient adrecipient, out UMRecipient umrecipient)
		{
			UMSubscriber umsubscriber = new UMSubscriber();
			if (umsubscriber.Initialize(adrecipient, false))
			{
				umrecipient = umsubscriber;
				return true;
			}
			umsubscriber.Dispose();
			umrecipient = null;
			return false;
		}

		public static bool IsValidSubscriber(ADRecipient recipient)
		{
			bool result;
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(recipient))
			{
				result = (umsubscriber != null);
			}
			return result;
		}

		public static bool IsValidSubscriber(string extension, UMDialPlan dialPlan, UMRecipient scopingUser)
		{
			bool result;
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromExtension<UMSubscriber>(extension, dialPlan, scopingUser))
			{
				result = (umsubscriber != null);
			}
			return result;
		}

		public static TranscriptionEnabledSetting IsPartnerTranscriptionEnabled(UMMailboxPolicy mailboxPolicy, TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig)
		{
			ValidateArgument.NotNull(mailboxPolicy, "mailboxPolicy");
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "IsPartnerTranscriptionEnabled(VoiceMailPreviewPartnerAddress = '{0}', transcriptionEnabledInMailboxConfig = '{1}'", new object[]
			{
				mailboxPolicy.VoiceMailPreviewPartnerAddress,
				transcriptionEnabledInMailboxConfig
			});
			if (mailboxPolicy.VoiceMailPreviewPartnerAddress != null && mailboxPolicy.VoiceMailPreviewPartnerAddress.Value.IsValidAddress)
			{
				return UMSubscriber.IsTranscriptionEnabled(mailboxPolicy, transcriptionEnabledInMailboxConfig);
			}
			return TranscriptionEnabledSetting.Disabled;
		}

		public static TranscriptionEnabledSetting IsTranscriptionEnabled(UMMailboxPolicy mailboxPolicy, TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig)
		{
			ValidateArgument.NotNull(mailboxPolicy, "mailboxPolicy");
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "IsTranscriptionEnabled(AllowVoiceMailPreview = '{0}', transcriptionEnabledInMailboxConfig = '{1}'", new object[]
			{
				mailboxPolicy.AllowVoiceMailPreview,
				transcriptionEnabledInMailboxConfig
			});
			if (mailboxPolicy.AllowVoiceMailPreview)
			{
				return transcriptionEnabledInMailboxConfig;
			}
			return TranscriptionEnabledSetting.Disabled;
		}

		public bool IsBlockedNumber(PhoneNumber callerId)
		{
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._User, this),
				PIIMessage.Create(PIIType._Caller, callerId)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "IsBlockedNumber(subscriber=_User, callerId=_Caller). BlockedNumbers has {0} entries.", new object[]
			{
				base.ConfigFolder.BlockedNumbers.Count
			});
			foreach (string matchString in base.ConfigFolder.BlockedNumbers)
			{
				if (callerId.IsMatch(matchString, this.DialPlan))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "IsBlockedNumber: Subscriber=_User. _PhoneNumber is in the blocked number list.", new object[0]);
					return true;
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "IsBlockedNumber: Subscriber=_User. _PhoneNumber not found in blocked number list.", new object[]
			{
				this,
				callerId
			});
			return false;
		}

		public bool HasCustomGreeting(MailboxGreetingEnum t)
		{
			return base.ConfigFolder.HasCustomMailboxGreeting(t);
		}

		public void RemoveCustomGreeting(MailboxGreetingEnum t)
		{
			base.ConfigFolder.RemoveCustomMailboxGreeting(t);
		}

		public bool IsOOF()
		{
			bool result = false;
			try
			{
				if (base.ConfigFolder == null)
				{
					return false;
				}
				result = base.ConfigFolder.IsOof;
			}
			catch (LocalizedException se)
			{
				this.LogException(se, "get OOF status");
			}
			return result;
		}

		public ITempWavFile GetGreeting()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "RecordVoicemailManager::FetchGreetingFromXSO", new object[0]);
			GreetingBase greetingBase = null;
			ITempWavFile tempWavFile = null;
			try
			{
				if (base.ConfigFolder == null)
				{
					return null;
				}
				greetingBase = (base.ConfigFolder.IsOof ? base.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away) : base.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail));
				if (greetingBase == null)
				{
					return null;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Using greeting g={0}", new object[]
				{
					greetingBase.Name
				});
				tempWavFile = greetingBase.Get();
				if (tempWavFile == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "No greeting found in XSO.", new object[0]);
				}
				else
				{
					tempWavFile.ExtraInfo = greetingBase.Name;
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Greeting g={0} was downloaded from XSO", new object[]
					{
						greetingBase.Name
					});
				}
			}
			catch (LocalizedException se)
			{
				this.LogException(se, "get greeting");
			}
			finally
			{
				if (greetingBase != null)
				{
					greetingBase.Dispose();
				}
			}
			return tempWavFile;
		}

		public IList<string> GetExtensionsInPrimaryDialPlan()
		{
			return Utils.GetExtensionsInDialPlan(this.DialPlan, base.ADRecipient);
		}

		public bool ShouldMessageBeProtected(bool callAnswering, bool messageMarkedPrivate)
		{
			DRMProtectionOptions drmprotectionOptions = callAnswering ? this.DRMPolicyForCA : this.DRMPolicyForInterpersonal;
			return drmprotectionOptions == DRMProtectionOptions.All || (drmprotectionOptions == DRMProtectionOptions.Private && messageMarkedPrivate);
		}

		internal bool IsLinkedToDialPlan(UMDialPlan dialPlanToCheck)
		{
			ValidateArgument.NotNull(dialPlanToCheck, "dialPlanToCheck");
			bool flag = this.DialPlan.Guid == dialPlanToCheck.Guid;
			if (!flag)
			{
				List<string> dialPlanPhoneContexts = UMMailbox.GetDialPlanPhoneContexts(base.ADRecipient.EmailAddresses, true);
				flag = dialPlanPhoneContexts.Exists((string currentObj) => string.Equals(dialPlanToCheck.PhoneContext, currentObj, StringComparison.OrdinalIgnoreCase));
			}
			return flag;
		}

		internal bool IsLimitedOVAAccessAllowed(UMDialPlan dp, PhoneNumber callerId)
		{
			bool flag = false;
			ValidateArgument.NotNull(dp, "dp");
			ValidateArgument.NotNull(callerId, "callerId");
			if (this.IsPinlessVoicemailAccessEnabled)
			{
				ProxyAddress address = UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(callerId.ToDial, ProxyAddressPrefix.UM.PrimaryPrefix, dp.PhoneContext);
				flag = (base.ADRecipient.EmailAddresses.Find((ProxyAddress o) => ProxyAddressBase.Equals(o, address, StringComparison.OrdinalIgnoreCase)) != null);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "IsLimitedOVAAccessAllowed(): {0} {1} entry in the proxyaddresses property of the user {2}", new object[]
				{
					flag ? "Found" : "Could not find",
					address,
					base.ADUser.LegacyExchangeDN
				});
			}
			return flag;
		}

		internal TranscriptionEnabledSetting IsTranscriptionEnabledInMailboxConfig(VoiceMailTypeEnum voiceMailType)
		{
			TranscriptionEnabledSetting transcriptionEnabledSetting = TranscriptionEnabledSetting.Enabled;
			try
			{
				switch (voiceMailType)
				{
				case VoiceMailTypeEnum.ReceivedVoiceMails:
					if (!base.ConfigFolder.ReceivedVoiceMailPreviewEnabled)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "User has transcription turned off for Received voicemails", new object[0]);
						transcriptionEnabledSetting = TranscriptionEnabledSetting.Disabled;
					}
					break;
				case VoiceMailTypeEnum.SentVoiceMails:
					if (!base.ConfigFolder.SentVoiceMailPreviewEnabled)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "User has transcription turned off for sent voicemails", new object[0]);
						transcriptionEnabledSetting = TranscriptionEnabledSetting.Disabled;
					}
					break;
				default:
					throw new InvalidArgumentException("voiceMailType");
				}
			}
			catch (StorageTransientException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Failed to get user's settings for transcription. Most likely mailbox is down. Error: {0}", new object[]
				{
					ex.ToString()
				});
				transcriptionEnabledSetting = TranscriptionEnabledSetting.Unknown;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "IsTranscriptionEnabledInMailboxConfig returned {0}", new object[]
			{
				transcriptionEnabledSetting
			});
			return transcriptionEnabledSetting;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMSubscriber>(this);
		}

		protected override bool Initialize(ADRecipient recipient, bool throwOnFailure)
		{
			bool flag = false;
			try
			{
				if (!base.Initialize(recipient, throwOnFailure))
				{
					return flag;
				}
				flag = this.InitializeInternal(recipient, throwOnFailure);
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
			return flag;
		}

		protected override bool Initialize(ADRecipient recipient, MailboxSession session, bool throwOnFailure)
		{
			bool flag = false;
			try
			{
				if (!base.Initialize(recipient, session, throwOnFailure))
				{
					return flag;
				}
				flag = this.InitializeInternal(recipient, throwOnFailure);
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
			return flag;
		}

		private bool InitializeInternal(ADRecipient recipient, bool throwOnFailure)
		{
			this.telephonyCulture = new Lazy<CultureInfo>(new Func<CultureInfo>(this.InitTelephonyCulture));
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(base.ADUser);
			if (!base.CheckField(base.ADUser.UMRecipientDialPlanId, "dialPlanId", UMRecipient.FieldMissingCheck, throwOnFailure))
			{
				return false;
			}
			this.dialPlan = iadsystemConfigurationLookup.GetDialPlanFromId(base.ADUser.UMRecipientDialPlanId);
			if (!base.CheckField(this.dialPlan, "dialPlan", UMRecipient.FieldMissingCheck, throwOnFailure))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(this.Extension) && !base.CheckField(base.ADUser.UMExtension, "UMExtensionLength", (object fieldValue) => UMUriType.TelExtn != Utils.DetermineNumberType(base.ADUser.UMExtension) || base.ADUser.UMExtension.Length == this.DialPlan.NumberOfDigitsInExtension, throwOnFailure))
			{
				return false;
			}
			if (!base.CheckField(base.InternalADUMMailboxSettings, "InternalADUMMailboxSettings", UMRecipient.FieldMissingCheck, throwOnFailure))
			{
				return false;
			}
			if (!base.CheckField(this.UMMailboxPolicy, "UMMailboxPolicy", UMRecipient.FieldMissingCheck, throwOnFailure))
			{
				return false;
			}
			this.pwdPolicy = new PasswordPolicy(this.UMMailboxPolicy);
			if (!base.CheckField(this.pwdPolicy, "PasswordPolicy", UMRecipient.FieldMissingCheck, throwOnFailure))
			{
				return false;
			}
			if (!base.CheckField(this, "UMEnabled", (object fieldValue) => (base.ADUser.UMEnabledFlags & UMEnabledFlags.UMEnabled) == UMEnabledFlags.UMEnabled, throwOnFailure))
			{
				return false;
			}
			this.enabledFlags.Initialize(this, this.UMMailboxPolicy);
			return true;
		}

		private CultureInfo InitTelephonyCulture()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "InitTelephonyCulture: lazy initialization", new object[0]);
			if (base.PreferredCultures.Length == 0)
			{
				return null;
			}
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			Server serverFromName = adtopologyLookup.GetServerFromName(Utils.GetLocalHostName());
			if (serverFromName == null)
			{
				return base.PreferredCultures[0];
			}
			if ((serverFromName.CurrentServerRole & ServerRole.UnifiedMessaging) != ServerRole.UnifiedMessaging)
			{
				return base.PreferredCultures[0];
			}
			return this.InitBestPromptCulture();
		}

		private CultureInfo InitBestPromptCulture()
		{
			CultureInfo preferredClientCulture = UmCultures.GetPreferredClientCulture(base.PreferredCultures);
			if (preferredClientCulture == null || !UmCultures.IsPromptCultureAvailable(preferredClientCulture))
			{
				return null;
			}
			return preferredClientCulture;
		}

		private void LogException(LocalizedException se, string info)
		{
			PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, this.MailAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "Exception trying to {1} from XSO for user=_EmailAddress. e={0}", new object[]
			{
				se,
				info
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToRetrieveMailboxData, null, new object[]
			{
				CallId.Id,
				this.MailAddress,
				CommonUtil.ToEventLogString(Utils.ConcatenateMessagesOnException(se))
			});
		}

		private UMDialPlan dialPlan;

		private PasswordPolicy pwdPolicy;

		private bool authenticated;

		private UmFeatureFlags enabledFlags = default(UmFeatureFlags);

		private Lazy<CultureInfo> telephonyCulture;
	}
}
